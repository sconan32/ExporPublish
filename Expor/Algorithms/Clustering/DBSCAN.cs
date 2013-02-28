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
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Algorithms.Clustering
{

    [Title("DBSCAN: Density-Based Clustering of Applications with Noise")]
    [Description("Algorithm to find density-connected sets in a database based on the parameters 'minpts' and 'epsilon' (specifying a volume). " + "These two parameters determine a density threshold for clustering.")]
    [Reference(Authors = "M. Ester, H.-P. Kriegel, J. Sander, and X. Xu",
        Title = "A Density-Based Algorithm for Discovering Clusters in Large Spatial Databases with Noise",
        BookTitle = "Proc. 2nd Int. Conf. on Knowledge Discovery and Data Mining (KDD '96), Portland, OR, 1996",
        Url = "http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.71.1980")]
    public class DBSCAN : AbstractDistanceBasedAlgorithm<INumberVector>, IClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(DBSCAN));

        /**
         * Parameter to specify the maximum radius of the neighborhood to be
         * considered, must be suitable to the distance function specified.
         */
        public static OptionDescription EPSILON_ID =
            OptionDescription.GetOrCreate("dbscan.epsilon", "The maximum radius of the neighborhood to be considered.");

        /**
         * Holds the value of {@link #EPSILON_ID}.
         */
        private IDistanceValue epsilon;

        /**
         * Parameter to specify the threshold for minimum number of points in the
         * epsilon-neighborhood of a point, must be an integer greater than 0.
         */
        public static OptionDescription MINPTS_ID =
            OptionDescription.GetOrCreate("dbscan.minpts", "Threshold for minimum number of points in the epsilon-neighborhood of a point.");

        /**
         * Holds the value of {@link #MINPTS_ID}.
         */
        protected int minpts;

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
         * Constructor with parameters.
         * 
         * @param distanceFunction Distance function
         * @param epsilon Epsilon value
         * @param minpts Minpts parameter
         */
        public DBSCAN(IDistanceFunction distanceFunction, IDistanceValue epsilon, int minpts) :
            base(distanceFunction)
        {
            this.epsilon = epsilon;
            this.minpts = minpts;
        }

        /**
         * Performs the DBSCAN algorithm on the given database.
         */
        public ClusterList Run(IRelation relation)
        {
            IRangeQuery rangeQuery = QueryUtil.GetRangeQuery(relation, GetDistanceFunction());
            int size = relation.Count;

            //FiniteProgress objprog = logger.IsVerbose ? new FiniteProgress("Processing objects", size, logger) : null;
           // IndefiniteProgress clusprog = logger.IsVerbose ? new IndefiniteProgress("Number of clusters", logger) : null;
            resultList = new List<IModifiableDbIds>();
            noise = DbIdUtil.NewHashSet();
            processedIDs = DbIdUtil.NewHashSet(size);
            if (size >= minpts)
            {
                //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                foreach (var iditer in relation.GetDbIds())
                {
                    if (!processedIDs.Contains(iditer))
                    {
                        expandCluster(relation, rangeQuery, iditer.DbId);
                    }
                    //if (objprog != null && clusprog != null)
                    //{
                    //    objprog.SetProcessed(processedIDs.Count, logger);
                    //    clusprog.SetProcessed(resultList.Count, logger);
                    //}
                    if (processedIDs.Count == size)
                    {
                        break;
                    }
                }
            }
            else
            {
                // for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                foreach (var iditer in relation.GetDbIds())
                {
                    noise.Add(iditer);
                    //if (objprog != null && clusprog != null)
                    //{
                    //    objprog.SetProcessed(noise.Count, logger);
                    //    clusprog.SetProcessed(resultList.Count, logger);
                    //}
                }
            }
            // Finish progress logging
            //if (objprog != null)
            //{
            //    objprog.EnsureCompleted(logger);
            //}
            //if (clusprog != null)
            //{
            //    clusprog.SetCompleted(logger);
            //}

            ClusterList result = new ClusterList("DBSCAN Clustering", "dbscan-clustering");
            foreach (IModifiableDbIds res in resultList)
            {
                Cluster c = new Cluster(res, ClusterModel.CLUSTER);
                result.AddCluster(c);
            }

            Cluster n = new Cluster(noise, true, ClusterModel.CLUSTER);
            result.AddCluster(n);

            return result;
        }

        /**
         * DBSCAN-function expandCluster.
         * <p/>
         * Border-Objects become members of the first possible cluster.
         * 
         * @param relation IDatabase relation to run on
         * @param rangeQuery Range query to use
         * @param startObjectID potential seed of a new potential cluster
         * @param objprog the progress object for logging the current status
         */
        protected void expandCluster(IRelation relation, IRangeQuery rangeQuery,
            IDbId startObjectID)
        {
            IDistanceDbIdList seeds = rangeQuery.GetRangeForDbId(startObjectID, epsilon);

            // startObject is no core-object
            if (seeds.Count < minpts)
            {
                noise.Add(startObjectID);
                processedIDs.Add(startObjectID);
                //if (objprog != null && clusprog != null)
                //{
                //    objprog.SetProcessed(processedIDs.Count, logger);
                //    clusprog.SetProcessed(resultList.Count, logger);
                //}
                return;
            }

            // try to expand the cluster
            IModifiableDbIds currentCluster = DbIdUtil.NewArray();
            foreach (IDistanceDbIdPair seed in(IEnumerable<IDistanceDbIdPair>) seeds)
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
            //var enumseeds = seeds as IEnumerable<IDistanceDbIdPair>;
            seeds.RemoveAt(0);

            while (seeds.Count > 0)
            {
                IDistanceDbIdPair o = seeds[0];
                seeds.RemoveAt(0);
               IDistanceDbIdList neighborhood = rangeQuery.GetRangeForDbId(o, epsilon);

                if (neighborhood.Count >= minpts)
                {
                    foreach (IDistanceDbIdPair neighbor in neighborhood)
                    {
                        bool inNoise = noise.Contains(neighbor);
                        bool unclassified = !processedIDs.Contains(neighbor);
                        if (inNoise || unclassified)
                        {
                            if (unclassified)
                            {
                                seeds.Add(neighbor);
                            }
                            currentCluster.Add(neighbor);
                            processedIDs.Add(neighbor);
                            if (inNoise)
                            {
                                noise.Remove(neighbor);
                            }
                        }
                    }
                }

                if (processedIDs.Count == relation.Count && noise.Count == 0)
                {
                    break;
                }

                //if (objprog != null && clusprog != null)
                //{
                //    objprog.SetProcessed(processedIDs.Count, logger);
                //    int numClusters = currentCluster.Count > minpts ? resultList.Count + 1 : resultList.Count;
                //    clusprog.SetProcessed(numClusters, logger);
                //}
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
            return TypeUtil.Array(GetDistanceFunction().GetInputTypeRestriction());
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
         */
        public new class Parameterizer :
            AbstractDistanceBasedAlgorithm<INumberVector>.Parameterizer
        {
            protected IDistanceValue epsilon = null;

            protected int minpts = 0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DistanceParameter epsilonP = new DistanceParameter(EPSILON_ID, distanceFunction);
                if (config.Grab(epsilonP))
                {
                    epsilon = epsilonP.GetValue();
                }

                IntParameter minptsP = new IntParameter(MINPTS_ID);
                minptsP.AddConstraint(new GreaterConstraint<int>(0));
                if (config.Grab(minptsP))
                {
                    minpts = minptsP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new DBSCAN(distanceFunction, epsilon, minpts);
            }


        }
    }
}
