using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Distances.DistanceFuctions.Subspace;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Results;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Indexes.Preprocessed.Preference
{
    [Description("Computes the preference vector of objects of a certain database according to the DiSH algorithm.")]
    public class DiSHPreferenceVectorIndex<V> : AbstractPreferenceVectorIndex, IPreferenceVectorIndex
    where V : INumberVector
    {
        /**
         * Logger to use
         */
        protected static Logging logger = Logging.GetLogger(typeof(DiSHPreferenceVectorIndex<V>));

        /**
         * Available strategies for determination of the preference vector.
         * 
         * @apiviz.exclude
         */
        public enum Strategy
        {
            /**
             * Apriori strategy
             */
            APRIORI,
            /**
             * Max intersection strategy
             */
            MAX_INTERSECTION
        }

        /**
         * The epsilon value for each dimension;
         */
        protected DoubleDistanceValue[] epsilon;

        /**
         * Threshold for minimum number of points in the neighborhood.
         */
        protected int minpts;

        /**
         * The strategy to determine the preference vector.
         */
        protected Strategy strategy;

        /**
         * Constructor.
         * 
         * @param relation Relation to use
         * @param epsilon Epsilon value
         * @param minpts MinPts value
         * @param strategy Strategy
         */
        public DiSHPreferenceVectorIndex(IRelation relation, DoubleDistanceValue[] epsilon, int minpts, Strategy strategy) :
            base(relation)
        {
            this.epsilon = epsilon;
            this.minpts = minpts;
            this.strategy = strategy;
        }


        protected override void Preprocess()
        {
            if (relation == null || relation.Count == 0)
            {
                throw new ArgumentException(ExceptionMessages.DATABASE_EMPTY);
            }

            storage = DataStoreUtil.MakeStorage<BitArray>(relation.GetDbIds(),
                DataStoreHints.Hot| DataStoreHints.Temp, typeof(BitArray));

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("\n eps ").Append((epsilon.ToList()));
                msg.Append("\n minpts ").Append(minpts);
                msg.Append("\n strategy ").Append(strategy);
                logger.Debug(msg.ToString());
            }

            try
            {
                long start = DateTime.Now.ToFileTime();
                FiniteProgress progress = logger.IsVerbose ?
                    new FiniteProgress("Preprocessing preference vector", relation.Count, logger) : null;

                // only one epsilon value specified
                int dim = DatabaseUtil.Dimensionality(relation);
                if (epsilon.Length == 1 && dim != 1)
                {
                    DoubleDistanceValue eps = epsilon[0];
                    epsilon = new DoubleDistanceValue[dim];
                    Array.ForEach(epsilon, (i) => { i = eps; });
                }

                // epsilons as string
                IRangeQuery[] rangeQueries = initRangeQueries(relation, dim);

                //for(DbIdIter it = relation.iterDbIds(); it.valid(); it.advance()) {\
                foreach (var it in relation.GetDbIds())
                {
                    StringBuilder msg = new StringBuilder();
                    IDbId id = it.DbId;

                    if (logger.IsDebugging)
                    {
                        msg.Append("\nid = ").Append(id);
                        // msg.Append(" ").Append(database.Get(id));
                        //msg.Append(" ").Append(database.GetObjectLabelQuery().Get(id));
                    }

                    // determine neighbors in each dimension
                    IModifiableDbIds[] allNeighbors = ClassGenericsUtil.NewArrayOfNull<IModifiableDbIds>(dim, typeof(IModifiableDbIds));
                    for (int d = 0; d < dim; d++)
                    {
                        IDistanceDbIdList qrList = rangeQueries[d].GetRangeForDbId(id, epsilon[d]);
                        allNeighbors[d] = DbIdUtil.NewHashSet(qrList.Count);
                        foreach (IDistanceResultPair qr in qrList)
                        {
                            allNeighbors[d].Add(qr.DbId);
                        }
                    }

                    if (logger.IsDebugging)
                    {
                        for (int d = 0; d < dim; d++)
                        {
                            msg.Append("\n neighbors [").Append(d).Append("]");
                            msg.Append(" (").Append(allNeighbors[d].Count).Append(") = ");
                            msg.Append(allNeighbors[d]);
                        }
                    }

                    BitArray preferenceVector = DeterminePreferenceVector(relation, allNeighbors, msg);
                    storage[id] = preferenceVector;

                    if (logger.IsDebugging)
                    {
                        logger.Debug(msg.ToString());
                    }

                    if (progress != null)
                    {
                        progress.IncrementProcessed(logger);
                    }
                }
                if (progress != null)
                {
                    progress.EnsureCompleted(logger);
                }

                long end = DateTime.Now.ToFileTime();
                // TODO: re-Add timing code!
                if (logger.IsVerbose)
                {
                    long elapsedTime = end - start;
                    logger.Verbose(this.GetType().Name + " runtime: " + elapsedTime + " milliseconds.");
                }
            }
            catch (ParameterException e)
            {
                throw new InvalidOperationException("", e);
            }
            catch (UnableToComplyException e)
            {
                throw new InvalidOperationException("", e);
            }
        }

        /**
         * Determines the preference vector according to the specified neighbor ids.
         * 
         * @param relation the database storing the objects
         * @param neighborIDs the list of ids of the neighbors in each dimension
         * @param msg a string buffer for Debug messages
         * @return the preference vector
         * @throws de.lmu.ifi.dbs.elki.utilities.optionhandling.ParameterException
         * 
         * @throws de.lmu.ifi.dbs.elki.utilities.exceptions.UnableToComplyException
         */
        private BitArray DeterminePreferenceVector(IRelation relation, IModifiableDbIds[] neighborIDs,
            StringBuilder msg)
        {
            if (strategy.Equals(Strategy.APRIORI))
            {
                return DeterminePreferenceVectorByApriori(relation, neighborIDs, msg);
            }
            else if (strategy.Equals(Strategy.MAX_INTERSECTION))
            {
                return determinePreferenceVectorByMaxIntersection(neighborIDs, msg);
            }
            else
            {
                throw new InvalidOperationException("Should never happen!");
            }
        }

        /**
         * Determines the preference vector with the apriori strategy.
         * 
         * @param relation the database storing the objects
         * @param neighborIDs the list of ids of the neighbors in each dimension
         * @param msg a string buffer for Debug messages
         * @return the preference vector
         * @throws de.lmu.ifi.dbs.elki.utilities.optionhandling.ParameterException
         * 
         * @throws de.lmu.ifi.dbs.elki.utilities.exceptions.UnableToComplyException
         * 
         */
        private BitArray DeterminePreferenceVectorByApriori(IRelation relation,
            IModifiableDbIds[] neighborIDs, StringBuilder msg)
        {
            int dimensionality = neighborIDs.Length;

            // database for apriori
            IUpdatableDatabase apriori_db = new HashmapDatabase();
            SimpleTypeInformation bitmeta = VectorFieldTypeInformation<BitVector>.Get(typeof(BitVector), dimensionality);
            //for(DbIdIter it = relation.iterDbIds(); it.valid(); it.advance()) {
            foreach (var it in relation.GetDbIds())
            {
                IDbId id = it.DbId;
                Bit[] bits = new Bit[dimensionality];
                bool allFalse = true;
                for (int d = 0; d < dimensionality; d++)
                {
                    if (neighborIDs[d].Contains(id))
                    {
                        bits[d] = new Bit(true);
                        allFalse = false;
                    }
                    else
                    {
                        bits[d] = new Bit(false);
                    }
                }
                if (!allFalse)
                {
                    SingleObjectBundle oaa = new SingleObjectBundle();
                    oaa.Append(bitmeta, new BitVector(bits));
                    apriori_db.Insert(oaa);
                }
            }
            APRIORI apriori = new APRIORI(minpts);
            AprioriResult aprioriResult = (AprioriResult)apriori.Run(apriori_db);

            // result of apriori
            List<BitArray> frequentItemsets = aprioriResult.GetSolution();
            IDictionary<BitArray, Int32?> supports = aprioriResult.GetSupports();
            if (logger.IsDebugging)
            {
                msg.Append("\n Frequent itemsets: " + frequentItemsets);
                msg.Append("\n All supports: " + supports);
            }
            int maxSupport = 0;
            int maxCardinality = 0;
            BitArray preferenceVector = new BitArray(1000);
            foreach (BitArray bitSet in frequentItemsets)
            {
                int cardinality = bitSet.Cardinality();
                if ((maxCardinality < cardinality) || (maxCardinality == cardinality && maxSupport == supports[(bitSet)]))
                {
                    preferenceVector = bitSet;
                    maxCardinality = cardinality;
                    maxSupport = supports[(bitSet)].Value;
                }
            }

            if (logger.IsDebugging)
            {
                msg.Append("\n preference ");
                msg.Append(FormatUtil.Format(dimensionality, preferenceVector));
                msg.Append("\n");
                logger.Debug(msg.ToString());
            }

            return preferenceVector;
        }

        /**
         * Determines the preference vector with the max intersection strategy.
         * 
         * @param neighborIDs the list of ids of the neighbors in each dimension
         * @param msg a string buffer for Debug messages
         * @return the preference vector
         */
        private BitArray determinePreferenceVectorByMaxIntersection(IModifiableDbIds[] neighborIDs, StringBuilder msg)
        {
            int dimensionality = neighborIDs.Length;
            BitArray preferenceVector = new BitArray(dimensionality);

            IDictionary<Int32, IModifiableDbIds> candidates = new Dictionary<Int32, IModifiableDbIds>(dimensionality);
            for (int i = 0; i < dimensionality; i++)
            {
                IModifiableDbIds s_i = neighborIDs[i];
                if (s_i.Count > minpts)
                {
                    candidates[i] = s_i;
                }
            }
            if (logger.IsDebugging)
            {
                msg.Append("\n candidates " + candidates.Keys);
            }

            if (candidates.Count > 0)
            {
                int i = max(candidates);
                IModifiableDbIds intersection = candidates[i];
                candidates.Remove(i);
                preferenceVector.Set(i, true);
                while (candidates.Count > 0)
                {
                    IModifiableDbIds newIntersection = DbIdUtil.NewHashSet();
                    i = maxIntersection(candidates, intersection, newIntersection);
                    IModifiableDbIds s_i = candidates[i];
                    candidates.Remove(i);
                    // TODO: aren't we re-computing the same intersection here?
                    newIntersection = DbIdUtil.Intersection(intersection, s_i);
                    intersection = newIntersection;

                    if (intersection.Count < minpts)
                    {
                        break;
                    }
                    else
                    {
                        preferenceVector.Set(i, true);
                    }
                }
            }

            if (logger.IsDebugging)
            {
                msg.Append("\n preference ");
                msg.Append(FormatUtil.Format(dimensionality, preferenceVector));
                msg.Append("\n");
                logger.Debug(msg.ToString());
            }

            return preferenceVector;
        }

        /**
         * Returns the set with the maximum size contained in the specified map.
         * 
         * @param candidates the map containing the sets
         * @return the set with the maximum size
         */
        private int max(IDictionary<Int32, IModifiableDbIds> candidates)
        {
            IDbIds maxSet = null;
            Int32 maxDim = 0;
            foreach (Int32 nextDim in candidates.Keys)
            {
                IDbIds nextSet = candidates[(nextDim)];
                if (maxSet == null || maxSet.Count < nextSet.Count)
                {
                    maxSet = nextSet;
                    maxDim = nextDim;
                }
            }

            return maxDim;
        }

        /**
         * Returns the index of the set having the maximum intersection set with the
         * specified set contained in the specified map.
         * 
         * @param candidates the map containing the sets
         * @param set the set to intersect with
         * @param result the set to put the result in
         * @return the set with the maximum size
         */
        private int maxIntersection(IDictionary<Int32, IModifiableDbIds> candidates, IDbIds set, IModifiableDbIds result)
        {
            Int32 maxDim = 0;
            foreach (Int32 nextDim in candidates.Keys)
            {
                IDbIds nextSet = candidates[(nextDim)];
                IModifiableDbIds nextIntersection = DbIdUtil.Intersection(set, nextSet);
                if (result.Count < nextIntersection.Count)
                {
                    result = nextIntersection;
                    maxDim = nextDim;
                }
            }

            return maxDim;
        }

        /**
         * Initializes the dimension selecting distancefunctions to determine the
         * preference vectors.
         * 
         * @param relation the database storing the objects
         * @param dimensionality the dimensionality of the objects
         * @return the dimension selecting distancefunctions to determine the
         *         preference vectors
         * @throws ParameterException
         */
        private IRangeQuery[] initRangeQueries(IRelation relation, int dimensionality)
        {
            Type rqcls = ClassGenericsUtil.UglyCastIntoSubclass(typeof(IRangeQuery));
            IRangeQuery[] rangeQueries = ClassGenericsUtil.NewArrayOfNull<IRangeQuery>(dimensionality, rqcls);
            for (int d = 0; d < dimensionality; d++)
            {
                rangeQueries[d] = relation.GetDatabase().GetRangeQuery(
                    new PrimitiveDistanceQuery<INumberVector>(relation, new DimensionSelectingDistanceFunction(d + 1)));
            }
            return rangeQueries;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }


        public override String LongName
        {
            get { return "DiSH Preference Vectors"; }
        }


        public override String ShortName
        {
            get { return "dish-pref"; }
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses DiSHPreferenceVectorIndex oneway - - 芦create禄
         * 
         * @param <V> Vector type
         */
        public new class Factory : AbstractPreferenceVectorIndex.Factory
        {
            /**
             * The default value for epsilon.
             */
            public static DoubleDistanceValue DEFAULT_EPSILON = new DoubleDistanceValue(0.001);

            /**
             * A comma separated list of positive doubles specifying the maximum radius
             * of the neighborhood to be considered in each dimension for determination
             * of the preference vector (default is {@link #DEFAULT_EPSILON} in each
             * dimension). If only one value is specified, this value will be used for
             * each dimension.
             * 
             * <p>
             * Key: {@code -dish.epsilon}
             * </p>
             * <p>
             * Default value: {@link #DEFAULT_EPSILON}
             * </p>
             */
            public static OptionDescription EPSILON_ID = OptionDescription.GetOrCreate(
                "dish.epsilon", "A comma separated list of positive doubles specifying the " +
                "maximum radius of the neighborhood to be " + "considered in each dimension for determination of " + "the preference vector " + "(default is " + DEFAULT_EPSILON + " in each dimension). " + "If only one value is specified, this value " + "will be used for each dimension.");

            /**
             * Option name for {@link #MINPTS_ID}.
             */
            public static String MINPTS_P = "dish.minpts";

            /**
             * Description for the determination of the preference vector.
             */
            private static String CONDITION = "The value of the preference vector in dimension d_i is set to 1 " +
                "if the epsilon neighborhood Contains more than " + MINPTS_P +
                " points and the following condition holds: " +
                "for all dimensions d_j: " + "|neighbors(d_i) intersection neighbors(d_j)| >= " + MINPTS_P + ".";

            /**
             * Positive threshold for minimum numbers of points in the
             * epsilon-neighborhood of a point, must satisfy following
             * {@link #CONDITION}.
             * 
             * <p>
             * Key: {@code -dish.minpts}
             * </p>
             */
            public static OptionDescription MINPTS_ID = OptionDescription.GetOrCreate(
                MINPTS_P, "Positive threshold for minumum numbers of points in the epsilon-" +
                "neighborhood of a point. " + CONDITION);

            /**
             * Default strategy.
             */
            public static Strategy DEFAULT_STRATEGY = Strategy.MAX_INTERSECTION;

            /**
             * The strategy for determination of the preference vector, available
             * strategies are: {@link Strategy#APRIORI } and
             * {@link Strategy#MAX_INTERSECTION}.
             * 
             * <p>
             * Key: {@code -dish.strategy}
             * </p>
             * <p>
             * Default value: {@link #DEFAULT_STRATEGY}
             * </p>
             */
            public static OptionDescription STRATEGY_ID = OptionDescription.GetOrCreate(
                "dish.strategy", "The strategy for determination of the preference vector, " +
                "available strategies are: [" + Strategy.APRIORI + "| " + Strategy.MAX_INTERSECTION + "]" +
                "(default is " + DEFAULT_STRATEGY + ")");

            /**
             * The epsilon value for each dimension;
             */
            protected DoubleDistanceValue[] epsilon;

            /**
             * Threshold for minimum number of points in the neighborhood.
             */
            protected int minpts;

            /**
             * The strategy to determine the preference vector.
             */
            protected Strategy strategy;

            /**
             * Constructor.
             * 
             * @param epsilon Epsilon
             * @param minpts Minpts
             * @param strategy Strategy
             */
            public Factory(DoubleDistanceValue[] epsilon, int minpts, Strategy strategy) :
                base()
            {
                this.epsilon = epsilon;
                this.minpts = minpts;
                this.strategy = strategy;
            }


            public override IIndex Instantiate(IRelation relation)
            {
                return new DiSHPreferenceVectorIndex<V>(relation, epsilon, minpts, strategy);
            }

            /**
             * Return the minpts value
             * 
             * @return minpts
             */
            public int GetMinpts()
            {
                return minpts;
            }

            /**
             * IParameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public class Parameterizer : AbstractParameterizer
            {
                /**
                 * The epsilon value for each dimension;
                 */
                protected DoubleDistanceValue[] epsilon;

                /**
                 * Threshold for minimum number of points in the neighborhood.
                 */
                protected int minpts;

                /**
                 * The strategy to determine the preference vector.
                 */
                protected Strategy strategy;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    IntParameter minptsP = new IntParameter(MINPTS_ID, new GreaterConstraint<int>(0));
                    if (config.Grab(minptsP))
                    {
                        minpts = minptsP.GetValue();
                    }

                    // parameter epsilon
                    // todo: constraint auf positive werte
                    List<Double> defaultEps = new List<Double>();
                    defaultEps.Add(DEFAULT_EPSILON.DoubleValue());
                    DoubleListParameter epsilonP = new DoubleListParameter(EPSILON_ID, true);
                    epsilonP.SetDefaultValue(defaultEps);
                    if (config.Grab(epsilonP))
                    {
                        IList<Double> eps_list = epsilonP.GetValue();
                        epsilon = new DoubleDistanceValue[eps_list.Count];

                        for (int d = 0; d < eps_list.Count; d++)
                        {
                            epsilon[d] = new DoubleDistanceValue(eps_list[(d)]);
                            if (epsilon[d].DoubleValue() < 0)
                            {
                                config.ReportError(new WrongParameterValueException(epsilonP, eps_list.ToString()));
                            }
                        }
                    }

                    // parameter strategy
                    EnumParameter strategyP = new EnumParameter(STRATEGY_ID, typeof(Strategy), DEFAULT_STRATEGY);
                    if (config.Grab(strategyP))
                    {
                        strategy = (Strategy)strategyP.GetValue();
                    }
                }


                protected override object MakeInstance()
                {
                    return new Factory(epsilon, minpts, strategy);
                }
            }
        }
    }
}
