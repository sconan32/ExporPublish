using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed.Preference;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public class HiSCDistanceFunction : AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>
    {
        /**
         * Logger for debug.
         */
        static Logging logger = Logging.GetLogger(typeof(HiSCDistanceFunction));

        /**
         * Constructor.
         * 
         * @param indexFactory
         * @param epsilon
         */
        public HiSCDistanceFunction(HiSCPreferenceVectorIndex<INumberVector>.Factory indexFactory, double epsilon) :
            base(indexFactory, epsilon)
        {
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            // We can't really avoid these warnings, due to a limitation in Java
            // Generics (AFAICT)

            HiSCPreferenceVectorIndex<INumberVector> indexinst =
                (HiSCPreferenceVectorIndex<INumberVector>)indexFactory.Instantiate((IRelation)database);
            return new Instance(database, indexinst, GetEpsilon(), this);
        }

        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         * 
         * @param <V> the type of NumberVector to compute the distances in between
         */
        public new class Instance : AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>.Instance
        {
            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Preprocessed index
             * @param epsilon Epsilon
             * @param distanceFunction parent distance function
             */
            public Instance(IRelation database,
                HiSCPreferenceVectorIndex<INumberVector> index, double epsilon, HiSCDistanceFunction distanceFunction) :
                base(database, index, epsilon, distanceFunction)
            {
            }

            /**
             * Computes the correlation distance between the two specified vectors
             * according to the specified preference vectors.
             * 
             * @param v1 first vector
             * @param v2 second vector
             * @param pv1 the first preference vector
             * @param pv2 the second preference vector
             * @return the correlation distance between the two specified vectors
             */

            public override PreferenceVectorBasedCorrelationDistance CorrelationDistance(INumberVector v1, INumberVector v2, BitArray pv1, BitArray pv2)
            {
                BitArray commonPreferenceVector = (BitArray)pv1.Clone();
                commonPreferenceVector.And(pv2);
                int dim = v1.Count;

                // number of zero values in commonPreferenceVector
                int subspaceDim = dim - commonPreferenceVector.Cardinality();

                // special case: v1 and v2 are in parallel subspaces
                double dist1 = WeightedDistance(v1, v2, pv1);
                double dist2 = WeightedDistance(v1, v2, pv2);

                if (Math.Max(dist1, dist2) > epsilon)
                {
                    subspaceDim++;
                    if (logger.IsDebugging)
                    {
                        //Representation<String> rep = rep.GetObjectLabelQuery();
                        StringBuilder msg = new StringBuilder();
                        msg.Append("\ndist1 " + dist1);
                        msg.Append("\ndist2 " + dist2);
                        // msg.Append("\nv1 " + rep.Get(v1.GetID()));
                        // msg.Append("\nv2 " + rep.Get(v2.GetID()));
                        msg.Append("\nsubspaceDim " + subspaceDim);
                        msg.Append("\ncommon pv " + FormatUtil.Format(dim, commonPreferenceVector));
                        logger.Debug(msg.ToString());
                    }
                }

                // flip commonPreferenceVector for distance computation in common subspace
                BitArray inverseCommonPreferenceVector = (BitArray)commonPreferenceVector.Clone();
                inverseCommonPreferenceVector.Flip(0, dim);

                return new PreferenceVectorBasedCorrelationDistance(DatabaseUtil.Dimensionality(relation),
                    subspaceDim, WeightedDistance(v1, v2, inverseCommonPreferenceVector), commonPreferenceVector);
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                Type cls = ClassGenericsUtil.UglyCastIntoSubclass(typeof(HiSCPreferenceVectorIndex<INumberVector>.Factory));
                factory = config.TryInstantiate<HiSCPreferenceVectorIndex<INumberVector>.Factory>(cls);
            }


            protected override object MakeInstance()
            {
                return new HiSCDistanceFunction((HiSCPreferenceVectorIndex<INumberVector>.Factory)factory, epsilon);
            }
        }


    }
}
