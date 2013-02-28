using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Indexes.Preprocessed.Preference
{

    [Title("HiSC Preprocessor")]
    [Description("Computes the preference vector of objects of a certain database according to the HiSC algorithm.")]
    public class HiSCPreferenceVectorIndex<V> : AbstractPreferenceVectorIndex, IPreferenceVectorIndex
    where V : INumberVector
    {
        /**
         * Logger to use
         */
        protected static Logging logger = Logging.GetLogger(typeof(HiSCPreferenceVectorIndex<V>));

        /**
         * Holds the value of parameter alpha.
         */
        protected double alpha;

        /**
         * Holds the value of parameter k.
         */
        protected int? k;

        /**
         * Constructor.
         * 
         * @param relation Relation in use
         * @param alpha Alpha value
         * @param k k value
         */
        public HiSCPreferenceVectorIndex(IRelation relation, double alpha, int? k) :
            base(relation)
        {
            this.alpha = alpha;
            this.k = k;
        }


        protected override void Preprocess()
        {
            if (relation == null || relation.Count <= 0)
            {
                throw new ArgumentException(ExceptionMessages.DATABASE_EMPTY);
            }

            storage = DataStoreUtil.MakeStorage<BitArray>(relation.GetDbIds(), DataStoreHints.Hot | DataStoreHints.Temp, typeof(BitArray));

            StringBuilder msg = new StringBuilder();

            long start = DateTime.Now.ToFileTime();
            FiniteProgress progress = logger.IsVerbose ? new FiniteProgress("Preprocessing preference vector", relation.Count, logger) : null;

            IKNNQuery knnQuery = QueryUtil.GetKNNQuery(relation, EuclideanDistanceFunction.STATIC, (int)k);

            //for (DbIdIter it = relation.iterDbIds(); it.valid(); it.advance()) {
            foreach (var it in relation.GetDbIds())
            {
                IDbId id = it.DbId;

                if (logger.IsDebugging)
                {
                    msg.Append("\n\nid = ").Append(id);
                    ///msg.Append(" ").Append(database.GetObjectLabelQuery().Get(id));
                    msg.Append("\n knns: ");
                }

                IKNNList knns = knnQuery.GetKNNForDbId(id, (int)k);
                BitArray preferenceVector = determinePreferenceVector(relation, id, knns.ToDbIds(), msg);
                storage[id] = preferenceVector;

                if (progress != null)
                {
                    progress.IncrementProcessed(logger);
                }
            }
            if (progress != null)
            {
                progress.EnsureCompleted(logger);
            }

            if (logger.IsDebugging)
            {
                logger.Debug(msg.ToString());
            }

            long end = DateTime.Now.ToFileTime();
            // TODO: re-Add timing code!
            if (logger.IsVerbose)
            {
                long elapsedTime = end - start;
                logger.Verbose(this.GetType().Name + " runtime: " + elapsedTime + " milliseconds.");
            }
        }

        /**
         * Determines the preference vector according to the specified neighbor ids.
         * 
         * @param relation the database storing the objects
         * @param id the id of the object for which the preference vector should be
         *        determined
         * @param neighborIDs the ids of the neighbors
         * @param msg a string buffer for debug messages
         * @return the preference vector
         */
        private BitArray determinePreferenceVector(IRelation relation, IDbId id, IDbIds neighborIDs, StringBuilder msg)
        {
            // variances
            double[] variances = DatabaseUtil.Variances(relation, (INumberVector)relation[(id)], neighborIDs);

            // preference vector
            BitArray preferenceVector = new BitArray(variances.Length);
            for (int d = 0; d < variances.Length; d++)
            {
                if (variances[d] < alpha)
                {
                    preferenceVector.Set(d, true);
                }
            }

            if (msg != null && logger.IsDebugging)
            {
                msg.Append("\nalpha " + alpha);
                msg.Append("\nvariances ");
                msg.Append(FormatUtil.Format(variances, ", ", 4));
                msg.Append("\npreference ");
                msg.Append(FormatUtil.Format(variances.Length, preferenceVector));
            }

            return preferenceVector;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }


        public override String LongName
        {
            get { return "HiSC Preference Vectors"; }
        }


        public override String ShortName
        {
            get { return "hisc-pref"; }
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses HiSCPreferenceVectorIndex oneway - - 芦create禄
         * 
         * @param <V> Vector type
         */
        public new class Factory : AbstractPreferenceVectorIndex.Factory
        {
            /**
             * The default value for alpha.
             */
            public static double DEFAULT_ALPHA = 0.01;

            /**
             * The maximum absolute variance along a coordinate axis. Must be in the
             * range of [0.0, 1.0).
             * <p>
             * Default value: {@link #DEFAULT_ALPHA}
             * </p>
             * <p>
             * Key: {@code -hisc.alpha}
             * </p>
             */
            public static OptionDescription ALPHA_ID = OptionDescription.GetOrCreate("hisc.alpha",
                "The maximum absolute variance along a coordinate axis.");

            /**
             * The number of nearest neighbors considered to determine the preference
             * vector. If this value is not defined, k is set to three times of the
             * dimensionality of the database objects.
             * <p>
             * Key: {@code -hisc.k}
             * </p>
             * <p>
             * Default value: three times of the dimensionality of the database objects
             * </p>
             */
            public static OptionDescription K_ID = OptionDescription.GetOrCreate("hisc.k",
                "The number of nearest neighbors considered to determine the preference vector. If this value is not defined, k ist set to three times of the dimensionality of the database objects.");

            /**
             * Holds the value of parameter {@link #ALPHA_ID}.
             */
            protected double alpha;

            /**
             * Holds the value of parameter {@link #K_ID}.
             */
            protected int? k;

            /**
             * Constructor.
             * 
             * @param alpha Alpha
             * @param k k
             */
            public Factory(double alpha, int? k) :
                base()
            {
                this.alpha = alpha;
                this.k = k;
            }


            public override IIndex Instantiate(IRelation relation)
            {
                int? usek;
                if (k == null)
                {
                    usek = 3 * DatabaseUtil.Dimensionality(relation);
                }
                else
                {
                    usek = k;
                }
                return new HiSCPreferenceVectorIndex<V>(relation, alpha, usek);
            }

            /**
             * Parameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public class Parameterizer : AbstractParameterizer
            {
                /**
                 * Holds the value of parameter {@link #ALPHA_ID}.
                 */
                protected double alpha;

                /**
                 * Holds the value of parameter {@link #K_ID}.
                 */
                protected int k;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    DoubleParameter ALPHA_PARAM = new DoubleParameter(ALPHA_ID,
                        new IntervalConstraint<double>(0.0, IntervalConstraint<double>.IntervalBoundary.OPEN,
                            1.0, IntervalConstraint<double>.IntervalBoundary.OPEN), DEFAULT_ALPHA);
                    if (config.Grab(ALPHA_PARAM))
                    {
                        alpha = ALPHA_PARAM.GetValue();
                    }

                    IntParameter K_PARAM = new IntParameter(K_ID, new GreaterConstraint<int>(0), true);
                    if (config.Grab(K_PARAM))
                    {
                        k = K_PARAM.GetValue();
                    }
                }


                protected override object MakeInstance()
                {
                    return new Factory(alpha, k);
                }
            }
        }
    }
}
