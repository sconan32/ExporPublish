using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log.Progress;

namespace Socona.Expor.Algorithms.Clustering
{

    public abstract class AbstractProjectedDBSCAN<V> : AbstractAlgorithm, IClusteringAlgorithm
    where V : INumberVector
    {
        /**
         * Parameter to specify the distance function to determine the distance
         * between database objects, must extend
         * {@link de.lmu.ifi.dbs.elki.distance.distancefunction.LocallyWeightedDistanceFunction}
         * .
         * <p>
         * Key: {@code -projdbscan.distancefunction}
         * </p>
         * <p>
         * Default value:
         * {@link de.lmu.ifi.dbs.elki.distance.distancefunction.LocallyWeightedDistanceFunction}
         * </p>
         */
        public static OptionDescription OUTER_DISTANCE_FUNCTION_ID = OptionDescription.GetOrCreate(
            "projdbscan.outerdistancefunction", "Distance function to determine the distance between database objects.");

        /**
         * Parameter distance function
         */
        public static OptionDescription INNER_DISTANCE_FUNCTION_ID = OptionDescription.GetOrCreate(
            "projdbscan.distancefunction", "Distance function to determine the neighbors for variance analysis.");

        /**
         * Parameter to specify the maximum radius of the neighborhood to be
         * considered, must be suitable to {@link LocallyWeightedDistanceFunction}.
         * <p>
         * Key: {@code -projdbscan.epsilon}
         * </p>
         */
        public static OptionDescription EPSILON_ID = OptionDescription.GetOrCreate(
            "projdbscan.epsilon", "The maximum radius of the neighborhood to be considered.");

        /**
         * Parameter to specify the intrinsic dimensionality of the clusters to find,
         * must be an integer greater than 0.
         * <p>
         * Key: {@code -projdbscan.lambda}
         * </p>
         */
        public static OptionDescription LAMBDA_ID = OptionDescription.GetOrCreate(
            "projdbscan.lambda", "The intrinsic dimensionality of the clusters to find.");

        /**
         * Parameter to specify the threshold for minimum number of points in the
         * epsilon-neighborhood of a point, must be an integer greater than 0.
         * <p>
         * Key: {@code -projdbscan.minpts}
         * </p>
         */
        public static OptionDescription MINPTS_ID = OptionDescription.GetOrCreate(
            "projdbscan.minpts", "Threshold for minimum number of points in " +
            "the epsilon-neighborhood of a point.");

        /**
         * Holds the instance of the distance function specified by
         * {@link #INNER_DISTANCE_FUNCTION_ID}.
         */
        private LocallyWeightedDistanceFunction<V> distanceFunction;

        /**
         * Holds the value of {@link #EPSILON_ID}.
         */
        protected DoubleDistanceValue epsilon;

        /**
         * Holds the value of {@link #LAMBDA_ID}.
         */
        private int lambda;

        /**
         * Holds the value of {@link #MINPTS_ID}.
         */
        protected int minpts = 1;

        /**
         * Holds a list of clusters found.
         */
        private List<IModifiableDbIds> resultList;

        /**
         * Holds a Set of noise.
         */
        private IModifiableDbIds noise;

        /**
         * Holds a Set of processed ids.
         */
        private IModifiableDbIds processedIDs;

        /**
         * Constructor.
         * 
         * @param epsilon Epsilon
         * @param minpts MinPts parameter
         * @param distanceFunction Outer distance function
         * @param lambda Lambda value
         */
        public AbstractProjectedDBSCAN(DoubleDistanceValue epsilon, int minpts,
            LocallyWeightedDistanceFunction<V> distanceFunction, int lambda) :
            base()
        {
            this.epsilon = epsilon;
            this.minpts = minpts;
            this.distanceFunction = distanceFunction;
            this.lambda = lambda;
        }

        /**
         * Run the algorithm
         * 
         * @param database Database to process
         * @param relation Relation to process
         * @return Clustering result
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            FiniteProgress objprog = GetLogger().IsVerbose ? new FiniteProgress("Processing objects", relation.Count, GetLogger()) : null;
            IndefiniteProgress clusprog = GetLogger().IsVerbose ? new IndefiniteProgress("Number of clusters", GetLogger()) : null;
            resultList = new List<IModifiableDbIds>();
            noise = DbIdUtil.NewHashSet();
            processedIDs = DbIdUtil.NewHashSet(relation.Count);

            LocallyWeightedDistanceFunction<V>.Instance distFunc =
                (LocallyWeightedDistanceFunction<V>.Instance)distanceFunction.Instantiate(relation);
            IRangeQuery rangeQuery = database.GetRangeQuery(distFunc);

            if (relation.Count >= minpts)
            {
                //for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance())
                foreach (var iditer in relation.GetDbIds())
                {
                    if (!processedIDs.Contains(iditer))
                    {
                        expandCluster(distFunc, rangeQuery, iditer.DbId, objprog, clusprog);
                        if (processedIDs.Count == relation.Count && noise.Count == 0)
                        {
                            break;
                        }
                    }
                    if (objprog != null && clusprog != null)
                    {
                        objprog.SetProcessed(processedIDs.Count, GetLogger());
                        clusprog.SetProcessed(resultList.Count, GetLogger());
                    }
                }
            }
            else
            {
                // for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance())
                foreach (var iditer in relation.GetDbIds())
                {
                    noise.Add(iditer);
                    if (objprog != null && clusprog != null)
                    {
                        objprog.SetProcessed(processedIDs.Count, GetLogger());
                        clusprog.SetProcessed(resultList.Count, GetLogger());
                    }
                }
            }

            if (objprog != null && clusprog != null)
            {
                objprog.SetProcessed(processedIDs.Count, GetLogger());
                clusprog.SetProcessed(resultList.Count, GetLogger());
            }

            ClusterList result = new ClusterList(GetLongResultName(), GetShortResultName());
            //for (Iterator<IModifiableDbIds> resultListIter = resultList.iterator(); resultListIter.hasNext(); )
            foreach (var resultListIter in resultList)
            {
                Cluster c = new Cluster(resultListIter, ClusterModel.CLUSTER);
                result.AddCluster(c);
            }

            Cluster n = new Cluster(noise, true, ClusterModel.CLUSTER);
            result.AddCluster(n);

            if (objprog != null && clusprog != null)
            {
                objprog.SetProcessed(processedIDs.Count, GetLogger());
                clusprog.SetProcessed(resultList.Count, GetLogger());
            }
            // Signal that the progress has completed.
            if (objprog != null && clusprog != null)
            {
                objprog.EnsureCompleted(GetLogger());
                clusprog.SetCompleted(GetLogger());
            }
            return result;
        }

        /**
         * Return the long result name.
         * 
         * @return Long name for result
         */
        public abstract String GetLongResultName();

        /**
         * Return the short result name.
         * 
         * @return Short name for result
         */
        public abstract String GetShortResultName();

        /**
         * ExpandCluster function of DBSCAN.
         * 
         * @param distFunc Distance query to use
         * @param rangeQuery Range query
         * @param startObjectID the object id of the database object to start the
         *        expansion with
         * @param objprog the progress object for logging the current status
         */
        protected void expandCluster(LocallyWeightedDistanceFunction<V>.Instance distFunc,
            IRangeQuery rangeQuery, IDbId startObjectID, FiniteProgress objprog, IndefiniteProgress clusprog)
        {
            int corrDim = (distFunc.GetIndex() as ILocalProjectionIndex).GetLocalProjection(startObjectID).GetCorrelationDimension();

            if (GetLogger().IsDebugging)
            {
                GetLogger().Debug("EXPAND CLUSTER id = " + startObjectID + " " + corrDim + "\n#clusters: " + resultList.Count);
            }

            // euclidean epsilon neighborhood < minpts OR local dimensionality >
            // lambda -> noise
            //if (corrDim == null || corrDim > lambda)
            if (corrDim > lambda)
            {
                noise.Add(startObjectID);
                processedIDs.Add(startObjectID);
                if (objprog != null && clusprog != null)
                {
                    objprog.SetProcessed(processedIDs.Count, GetLogger());
                    clusprog.SetProcessed(resultList.Count, GetLogger());
                }
                return;
            }

            // compute weighted epsilon neighborhood
            IDistanceDbIdList seeds = rangeQuery.GetRangeForDbId(startObjectID, epsilon);
            // neighbors < minPts -> noise
            if (seeds.Count < minpts)
            {
                noise.Add(startObjectID);
                processedIDs.Add(startObjectID);
                if (objprog != null && clusprog != null)
                {
                    objprog.SetProcessed(processedIDs.Count, GetLogger());
                    clusprog.SetProcessed(resultList.Count, GetLogger());
                }
                return;
            }

            // try to expand the cluster
            IModifiableDbIds currentCluster = DbIdUtil.NewArray();
            foreach (IDistanceResultPair seed in seeds)
            {
                int nextID_corrDim =
                    (distFunc.GetIndex() as ILocalProjectionIndex).GetLocalProjection(seed).
                    GetCorrelationDimension();
                // nextID is not reachable from start object
                if (nextID_corrDim > lambda)
                {
                    continue;
                }

                if (!processedIDs.Contains(seed))
                {
                    currentCluster.Add(seed);
                    processedIDs.Add(seed);
                }
                else if (noise.Contains(seed))
                {
                    currentCluster.Add(seed);
                    noise.Remove(seed);
                }
            }
            seeds.RemoveAt(0);

            while (seeds.Count > 0)
            {
                IDistanceDbIdPair q = seeds[0];
                seeds.RemoveAt(0);
                int corrDim_q = (distFunc.GetIndex() as ILocalProjectionIndex).
                    GetLocalProjection(q).GetCorrelationDimension();
                // q forms no lambda-dim hyperplane
                if (corrDim_q > lambda)
                {
                    continue;
                }

                IDistanceDbIdList reachables = rangeQuery.GetRangeForDbId(q, epsilon);
                if (reachables.Count > minpts)
                {
                    foreach (IDistanceDbIdPair r in (IEnumerable<IDistanceDbIdPair>)reachables)
                    {
                        int corrDim_r = (distFunc.GetIndex() as ILocalProjectionIndex).
                            GetLocalProjection(r).GetCorrelationDimension();
                        // r is not reachable from q
                        if (corrDim_r > lambda)
                        {
                            continue;
                        }

                        bool inNoise = noise.Contains(r);
                        bool unclassified = !processedIDs.Contains(r);
                        if (inNoise || unclassified)
                        {
                            if (unclassified)
                            {
                                seeds.Add(r);
                            }
                            currentCluster.Add(r);
                            processedIDs.Add(r);
                            if (inNoise)
                            {
                                noise.Remove(r);
                            }
                            if (objprog != null && clusprog != null)
                            {
                                objprog.SetProcessed(processedIDs.Count, GetLogger());
                                int numClusters = currentCluster.Count > minpts ? resultList.Count + 1 : resultList.Count;
                                clusprog.SetProcessed(numClusters, GetLogger());
                            }
                        }
                    }
                }

                /* if(processedIDs.Count== relation.Count&& noise.Count== 0) {
                  break;
                } */
            }

            if (currentCluster.Count >= minpts)
            {
                resultList.Add(currentCluster);
            }
            else
            {
                noise.AddDbIds(currentCluster);
                noise.Add(startObjectID);
                processedIDs.Add(startObjectID);
            }

            if (objprog != null && clusprog != null)
            {
                objprog.SetProcessed(processedIDs.Count, GetLogger());
                clusprog.SetProcessed(resultList.Count, GetLogger());
            }
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(distanceFunction.GetInputTypeRestriction());
        }

        /**
         * IParameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected IDistanceFunction innerdist;

            protected IDistanceValue epsilon;

            protected LocallyWeightedDistanceFunction<V> outerdist;

            protected int minpts = -1;

            protected int lambda;

            protected void configInnerDistance(IParameterization config)
            {
                ObjectParameter<IDistanceFunction> innerdistP =
                    new ObjectParameter<IDistanceFunction>(
                        AbstractProjectedDBSCAN<V>.INNER_DISTANCE_FUNCTION_ID,
                    typeof(IDistanceFunction), typeof(EuclideanDistanceFunction));
                if (config.Grab(innerdistP))
                {
                    innerdist = innerdistP.InstantiateClass(config);
                }
            }

            protected void configEpsilon(IParameterization config, IDistanceFunction innerdist)
            {
                IDistanceValue distanceParser = innerdist != null ? innerdist.DistanceFactory : null;
                DistanceParameter epsilonP = new DistanceParameter(EPSILON_ID, distanceParser);
                if (config.Grab(epsilonP))
                {
                    epsilon = epsilonP.GetValue();
                }
            }

            protected void configMinPts(IParameterization config)
            {
                IntParameter minptsP = new IntParameter(MINPTS_ID, new GreaterConstraint<int>(0));
                if (config.Grab(minptsP))
                {
                    minpts = minptsP.GetValue();
                }
            }

            protected void configOuterDistance(IParameterization config, IDistanceValue epsilon, int minpts, Type preprocessorClass, IDistanceFunction innerdist)
            {
                ObjectParameter<LocallyWeightedDistanceFunction<V>> outerdistP = new ObjectParameter<LocallyWeightedDistanceFunction<V>>(OUTER_DISTANCE_FUNCTION_ID,
                    typeof(LocallyWeightedDistanceFunction<V>), typeof(LocallyWeightedDistanceFunction<V>));
                if (config.Grab(outerdistP))
                {
                    // parameters for the distance function
                    ListParameterization distanceFunctionParameters = new ListParameterization();
                    // distanceFunctionParameters.AddFlag(PreprocessorHandler.OMIT_PREPROCESSING_ID);
                    distanceFunctionParameters.AddParameter(
                        AbstractIndexBasedDistanceFunction<V>.INDEX_ID, preprocessorClass);
                    distanceFunctionParameters.AddParameter(
                        AbstractProjectedDBSCAN<V>.INNER_DISTANCE_FUNCTION_ID, innerdist);
                    distanceFunctionParameters.AddParameter(
                        AbstractProjectedDBSCAN<V>.EPSILON_ID, epsilon);
                    distanceFunctionParameters.AddParameter(
                        AbstractProjectedDBSCAN<V>.MINPTS_ID, minpts);
                    ChainedParameterization combinedConfig = new ChainedParameterization(distanceFunctionParameters, config);
                    combinedConfig.ErrorsTo(config);
                    outerdist = outerdistP.InstantiateClass(combinedConfig);
                }
            }

            protected void configLambda(IParameterization config)
            {
                IntParameter lambdaP = new IntParameter(LAMBDA_ID, new GreaterConstraint<int>(0));
                if (config.Grab(lambdaP))
                {
                    lambda = lambdaP.GetValue();
                }
            }
        }
    }
}
