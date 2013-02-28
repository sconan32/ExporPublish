using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;
using Wintellect.PowerCollections;

namespace Socona.Expor.Algorithms.Clustering.Subspace
{

    [Title("PROCLUS: PROjected CLUStering")]
    [Description("Algorithm to find subspace clusters in high dimensional spaces.")]
    [Reference(Authors = "C. C. Aggarwal, C. Procopiuc, J. L. Wolf, P. S. Yu, J. S. Park",
        Title = "Fast Algorithms for Projected Clustering",
        BookTitle = "Proc. ACM SIGMOD Int. Conf. on Management of Data (SIGMOD '99)",
        Url = "http://dx.doi.org/10.1145/304181.304188")]
    public class PROCLUS : AbstractProjectedClustering, ISubspaceClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(PROCLUS));

        /**
         * Parameter to specify the multiplier for the initial number of medoids, must
         * be an integer greater than 0.
         * <p>
         * Default value: {@code 10}
         * </p>
         * <p>
         * Key: {@code -proclus.mi}
         * </p>
         */
        public static OptionDescription M_I_ID = OptionDescription.GetOrCreate("proclus.mi", "The multiplier for the initial number of medoids.");

        /**
         * Parameter to specify the random generator seed.
         */
        public static OptionDescription SEED_ID = OptionDescription.GetOrCreate("proclus.seed", "The random number generator seed.");

        /**
         * Holds the value of {@link #M_I_ID}.
         */
        private int m_i;

        /**
         * Holds the value of {@link #SEED_ID}.
         */
        private long? seed;

        /**
         * Java constructor.
         * 
         * @param k k Parameter
         * @param k_i k_i Parameter
         * @param l l Parameter
         * @param m_i m_i Parameter
         * @param seed Random generator seed
         */
        public PROCLUS(int k, int k_i, int l, int m_i, long? seed) :
            base(k, k_i, l)
        {
            this.m_i = m_i;
            this.seed = seed;
        }

        /**
         * Performs the PROCLUS algorithm on the given database.
         * 
         * @param database IDatabase to process
         * @param relation Relation to process
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            IDistanceQuery distFunc = this.GetDistanceQuery(database);
            IRangeQuery rangeQuery = database.GetRangeQuery(distFunc);

            Random random = new Random();
            if (seed != null)
            {
                random = new Random((int)seed.GetValueOrDefault());
            }

            if (DatabaseUtil.Dimensionality(relation) < l)
            {
                throw new Exception("Dimensionality of data < parameter l! " + "(" + DatabaseUtil.Dimensionality(relation) + " < " + l + ")");
            }

            // TODO: use a StepProgress!
            // initialization phase
            if (logger.IsVerbose)
            {
                logger.Verbose("1. Initialization phase...");
            }
            int sampleSize = Math.Min(relation.Count, k_i * k);
            IDbIds sampleSet = DbIdUtil.RandomSample(relation.GetDbIds(), sampleSize, random.Next());

            int medoidSize = Math.Min(relation.Count, m_i * k);
            IDbIds medoids = Greedy(distFunc, sampleSet, medoidSize, random);

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("\n");
                msg.Append("sampleSize ").Append(sampleSize).Append("\n");
                msg.Append("sampleSet ").Append(sampleSet).Append("\n");
                msg.Append("medoidSize ").Append(medoidSize).Append("\n");
                msg.Append("m ").Append(medoids).Append("\n");
                logger.Debug(msg.ToString());
            }

            // iterative phase
            if (logger.IsVerbose)
            {
                logger.Verbose("2. Iterative phase...");
            }
            double bestObjective = Double.PositiveInfinity;
            IModifiableDbIds m_best = null;
            IModifiableDbIds m_bad = null;
            IModifiableDbIds m_current = InitialSet(medoids, k, random);

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("\n");
                msg.Append("m_c ").Append(m_current).Append("\n");
                logger.Debug(msg.ToString());
            }

            // IndefiniteProgress cprogress = logger.IsVerbose ? new IndefiniteProgress("Current number of clusters:", logger) : null;

            // TODO: Use DataStore and Trove for performance
            IDictionary<IDbId, PROCLUSCluster> clusters = null;
            int loops = 0;
            while (loops < 10)
            {
                IDictionary<IDbId, ISet<int>> dimensions = FindDimensions(m_current, relation, distFunc, rangeQuery);
                clusters = AssignPoints(dimensions, relation);
                double objectiveFunction = EvaluateClusters(clusters, dimensions, relation);

                if (objectiveFunction < bestObjective)
                {
                    // restart counting loops
                    loops = 0;
                    bestObjective = objectiveFunction;
                    m_best = m_current;
                    m_bad = ComputeBadMedoids(clusters, (int)(relation.Count * 0.1 / k));
                }

                m_current = ComputeM_current(medoids, m_best, m_bad, random);
                loops++;
                //if(cprogress != null) {
                //  cprogress.setProcessed(clusters.Count, logger);
                //}
            }

            //if(cprogress != null) {
            //  cprogress.setCompleted(logger);
            //}

            // refinement phase
            if (logger.IsVerbose)
            {
                logger.Verbose("3. Refinement phase...");
            }

            IList<IPair<INumberVector, ISet<int>>> dimensions1 = FindDimensions(new List<PROCLUSCluster>(clusters.Values), relation);
            IList<PROCLUSCluster> Clusters = FinalAssignment(dimensions1, relation);

            // build result
            int numClusters = 1;
            ClusterList result = new ClusterList("ProClus clustering", "proclus-clustering");
            foreach (PROCLUSCluster c in Clusters)
            {
                Cluster cluster = new Cluster(c.objectIDs);
                cluster.Model = (new SubspaceModel<INumberVector>(new Subspace<INumberVector>(c.GetDimensions()), c.centroid));
                cluster.Name = ("cluster_" + numClusters++);

                result.AddCluster(cluster);
            }
            return result;
        }

        /**
         * Returns a piercing set of k medoids from the specified sample set.
         * 
         * @param distFunc the distance function
         * @param sampleSet the sample set
         * @param m the number of medoids to be returned
         * @param random random number generator
         * @return a piercing set of m medoids from the specified sample set
         */
        private IModifiableDbIds Greedy(IDistanceQuery distFunc, IDbIds sampleSet, int m, Random random)
        {
            IArrayModifiableDbIds s = DbIdUtil.NewArray(sampleSet);
            IModifiableDbIds medoids = DbIdUtil.NewHashSet();

            // m_1 is random point of S
            IDbId m_i = s.RemoveAt(random.Next(s.Count));
            medoids.Add(m_i);
            if (logger.IsDebugging)
            {
                logger.Debug("medoids " + medoids);
            }

            // compute distances between each point in S and m_i
            IDictionary<IDbId, IDistanceResultPair> distances = new Dictionary<IDbId, IDistanceResultPair>();

            //for(DbIdIter iter = s.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in s)
            {
                IDbId id = iter.DbId;
                DoubleDistanceValue dist = (DoubleDistanceValue)distFunc.Distance(id, m_i);
                distances[id] = new GenericDistanceResultPair(dist, id);
            }

            for (int i = 1; i < m; i++)
            {
                // choose medoid m_i to be far from prevois medoids
                List<IDistanceResultPair> d = new List<IDistanceResultPair>(distances.Values);
                d.Sort();

                m_i = d[d.Count - 1].DbId;
                medoids.Add(m_i);
                s.Remove(m_i);
                distances.Remove(m_i);

                // compute distances of each point to closest medoid
                // for(DbIdIter iter = s.iter(); iter.valid(); iter.advance()) {
                foreach (var iter in s)
                {
                    IDbId id = iter.DbId;
                    DoubleDistanceValue dist_new = (DoubleDistanceValue)distFunc.Distance(id, m_i);
                    DoubleDistanceValue dist_old = (DoubleDistanceValue)distances[(id)].GetDistance();

                    DoubleDistanceValue dist = dist_new.CompareTo(dist_old) < 0 ? dist_new : dist_old;
                    distances[id] = new GenericDistanceResultPair(dist, id);
                }

                if (logger.IsDebugging)
                {
                    logger.Debug("medoids " + medoids);
                }
            }

            return medoids;
        }

        /**
         * Returns a set of k elements from the specified sample set.
         * 
         * @param sampleSet the sample set
         * @param k the number of samples to be returned
         * @param random random number generator
         * @return a set of k elements from the specified sample set
         */
        private IModifiableDbIds InitialSet(IDbIds sampleSet, int k, Random random)
        {
            IArrayModifiableDbIds s = DbIdUtil.NewArray(sampleSet);
            IModifiableDbIds initialSet = DbIdUtil.NewHashSet();
            while (initialSet.Count < k)
            {
                IDbId next = s.RemoveAt(random.Next(s.Count));
                initialSet.Add(next);
            }
            return initialSet;
        }

        /**
         * Computes the set of medoids in current iteration.
         * 
         * @param m the medoids
         * @param m_best the best set of medoids found so far
         * @param m_bad the bad medoids
         * @param random random number generator
         * @return m_current, the set of medoids in current iteration
         */
        private IModifiableDbIds ComputeM_current(IDbIds m, IDbIds m_best, IDbIds m_bad, Random random)
        {
            IArrayModifiableDbIds m_list = DbIdUtil.NewArray(m);
            m_list.RemoveDbIds(m_best);

            IModifiableDbIds m_current = DbIdUtil.NewHashSet();
            //for(DbIdIter iter = m_best.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in m_best)
            {
                IDbId m_i = iter.DbId;
                if (m_bad.Contains(m_i))
                {
                    int currentSize = m_current.Count;
                    while (m_current.Count == currentSize)
                    {
                        IDbId next = m_list.RemoveAt(random.Next(m_list.Count));
                        m_current.Add(next);
                    }
                }
                else
                {
                    m_current.Add(m_i);
                }
            }

            return m_current;
        }

        /**
         * Computes the localities of the specified medoids: for each medoid m the
         * objects in the sphere centered at m with radius minDist are determined,
         * where minDist is the minimum distance between medoid m and any other medoid
         * m_i.
         * 
         * @param medoids the ids of the medoids
         * @param database the database holding the objects
         * @param distFunc the distance function
         * @return a mapping of the medoid's id to its locality
         */
        private IDictionary<IDbId, IDistanceDbIdList> GetLocalities(IDbIds medoids,
            IRelation database, IDistanceQuery distFunc, IRangeQuery rangeQuery)
        {
            IDictionary<IDbId, IDistanceDbIdList> result = new Dictionary<IDbId, IDistanceDbIdList>();

            //for(DbIdIter iter = medoids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in medoids)
            {
                IDbId m = iter.DbId;
                // determine minimum distance between current medoid m and any other
                // medoid m_i
                DoubleDistanceValue minDist = null;
                //for(DbIdIter iter2 = medoids.iter(); iter2.valid(); iter2.advance()) {
                foreach (var iter2 in medoids)
                {
                    IDbId m_i = iter2.DbId;
                    if (m_i.Equals(m))
                    {
                        continue;
                    }
                    DoubleDistanceValue currentDist = (DoubleDistanceValue)distFunc.Distance(m, m_i);
                    if (minDist == null || currentDist.CompareTo(minDist) < 0)
                    {
                        minDist = currentDist;
                    }
                }

                // determine points in sphere centered at m with radius minDist
                Debug.Assert(minDist != null);
                IDistanceDbIdList qr = rangeQuery.GetRangeForDbId(m, minDist);
                result[m] = qr;
            }

            return result;
        }

        /**
         * Determines the set of correlated dimensions for each medoid in the
         * specified medoid set.
         * 
         * @param medoids the set of medoids
         * @param database the database containing the objects
         * @param distFunc the distance function
         * @return the set of correlated dimensions for each medoid in the specified
         *         medoid set
         */
        private IDictionary<IDbId, ISet<int>> FindDimensions(IDbIds medoids,
            IRelation database, IDistanceQuery distFunc, IRangeQuery rangeQuery)
        {
            // Get localities
            IDictionary<IDbId, IDistanceDbIdList> localities = GetLocalities(medoids, database, distFunc, rangeQuery);

            // compute x_ij = avg distance from points in l_i to medoid m_i
            int dim = DatabaseUtil.Dimensionality(database);
            IDictionary<IDbId, double[]> averageDistances = new Dictionary<IDbId, double[]>();

            //for(DbIdIter iter = medoids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in medoids)
            {
                IDbId m_i = iter.DbId;
                INumberVector medoid_i = (INumberVector)database[(m_i)];
                IDistanceDbIdList l_i = localities[(m_i)];
                double[] x_i = new double[dim];
                foreach (IDistanceDbIdPair qr in (IEnumerable<IDistanceDbIdPair>)l_i)
                {
                    INumberVector o = (INumberVector)database[(qr.DbId)];
                    for (int d = 0; d < dim; d++)
                    {
                        x_i[d] += Math.Abs(medoid_i[d] - o[d]);
                    }
                }
                for (int d = 0; d < dim; d++)
                {
                    x_i[d] /= l_i.Count;
                }
                averageDistances[m_i] = x_i;
            }

            IDictionary<IDbId, ISet<int>> dimensionMap = new Dictionary<IDbId, ISet<int>>();
            List<Triple<Double, IDbId, int>> z_ijs = new List<Triple<Double, IDbId, int>>();
            // for(DbIdIter iter = medoids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in medoids)
            {
                IDbId m_i = iter.DbId;
                ISet<int> dims_i = new HashSet<int>();
                dimensionMap[m_i] = dims_i;

                double[] x_i = averageDistances[(m_i)];
                // y_i
                double y_i = 0;
                for (int j = 0; j < dim; j++)
                {
                    y_i += x_i[j];
                }
                y_i /= dim;

                // sigma_i
                double sigma_i = 0;
                for (int j = 0; j < dim; j++)
                {
                    double diff = x_i[j] - y_i;
                    sigma_i += diff * diff;
                }
                sigma_i /= (dim - 1);
                sigma_i = Math.Sqrt(sigma_i);

                for (int j = 0; j < dim; j++)
                {
                    z_ijs.Add(new Triple<Double, IDbId, int>((x_i[j] - y_i) / sigma_i, m_i, j));
                }
            }
            z_ijs.Sort();

            int max = Math.Max(k * l, 2);
            for (int m = 0; m < max; m++)
            {
                Triple<Double, IDbId, int> z_ij = z_ijs[(m)];
                ISet<int> dims_i = dimensionMap[(z_ij.Second)];
                dims_i.Add(z_ij.Third);

                if (logger.IsDebugging)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append("\n");
                    msg.Append("z_ij ").Append(z_ij).Append("\n");
                    msg.Append("D_i ").Append(FormatUtil.Format(dims_i)).Append("\n");
                    logger.Debug(msg.ToString());
                }
            }
            return dimensionMap;
        }

        /**
         * Refinement step that determines the set of correlated dimensions for each
         * cluster centroid.
         * 
         * @param clusters the list of clusters
         * @param database the database containing the objects
         * @return the set of correlated dimensions for each specified cluster
         *         centroid
         */
        private IList<IPair<INumberVector, ISet<int>>> FindDimensions(IList<PROCLUSCluster> clusters, IRelation database)
        {
            // compute x_ij = avg distance from points in c_i to c_i.centroid
            int dim = DatabaseUtil.Dimensionality(database);
            IDictionary<int, double[]> averageDistances = new Dictionary<int, double[]>();

            for (int i = 0; i < clusters.Count; i++)
            {
                PROCLUSCluster c_i = clusters[i];
                double[] x_i = new double[dim];
                //for(DbIdIter iter = c_i.objectIDs.iter(); iter.valid(); iter.advance()) {
                foreach (var iter in c_i.objectIDs)
                {
                    INumberVector o = (INumberVector)database[(iter)];
                    for (int d = 0; d < dim; d++)
                    {
                        x_i[d] += Math.Abs(c_i.centroid[d] - o[d]);
                    }
                }
                for (int d = 0; d < dim; d++)
                {
                    x_i[d] /= c_i.objectIDs.Count;
                }
                averageDistances[i] = x_i;
            }

            List<Triple<Double, int, int>> z_ijs = new List<Triple<Double, int, int>>();
            for (int i = 0; i < clusters.Count; i++)
            {
                double[] x_i = averageDistances[(i)];
                // y_i
                double y_i = 0;
                for (int j = 0; j < dim; j++)
                {
                    y_i += x_i[j];
                }
                y_i /= dim;

                // sigma_i
                double sigma_i = 0;
                for (int j = 0; j < dim; j++)
                {
                    double diff = x_i[j] - y_i;
                    sigma_i += diff * diff;
                }
                sigma_i /= (dim - 1);
                sigma_i = Math.Sqrt(sigma_i);

                for (int j = 0; j < dim; j++)
                {
                    z_ijs.Add(new Triple<Double, int, int>((x_i[j] - y_i) / sigma_i, i, j));
                }
            }
            z_ijs.Sort();

            // mapping cluster index -> dimensions
            IDictionary<int, ISet<int>> dimensionMap = new Dictionary<int, ISet<int>>();
            int max = Math.Max(k * l, 2);
            for (int m = 0; m < max; m++)
            {
                Triple<Double, int, int> z_ij = z_ijs[(m)];
                ISet<int> dims_i = null;
                dimensionMap.TryGetValue(z_ij.Second, out dims_i);
                if (dims_i == null)
                {
                    dims_i = new HashSet<int>();
                    dimensionMap[z_ij.Second] = dims_i;
                }
                dims_i.Add(z_ij.Third);

                if (logger.IsDebugging)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append("\n");
                    msg.Append("z_ij ").Append(z_ij).Append("\n");
                    msg.Append("D_i ").Append(dims_i).Append("\n");
                    logger.Debug(msg.ToString());
                }
            }

            // mapping cluster -> dimensions
            List<IPair<INumberVector, ISet<int>>> result = new List<IPair<INumberVector, ISet<int>>>();
            foreach (int i in dimensionMap.Keys)
            {
                ISet<int> dims_i = dimensionMap[i];
                PROCLUSCluster c_i = clusters[i];
                result.Add(new Socona.Expor.Utilities.Pairs.Pair<INumberVector, ISet<int>>(c_i.centroid, dims_i));
            }
            return result;
        }

        /**
         * Assigns the objects to the clusters.
         * 
         * @param dimensions set of correlated dimensions for each medoid of the
         *        cluster
         * @param database the database containing the objects
         * @return the assignments of the object to the clusters
         */
        private IDictionary<IDbId, PROCLUSCluster> AssignPoints(IDictionary<IDbId, ISet<int>> dimensions, IRelation database)
        {
            IDictionary<IDbId, IModifiableDbIds> clusterIDs = new Dictionary<IDbId, IModifiableDbIds>();
            foreach (IDbId m_i in dimensions.Keys)
            {
                clusterIDs[m_i] = DbIdUtil.NewHashSet();
            }

            //for(DbIdIter it = database.iterDbIds(); it.valid(); it.advance()) {
            foreach (var it in database.GetDbIds())
            {
                IDbId p_id = it.DbId;
                INumberVector p = (INumberVector)database[(p_id)];
                IDistanceResultPair minDist = null;
                foreach (IDbId m_i in dimensions.Keys)
                {
                    INumberVector m = (INumberVector)database[m_i];
                    IDistanceResultPair currentDist = new GenericDistanceResultPair(ManhattanSegmentalDistance(p, m, dimensions[m_i]), m_i);
                    if (minDist == null || currentDist.CompareTo(minDist) < 0)
                    {
                        minDist = currentDist;
                    }
                }
                // Add p to cluster with mindist
                Debug.Assert(minDist != null);
                IModifiableDbIds ids = clusterIDs[minDist.DbId];
                ids.Add(p_id);
            }

            IDictionary<IDbId, PROCLUSCluster> clusters = new Dictionary<IDbId, PROCLUSCluster>();
            foreach (IDbId m_i in dimensions.Keys)
            {
                IModifiableDbIds objectIDs = clusterIDs[(m_i)];
                if (objectIDs.Count > 0)
                {
                    ISet<int> clusterDimensions = dimensions[(m_i)];
                    INumberVector centroid = DatabaseUtil.Centroid<INumberVector>(database, objectIDs);
                    clusters[m_i] = new PROCLUSCluster(objectIDs, clusterDimensions, centroid);
                }
            }

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("\n");
                msg.Append("clusters ").Append(FormatUtil.Format(clusters)).Append("\n");
                logger.Debug(msg.ToString());
            }
            return clusters;
        }

        /**
         * Refinement step to assign the objects to the  clusters.
         * 
         * @param dimensions pair containing the centroid and the set of correlated
         *        dimensions for the centroid
         * @param database the database containing the objects
         * @return the assignments of the object to the clusters
         */
        private List<PROCLUSCluster> FinalAssignment(IList<IPair<INumberVector, ISet<int>>> dimensions, IRelation database)
        {
            IDictionary<int, IModifiableDbIds> clusterIDs = new Dictionary<int, IModifiableDbIds>();
            for (int i = 0; i < dimensions.Count; i++)
            {
                clusterIDs[i] = DbIdUtil.NewHashSet();
            }

            //for(DbIdIter it = database.iterDbIds(); it.valid(); it.advance()) {
            foreach (var it in database.GetDbIds())
            {
                IDbId p_id = it.DbId;
                INumberVector p = (INumberVector)database[(p_id)];
                //TODO:这里可能会报错。
                IPair<DoubleDistanceValue, int> minDist = null;
                for (int i = 0; i < dimensions.Count; i++)
                {
                    IPair<INumberVector, ISet<int>> pair_i = dimensions[i];
                    INumberVector c_i = pair_i.First;
                    ISet<int> dimensions_i = pair_i.Second;
                    DoubleDistanceValue currentDist = ManhattanSegmentalDistance(p, c_i, dimensions_i);
                    if (minDist == null || currentDist.CompareTo(minDist.First) < 0)
                    {
                        minDist = new Socona.Expor.Utilities.Pairs.Pair<DoubleDistanceValue, int>(currentDist, i);
                    }
                }
                // Add p to cluster with mindist
                Debug.Assert(minDist != null);
                Debug.Assert(minDist.First.ToDouble() != double.NaN);
                IModifiableDbIds ids = clusterIDs[minDist.Second];
                ids.Add(p_id);
            }

            List<PROCLUSCluster> clusters = new List<PROCLUSCluster>();
            for (int i = 0; i < dimensions.Count; i++)
            {
                IModifiableDbIds objectIDs = clusterIDs[(i)];
                if (objectIDs.Count > 0)
                {
                    ISet<int> clusterDimensions = dimensions[(i)].Second;
                    INumberVector centroid = DatabaseUtil.Centroid<INumberVector>(database, objectIDs);
                    clusters.Add(new PROCLUSCluster(objectIDs, clusterDimensions, centroid));
                }
            }

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("\n");
                msg.Append("clusters ").Append(clusters).Append("\n");
                logger.Debug(msg.ToString());
            }
            return clusters;
        }

        /**
         * Returns the Manhattan segmental distance between o1 and o2 relative to the
         * specified dimensions.
         * 
         * @param o1 the first object
         * @param o2 the second object
         * @param dimensions the dimensions to be considered
         * @return the Manhattan segmental distance between o1 and o2 relative to the
         *         specified dimensions
         */
        private DoubleDistanceValue ManhattanSegmentalDistance(INumberVector o1, INumberVector o2, ISet<int> dimensions)
        {
            double result = 0;
            foreach (int d in dimensions)
            {
                result += Math.Abs(o1[(d)] - o2[d]);
            }
            result /= dimensions.Count;
            return new DoubleDistanceValue(result);
        }

        /**
         * Evaluates the quality of the clusters.
         * 
         * @param clusters the clusters to be evaluated
         * @param dimensions the dimensions associated with each cluster
         * @param database the database holding the objects
         * @return a measure for the cluster quality
         */
        private double EvaluateClusters(IDictionary<IDbId, PROCLUSCluster> clusters, IDictionary<IDbId, ISet<int>> dimensions, IRelation database)
        {
            double result = 0;
            foreach (IDbId m_i in clusters.Keys)
            {
                PROCLUSCluster c_i = clusters[(m_i)];
                INumberVector centroid_i = c_i.centroid;

                ISet<int> dims_i = dimensions[(m_i)];
                double w_i = 0;
                foreach (int j in dims_i)
                {
                    w_i += AvgDistance(centroid_i, c_i.objectIDs, database, j);
                }

                w_i /= dimensions.Keys.Count;
                result += c_i.objectIDs.Count * w_i;
            }

            return result / database.Count;
        }

        /**
         * Computes the average distance of the objects to the centroid along the
         * specified dimension.
         * 
         * @param centroid the centroid
         * @param objectIDs the set of objects ids
         * @param database the database holding the objects
         * @param dimension the dimension for which the average distance is computed
         * @return the average distance of the objects to the centroid along the
         *         specified dimension
         */
        private double AvgDistance(INumberVector centroid, IDbIds objectIDs, IRelation database, int dimension)
        {
            double avg = 0;
            //for (DbIdIter iter = objectIDs.iter(); iter.valid(); iter.advance())
            foreach (var iter in objectIDs)
            {
                INumberVector o = (INumberVector)database[(iter)];
                avg += Math.Abs(centroid[(dimension)] - o[(dimension)]);
            }
            return avg / objectIDs.Count;
        }

        /**
         * Computes the bad medoids, where the medoid of a cluster with less than the
         * specified threshold of objects is bad.
         * 
         * @param clusters the clusters
         * @param threshold the threshold
         * @return the bad medoids
         */
        private IModifiableDbIds ComputeBadMedoids(IDictionary<IDbId, PROCLUSCluster> clusters, int threshold)
        {
            IModifiableDbIds badMedoids = DbIdUtil.NewHashSet();
            foreach (IDbId m_i in clusters.Keys)
            {
                PROCLUSCluster c_i = clusters[(m_i)];
                if (c_i.objectIDs.Count < threshold)
                {
                    badMedoids.Add(m_i);
                }
            }
            return badMedoids;
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
         * Encapsulates the attributes of a cluster.
         * 
         * @apiviz.exclude
         */
        private class PROCLUSCluster
        {
            /**
             * The ids of the objects belonging to this cluster.
             */
            internal IModifiableDbIds objectIDs;

            /**
             * The correlated dimensions of this cluster.
             */
            internal ISet<int> dimensions;

            /**
             * The centroids of this cluster along each dimension.
             */
            internal INumberVector centroid;

            /**
             * Provides a new cluster with the specified parameters.
             * 
             * @param objectIDs the ids of the objects belonging to this cluster
             * @param dimensions the correlated dimensions of this cluster
             * @param centroid the centroid of this cluster
             */
            public PROCLUSCluster(IModifiableDbIds objectIDs, ISet<int> dimensions, INumberVector centroid)
            {
                this.objectIDs = objectIDs;
                this.dimensions = dimensions;
                this.centroid = centroid;
            }


            public override String ToString()
            {
                StringBuilder result = new StringBuilder();
                result.Append("Dimensions: [");
                bool notFirst = false;
                foreach (int d in dimensions)
                {
                    if (notFirst)
                    {
                        result.Append(",");
                    }
                    else
                    {
                        notFirst = true;
                    }
                    result.Append(d);
                }
                result.Append("]");

                result.Append("\nCentroid: ").Append(centroid);
                return result.ToString();
            }

            /**
             * Returns the correlated dimensions of this cluster as BitArray.
             * 
             * @return the correlated dimensions of this cluster as BitArray
             */
            public BitArray GetDimensions()
            {
                BitArray result = new BitArray(1000);
                foreach (int d in dimensions)
                {
                    result.Set(d, true);
                }
                return result;
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractProjectedClustering.Parameterizer
        {
            protected int m_i = -1;

            protected long? seed = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);

                ConfigK(config);
                ConfigKI(config);
                ConfigL(config);

                IntParameter m_iP = new IntParameter(M_I_ID, 10);
                m_iP.AddConstraint(new GreaterConstraint<int>(0));
                if (config.Grab(m_iP))
                {
                    m_i = m_iP.GetValue();
                }

                LongParameter seedP = new LongParameter(SEED_ID, true);
                if (config.Grab(seedP))
                {
                    seed = seedP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new PROCLUS(k, k_i, l, m_i, seed);
            }


        }

    }
}
