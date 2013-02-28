using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions.Subspace;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;
using Socona.Expor.Data.Models;

namespace Socona.Expor.Algorithms.Clustering.Subspace
{

    [Title("SUBCLU: Density connected Subspace Clustering")]
    [Description("Algorithm to detect arbitrarily shaped and positioned clusters in subspaces. SUBCLU delivers for each subspace the same clusters DBSCAN would have found, when applied to this subspace seperately.")]
    [Reference(Authors = "K. Kailing, H.-P. Kriegel, P. Kr枚ger",
        Title = "Density connected Subspace Clustering for High Dimensional Data. ",
        BookTitle = "Proc. SIAM Int. Conf. on Data Mining (SDM'04), Lake Buena Vista, FL, 2004")]
    public class SUBCLU : AbstractAlgorithm, ISubspaceClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(SUBCLU));

        /**
         * The distance function to determine the distance between database objects.
         * <p>
         * Default value: {@link SubspaceEuclideanDistanceFunction}
         * </p>
         * <p>
         * Key: {@code -subclu.distancefunction}
         * </p>
         */
        public static OptionDescription DISTANCE_FUNCTION_ID = OptionDescription.GetOrCreate("subclu.distancefunction", "Distance function to determine the distance between database objects.");

        /**
         * Parameter to specify the maximum radius of the neighborhood to be
         * considered, must be suitable to
         * {@link AbstractDimensionsSelectingDoubleDistanceFunction}.
         * <p>
         * Key: {@code -subclu.epsilon}
         * </p>
         */
        public static OptionDescription EPSILON_ID = OptionDescription.GetOrCreate("subclu.epsilon", "The maximum radius of the neighborhood to be considered.");

        /**
         * Parameter to specify the threshold for minimum number of points in the
         * epsilon-neighborhood of a point, must be an integer greater than 0.
         * <p>
         * Key: {@code -subclu.minpts}
         * </p>
         */
        public static OptionDescription MINPTS_ID = OptionDescription.GetOrCreate("subclu.minpts", "Threshold for minimum number of points in the epsilon-neighborhood of a point.");

        /**
         * Holds the instance of the distance function specified by
         * {@link #DISTANCE_FUNCTION_ID}.
         */
        private AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector> distanceFunction;

        /**
         * Holds the value of {@link #EPSILON_ID}.
         */
        private DoubleDistanceValue epsilon;

        /**
         * Holds the value of {@link #MINPTS_ID}.
         */
        private int minpts;

        /**
         * Holds the result;
         */
        private ClusterList result;

        /**
         * Constructor.
         * 
         * @param distanceFunction Distance function
         * @param epsilon Epsilon value
         * @param minpts Minpts value
         */
        public SUBCLU(AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector> distanceFunction, DoubleDistanceValue epsilon, int minpts)
            : base()
        {
            this.distanceFunction = distanceFunction;
            this.epsilon = epsilon;
            this.minpts = minpts;
        }

        /**
         * Performs the SUBCLU algorithm on the given database.
         * 
         * @param relation Relation to process
         * @return Clustering result
         */
        public ClusterList Run(IRelation relation)
        {
            int dimensionality = DatabaseUtil.Dimensionality(relation);

            StepProgress stepprog = logger.IsVerbose ? new StepProgress(dimensionality) : null;

            // Generate all 1-dimensional clusters
            if (stepprog != null)
            {
                stepprog.BeginStep(1, "Generate all 1-dimensional clusters.", logger);
            }

            // mapping of dimensionality to set of subspaces
            Dictionary<int, List<Subspace<INumberVector>>> subspaceMap = new Dictionary<int, List<Subspace<INumberVector>>>();

            // list of 1-dimensional subspaces containing clusters
            List<Subspace<INumberVector>> s_1 = new List<Subspace<INumberVector>>();
            subspaceMap[0] = s_1;

            // mapping of subspaces to list of clusters
            SortedDictionary<Subspace<INumberVector>, List<Cluster>> clusterMap =
                new SortedDictionary<Subspace<INumberVector>, List<Cluster>>(
                    new Subspace<INumberVector>.DimensionComparator());

            for (int d = 0; d < dimensionality; d++)
            {
                Subspace<INumberVector> currentSubspace = new Subspace<INumberVector>(d);
                List<Cluster> clusters = runDBSCAN(relation, null, currentSubspace);

                if (logger.IsDebugging)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append("\n").Append(clusters.Count).Append(" clusters in subspace ").Append(currentSubspace.DimensonsToString()).Append(": \n");
                    foreach (Cluster cluster in clusters)
                    {
                        msg.Append("      " + cluster.Ids + "\n");
                    }
                    logger.Debug(msg.ToString());
                }

                if (clusters.Count > 0)
                {
                    s_1.Add(currentSubspace);
                    clusterMap[currentSubspace] = clusters;
                }
            }

            // Generate (d+1)-dimensional clusters from d-dimensional clusters
            for (int d = 0; d < dimensionality - 1; d++)
            {
                if (stepprog != null)
                {
                    stepprog.BeginStep(d + 2, "Generate " + (d + 2) + "-dimensional clusters from " + (d + 1) + "-dimensional clusters.", logger);
                }

                List<Subspace<INumberVector>> subspaces = subspaceMap[(d)];
                if (subspaces == null || subspaces.Count <= 0)
                {
                    if (stepprog != null)
                    {
                        for (int dim = d + 1; dim < dimensionality - 1; dim++)
                        {
                            stepprog.BeginStep(dim + 2, "Generation of" + (dim + 2) + "-dimensional clusters not applicable, because no more " + (d + 2) + "-dimensional subspaces found.", logger);
                        }
                    }
                    break;
                }

                List<Subspace<INumberVector>> candidates = generateSubspaceCandidates(subspaces);
                List<Subspace<INumberVector>> s_d = new List<Subspace<INumberVector>>();

                foreach (Subspace<INumberVector> candidate in candidates)
                {
                    Subspace<INumberVector> bestSubspace = BestSubspace(subspaces, candidate, clusterMap);
                    if (logger.IsDebugging)
                    {
                        logger.Debug("best subspace of " + candidate.DimensonsToString() + ": " + bestSubspace.DimensonsToString());
                    }

                    List<Cluster> bestSubspaceClusters = clusterMap[(bestSubspace)];
                    List<Cluster> clusters = new List<Cluster>();
                    foreach (Cluster cluster in bestSubspaceClusters)
                    {
                        List<Cluster> candidateClusters = runDBSCAN(relation, cluster.Ids, candidate);
                        if (candidateClusters.Count > 0)
                        {
                            clusters.AddRange(candidateClusters);
                        }
                    }

                    if (logger.IsDebugging)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append(clusters.Count + " cluster(s) in subspace " + candidate + ": \n");
                        foreach (Cluster c in clusters)
                        {
                            msg.Append("      " + c.Ids + "\n");
                        }
                        logger.Debug(msg.ToString());
                    }

                    if (clusters.Count > 0)
                    {
                        s_d.Add(candidate);
                        clusterMap[candidate] = clusters;
                    }
                }

                if (s_d.Count > 0)
                {
                    subspaceMap[d + 1] = s_d;
                }
            }

            // build result
            int numClusters = 1;
            result = new ClusterList("SUBCLU clustering", "subclu-clustering");
            foreach (Subspace<INumberVector> subspace in clusterMap.Keys.Reverse())
            {
                List<Cluster> clusters = clusterMap[(subspace)];
                foreach (Cluster cluster in clusters)
                {
                    Cluster newCluster = new Cluster(cluster.Ids);
                    newCluster.Model = (new SubspaceModel<INumberVector>(subspace,
                        DatabaseUtil.Centroid<INumberVector>(relation, cluster.Ids)));
                    newCluster.Name = ("cluster_" + numClusters++);
                    result.AddCluster(newCluster);
                }
            }

            if (stepprog != null)
            {
                stepprog.SetCompleted(logger);
            }
            return result;
        }

        /**
         * Returns the result of the algorithm.
         * 
         * @return the result of the algorithm
         */
        public ClusterList GetResult()
        {
            return result;
        }

        /**
         * Runs the DBSCAN algorithm on the specified partition of the database in the
         * given subspace. If parameter {@code ids} is null DBSCAN will be applied to
         * the whole database.
         * 
         * @param relation the database holding the objects to Run DBSCAN on
         * @param ids the IDs of the database defining the partition to Run DBSCAN on
         *        - if this parameter is null DBSCAN will be applied to the whole
         *        database
         * @param subspace the subspace to Run DBSCAN on
         * @return the clustering result of the DBSCAN Run
         */
        private List<Cluster> runDBSCAN(IRelation relation, IDbIds ids, Subspace<INumberVector> subspace)
        {
            // distance function
            distanceFunction.SelectedDimensions = (subspace.GetDimensions());

            ProxyDatabase proxy;
            if (ids == null)
            {
                // TODO: in this case, we might want to use an index - the proxy below
                // will prevent this!
                ids = relation.GetDbIds();
            }

            proxy = new ProxyDatabase(ids, relation);

            DBSCAN dbscan = new DBSCAN(distanceFunction, epsilon, minpts);
            // Run DBSCAN
            if (logger.IsVerbose)
            {
                logger.Verbose("\nRun DBSCAN on subspace " + subspace.DimensonsToString());
            }
            ClusterList dbsres = (ClusterList)dbscan.Run(proxy);

            // separate cluster and noise
            List<Cluster> clusterAndNoise = dbsres.GetAllClusters();
            List<Cluster> clusters = new List<Cluster>();
            foreach (Cluster c in clusterAndNoise)
            {
                if (!c.IsNoise())
                {
                    clusters.Add(c);
                }
            }
            return clusters;
        }

        /**
         * Generates {@code d+1}-dimensional subspace candidates from the specified
         * {@code d}-dimensional subspaces.
         * 
         * @param subspaces the {@code d}-dimensional subspaces
         * @return the {@code d+1}-dimensional subspace candidates
         */
        private List<Subspace<INumberVector>> generateSubspaceCandidates(List<Subspace<INumberVector>> subspaces)
        {
            List<Subspace<INumberVector>> candidates = new List<Subspace<INumberVector>>();

            if (subspaces.Count <= 0)
            {
                return candidates;
            }

            // Generate (d+1)-dimensional candidate subspaces
            int d = subspaces[(0)].Count;

            StringBuilder msgFine = new StringBuilder("\n");
            if (logger.IsDebugging)
            {
                msgFine.Append("subspaces ").Append(subspaces).Append("\n");
            }

            for (int i = 0; i < subspaces.Count; i++)
            {
                Subspace<INumberVector> s1 = subspaces[(i)];
                for (int j = i + 1; j < subspaces.Count; j++)
                {
                    Subspace<INumberVector> s2 = subspaces[(j)];
                    Subspace<INumberVector> candidate = s1.JoinWith(s2);

                    if (candidate != null)
                    {
                        if (logger.IsDebugging)
                        {
                            msgFine.Append("candidate: ").Append(candidate.DimensonsToString()).Append("\n");
                        }
                        // prune irrelevant candidate subspaces
                        List<Subspace<INumberVector>> lowerSubspaces = LowerSubspaces(candidate);
                        if (logger.IsDebugging)
                        {
                            msgFine.Append("lowerSubspaces: ").Append(lowerSubspaces).Append("\n");
                        }
                        bool irrelevantCandidate = false;
                        foreach (Subspace<INumberVector> s in lowerSubspaces)
                        {
                            if (!subspaces.Contains(s))
                            {
                                irrelevantCandidate = true;
                                break;
                            }
                        }
                        if (!irrelevantCandidate)
                        {
                            candidates.Add(candidate);
                        }
                    }
                }
            }

            if (logger.IsDebugging)
            {
                logger.Debug(msgFine.ToString());
            }
            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(d + 1).Append("-dimensional candidate subspaces: ");
                foreach (Subspace<INumberVector> candidate in candidates)
                {
                    msg.Append(candidate.DimensonsToString()).Append(" ");
                }
                logger.Debug(msg.ToString());
            }

            return candidates;
        }

        /**
         * Returns the list of all {@code (d-1)}-dimensional subspaces of the
         * specified {@code d}-dimensional subspace.
         * 
         * @param subspace the {@code d}-dimensional subspace
         * @return a list of all {@code (d-1)}-dimensional subspaces
         */
        private List<Subspace<INumberVector>> LowerSubspaces(Subspace<INumberVector> subspace)
        {
            int dimensionality = subspace.Count;
            if (dimensionality <= 1)
            {
                return null;
            }

            // order result according to the dimensions
            List<Subspace<INumberVector>> result = new List<Subspace<INumberVector>>();
            BitArray dimensions = subspace.GetDimensions();
            for (int dim = dimensions.NextSetBitIndex(0); dim >= 0; dim = dimensions.NextSetBitIndex(dim + 1))
            {
                BitArray newDimensions = (BitArray)dimensions.Clone();
                newDimensions.Set(dim, false);
                result.Add(new Subspace<INumberVector>(newDimensions));
            }

            return result;
        }

        /**
         * Determines the {@code d}-dimensional subspace of the {@code (d+1)}
         * -dimensional candidate with minimal number of objects in the cluster.
         * 
         * @param subspaces the list of {@code d}-dimensional subspaces containing
         *        clusters
         * @param candidate the {@code (d+1)}-dimensional candidate subspace
         * @param clusterMap the mapping of subspaces to clusters
         * @return the {@code d}-dimensional subspace of the {@code (d+1)}
         *         -dimensional candidate with minimal number of objects in the
         *         cluster
         */
        private Subspace<INumberVector> BestSubspace(List<Subspace<INumberVector>> subspaces,
            Subspace<INumberVector> candidate, SortedDictionary<Subspace<INumberVector>, List<Cluster>> clusterMap)
        {
            Subspace<INumberVector> bestSubspace = null;

            foreach (Subspace<INumberVector> subspace in subspaces)
            {
                int min = int.MaxValue;

                if (subspace.IsSubspace(candidate))
                {
                    List<Cluster> clusters = clusterMap[(subspace)];
                    foreach (Cluster cluster in clusters)
                    {
                        int clusterSize = cluster.Count;
                        if (clusterSize < min)
                        {
                            min = clusterSize;
                            bestSubspace = subspace;
                        }
                    }
                }
            }

            return bestSubspace;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.NUMBER_VECTOR_FIELD);
        }


        protected override Logging GetLogger()
        {
            return logger;
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
            protected int minpts = 0;

            protected DoubleDistanceValue epsilon = null;

            protected AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector> distance = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>> param =
                    new ObjectParameter<AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>>(
                        DISTANCE_FUNCTION_ID, typeof(AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>),
                        typeof(SubspaceEuclideanDistanceFunction));
                if (config.Grab(param))
                {
                    distance = param.InstantiateClass(config);
                }

                DistanceParameter epsilonP = new DistanceParameter(EPSILON_ID, distance);
                if (config.Grab(epsilonP))
                {
                    epsilon = (DoubleDistanceValue)epsilonP.GetValue();
                }

                IntParameter minptsP = new IntParameter(MINPTS_ID, new GreaterConstraint<int>(0));
                if (config.Grab(minptsP))
                {
                    minpts = minptsP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new SUBCLU(distance, epsilon, minpts);
            }
        }
    }
}
