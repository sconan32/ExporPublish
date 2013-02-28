using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Distances.SimilarityFunctions;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Algorithms.Clustering
{

    [Title("SNN: Shared Nearest Neighbor Clustering")]
    [Description("Algorithm to find shared-nearest-neighbors-density-connected sets in a database based on the " + "parameters 'minPts' and 'epsilon' (specifying a volume). " + "These two parameters determine a density threshold for clustering.")]
    [Reference(Authors = "L. Ert枚z, M. Steinbach, V. Kumar",
        Title = "Finding Clusters of Different Sizes, Shapes, and Densities in Noisy, High Dimensional Data",
        BookTitle = "Proc. of SIAM Data Mining (SDM), 2003",
        Url = "http://www.siam.org/meetings/sdm03/proceedings/sdm03_05.pdf")]
    public class SNNClustering : AbstractAlgorithm, IClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(SNNClustering));

        /**
         * Parameter to specify the minimum SNN density, must be an integer greater
         * than 0.
         */
        public static OptionDescription EPSILON_ID = OptionDescription.GetOrCreate("snn.epsilon", "The minimum SNN density.");

        /**
         * Holds the value of {@link #EPSILON_ID}.
         */
        private Int32DistanceValue epsilon;

        /**
         * Parameter to specify the threshold for minimum number of points in the
         * epsilon-SNN-neighborhood of a point, must be an integer greater than 0.
         */
        public static OptionDescription MINPTS_ID = OptionDescription.GetOrCreate("snn.minpts", "Threshold for minimum number of points in " + "the epsilon-SNN-neighborhood of a point.");

        /**
         * Holds the value of {@link #MINPTS_ID}.
         */
        private int minpts;

        /**
         * Holds a list of clusters found.
         */
        protected List<IModifiableDbIds> resultList;

        /**
         * Holds a set of noise.
         */
        protected IModifiableDbIds noise;

        /**
         * Holds a set of processed ids.
         */
        protected IModifiableDbIds processedIDs;

        /**
         * The similarity function for the shared nearest neighbor similarity.
         */
        private SharedNearestNeighborSimilarityFunction<INumberVector> similarityFunction;

        /**
         * Constructor.
         * 
         * @param similarityFunction Similarity function
         * @param epsilon Epsilon
         * @param minpts Minpts
         */
        public SNNClustering(SharedNearestNeighborSimilarityFunction<INumberVector> similarityFunction,
            Int32DistanceValue epsilon, int minpts) :
            base()
        {
            this.similarityFunction = similarityFunction;
            this.epsilon = epsilon;
            this.minpts = minpts;
        }

        /**
         * Perform SNN clustering
         * 
         * @param database Database
         * @param relation Relation
         * @return IResult
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            ISimilarityQuery snnInstance = similarityFunction.Instantiate(relation);

            FiniteProgress objprog = logger.IsVerbose ? new FiniteProgress("SNNClustering", relation.Count, logger) : null;
            IndefiniteProgress clusprog = logger.IsVerbose ? new IndefiniteProgress("Number of clusters", logger) : null;
            resultList = new List<IModifiableDbIds>();
            noise = DbIdUtil.NewHashSet();
            processedIDs = DbIdUtil.NewHashSet(relation.Count);
            if (relation.Count >= minpts)
            {
                //for(DbIdIter id = snnInstance.GetRelation().iterDbIds(); id.valid(); id.advance()) {
                foreach (var id in snnInstance.Relation.GetDbIds())
                {
                    if (!processedIDs.Contains(id))
                    {
                        ExpandCluster(snnInstance, id.DbId, objprog, clusprog);
                        if (processedIDs.Count == relation.Count && noise.Count == 0)
                        {
                            break;
                        }
                    }
                    if (objprog != null && clusprog != null)
                    {
                        objprog.SetProcessed(processedIDs.Count, logger);
                        clusprog.SetProcessed(resultList.Count, logger);
                    }
                }
            }
            else
            {
                //for(DbIdIter id = snnInstance.GetRelation().iterDbIds(); id.valid(); id.advance()) {
                foreach (var id in snnInstance.Relation.GetDbIds())
                {
                    noise.Add(id);
                    if (objprog != null && clusprog != null)
                    {
                        objprog.SetProcessed(noise.Count, logger);
                        clusprog.SetProcessed(resultList.Count, logger);
                    }
                }
            }
            // Finish progress logging
            if (objprog != null && clusprog != null)
            {
                objprog.EnsureCompleted(logger);
                clusprog.SetCompleted(logger);
            }

            ClusterList result = new ClusterList("Shared-Nearest-Neighbor Clustering", "snn-clustering");
            //for (Iterator<IModifiableDbIds> resultListIter = resultList.iterator(); resultListIter.hasNext(); )
            foreach (var resultItem in resultList)
            {
                result.AddCluster(new Cluster(resultItem, ClusterModel.CLUSTER));
            }
            result.AddCluster(new Cluster(noise, true, ClusterModel.CLUSTER));

            return result;
        }

        /**
         * Returns the shared nearest neighbors of the specified query object in the
         * given database.
         * 
         * @param snnInstance shared nearest neighbors
         * @param queryObject the query object
         * @return the shared nearest neighbors of the specified query object in the
         *         given database
         */
        protected IArrayModifiableDbIds FindSNNNeighbors(ISimilarityQuery snnInstance, IDbId queryObject)
        {
            IArrayModifiableDbIds neighbors = DbIdUtil.NewArray();
            //for(DbIdIter iditer = snnInstance.GetRelation().iterDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in snnInstance.Relation.GetDbIds())
            {
                if (snnInstance.Similarity(queryObject, iditer).CompareTo(epsilon) >= 0)
                {
                    neighbors.Add(iditer);
                }
            }
            return neighbors;
        }

        /**
         * DBSCAN-function expandCluster adapted to SNN criterion.
         * <p/>
         * <p/>
         * Border-Objects become members of the first possible cluster.
         * 
         * @param snnInstance shared nearest neighbors
         * @param startObjectID potential seed of a new potential cluster
         * @param objprog the progress object to report about the progress of
         *        clustering
         */
        protected void ExpandCluster(ISimilarityQuery snnInstance, IDbId startObjectID, FiniteProgress objprog, IndefiniteProgress clusprog)
        {
            IArrayModifiableDbIds seeds = FindSNNNeighbors(snnInstance, startObjectID);

            // startObject is no core-object
            if (seeds.Count < minpts)
            {
                noise.Add(startObjectID);
                processedIDs.Add(startObjectID);
                if (objprog != null && clusprog != null)
                {
                    objprog.SetProcessed(processedIDs.Count, logger);
                    clusprog.SetProcessed(resultList.Count, logger);
                }
                return;
            }

            // try to expand the cluster
            IModifiableDbIds currentCluster = DbIdUtil.NewArray();
            //for(DbIdIter seed = seeds.iter(); seed.valid(); seed.advance()) {
            foreach (var seed in seeds)
            {
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

            while (seeds.Count > 0)
            {
                IDbId o = seeds.RemoveAt(seeds.Count - 1);
                IArrayModifiableDbIds neighborhood = FindSNNNeighbors(snnInstance, o);

                if (neighborhood.Count >= minpts)
                {
                    //for(DbIdIter iter = neighborhood.iter(); iter.valid(); iter.advance()) {
                    foreach (var iter in neighborhood)
                    {
                        IDbId p = iter.DbId;
                        bool inNoise = noise.Contains(p);
                        bool unclassified = !processedIDs.Contains(p);
                        if (inNoise || unclassified)
                        {
                            if (unclassified)
                            {
                                seeds.Add(p);
                            }
                            currentCluster.Add(p);
                            processedIDs.Add(p);
                            if (inNoise)
                            {
                                noise.Remove(p);
                            }
                        }
                    }
                }

                if (objprog != null && clusprog != null)
                {
                    objprog.SetProcessed(processedIDs.Count, logger);
                    int numClusters = currentCluster.Count > minpts ? resultList.Count + 1 : resultList.Count;
                    clusprog.SetProcessed(numClusters, logger);
                }

                if (processedIDs.Count == snnInstance.Relation.Count && noise.Count == 0)
                {
                    break;
                }
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
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(similarityFunction.GetInputTypeRestriction());
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         * 
         * @param <O> object type
         */
        public class Parameterizer : AbstractParameterizer
        {
            protected Int32DistanceValue epsilon;

            protected int minpts;

            private SharedNearestNeighborSimilarityFunction<INumberVector> similarityFunction;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                Type cls = ClassGenericsUtil.UglyCastIntoSubclass(
                  typeof(SharedNearestNeighborSimilarityFunction<INumberVector>));
                similarityFunction = config.TryInstantiate<SharedNearestNeighborSimilarityFunction<INumberVector>>(cls);

                DistanceParameter epsilonP = new DistanceParameter(EPSILON_ID, Int32DistanceValue.FACTORY);
                if (config.Grab(epsilonP))
                {
                    epsilon = (Int32DistanceValue)epsilonP.GetValue();
                }

                IntParameter minptsP = new IntParameter(MINPTS_ID, new GreaterConstraint<int>(0));
                if (config.Grab(minptsP))
                {
                    minpts = minptsP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new SNNClustering(similarityFunction, epsilon, minpts);
            }
        }
    }
}
