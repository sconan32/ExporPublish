using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Tree;
using Socona.Expor.Indexes.Tree.Spatial;
using Socona.Expor.Results;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Algorithms
{

    [Title("K-Nearest Neighbor Join")]
    [Description("Algorithm to find the k-nearest neighbors of each object in a spatial database")]
    public class KNNJoin<V, N, E> : AbstractDistanceBasedAlgorithm<V>
        where V : INumberVector
        where N : ISpatialNode<N, E>
        where E : ISpatialEntry
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(KNNJoin<V, N, E>));

        /**
         * Parameter that specifies the k-nearest neighbors to be assigned, must be an
         * integer greater than 0. Default value: 1.
         */
        public static OptionDescription K_ID = OptionDescription.GetOrCreate("knnjoin.k", "Specifies the k-nearest neighbors to be assigned.");

        /**
         * The k parameter
         */
        int k;

        /**
         * Constructor.
         * 
         * @param distanceFunction Distance function
         * @param k k parameter
         */
        public KNNJoin(IDistanceFunction distanceFunction, int k) :
            base(distanceFunction)
        {
            this.k = k;
        }

        /**
         * Joins in the given spatial database to each object its k-nearest neighbors.
         * 
         * @param database Database to process
         * @param relation Relation to process
         * @return result
         */

        public IWritableDataStore<IKNNList> Run(IDatabase database, IRelation relation)
        {
            if (!(GetDistanceFunction() is ISpatialPrimitiveDistanceFunction))
            {
                throw new InvalidOperationException("Distance Function must be an instance of " +
                    typeof(ISpatialPrimitiveDistanceFunction).Name);
            }
            ICollection<SpatialIndexTree<N, E>> indexes = ResultUtil.FilterResults<SpatialIndexTree<N, E>>
                (database, typeof(SpatialIndexTree<N, E>));
            if (indexes.Count != 1)
            {
                throw new AbortException("KNNJoin found " + indexes.Count + " spatial indexes, expected exactly one.");
            }
            // FIXME: Ensure were looking at the right relation!
            SpatialIndexTree<N, E> index = indexes.ElementAt(0);
            ISpatialPrimitiveDistanceFunction distFunction = (ISpatialPrimitiveDistanceFunction)GetDistanceFunction();
            IDbIds ids = relation.GetDbIds();

            // Optimize for double?
            bool doubleOptimize = (GetDistanceFunction() is ISpatialPrimitiveDoubleDistanceFunction);

            // data pages
            IList<E> ps_candidates = new List<E>(index.GetLeaves());
            // knn heaps
            IList<IList<IKNNHeap>> heaps = new List<IList<IKNNHeap>>(ps_candidates.Count);
            Heap<Task> pq = new Heap<Task>(ps_candidates.Count * ps_candidates.Count / 10);

            // Initialize with the page self-pairing
            for (int i = 0; i < ps_candidates.Count; i++)
            {
                E pr_entry = ps_candidates[(i)];
                N pr = index.GetNode(pr_entry);
                heaps.Add(InitHeaps(distFunction, doubleOptimize, pr));
            }

            // Build priority queue
            int sqsize = ps_candidates.Count * (ps_candidates.Count - 1) / 2;
            if (logger.IsDebugging)
            {
                logger.Debug("Number of leaves: " + ps_candidates.Count + " so " + sqsize + " MBR computations.");
            }
            FiniteProgress mprogress = logger.IsVerbose ? new FiniteProgress("Comparing leaf MBRs", sqsize, logger) : null;
            for (int i = 0; i < ps_candidates.Count; i++)
            {
                E pr_entry = ps_candidates[(i)];
                IList<IKNNHeap> pr_heaps = heaps[i];
                IDistanceValue pr_knn_distance = ComputeStopDistance(pr_heaps);

                for (int j = i + 1; j < ps_candidates.Count; j++)
                {
                    E ps_entry = ps_candidates[j];
                    IList<IKNNHeap> ps_heaps = heaps[j];
                    IDistanceValue ps_knn_distance = ComputeStopDistance(ps_heaps);
                    IDistanceValue minDist = distFunction.MinDistance(pr_entry, ps_entry);
                    // Resolve immediately:
                    if (minDist.IsEmpty)
                    {
                        N pr = index.GetNode(ps_candidates[i]);
                        N ps = index.GetNode(ps_candidates[j]);
                        ProcessDataPagesOptimize(distFunction, doubleOptimize, pr_heaps, ps_heaps, pr, ps);
                    }
                    else if (minDist.CompareTo(pr_knn_distance) <= 0 || minDist.CompareTo(ps_knn_distance) <= 0)
                    {
                        pq.Add(new Task(minDist, i, j));
                    }
                    if (mprogress != null)
                    {
                        mprogress.IncrementProcessed(logger);
                    }
                }
            }
            if (mprogress != null)
            {
                mprogress.EnsureCompleted(logger);
            }

            // Process the queue
            FiniteProgress qprogress = logger.IsVerbose ? new FiniteProgress("Processing queue", pq.Count, logger) : null;
            IndefiniteProgress fprogress = logger.IsVerbose ? new IndefiniteProgress("Full comparisons", logger) : null;
            while (pq.Count > 0)
            {
                Task task = pq.Poll();
                IList<IKNNHeap> pr_heaps = heaps[(task.i)];
                IList<IKNNHeap> ps_heaps = heaps[(task.j)];
                IDistanceValue pr_knn_distance = ComputeStopDistance(pr_heaps);
                IDistanceValue ps_knn_distance = ComputeStopDistance(ps_heaps);
                bool dor = task.mindist.CompareTo(pr_knn_distance) <= 0;
                bool dos = task.mindist.CompareTo(ps_knn_distance) <= 0;
                if (dor || dos)
                {
                    N pr = index.GetNode(ps_candidates[(task.i)]);
                    N ps = index.GetNode(ps_candidates[(task.j)]);
                    if (dor && dos)
                    {
                        ProcessDataPagesOptimize(distFunction, doubleOptimize, pr_heaps, ps_heaps, pr, ps);
                    }
                    else
                    {
                        if (dor)
                        {
                            ProcessDataPagesOptimize(distFunction, doubleOptimize, pr_heaps, null, pr, ps);
                        }
                        else /* dos */
                        {
                            ProcessDataPagesOptimize(distFunction, doubleOptimize, ps_heaps, null, ps, pr);
                        }
                    }
                    if (fprogress != null)
                    {
                        fprogress.IncrementProcessed(logger);
                    }
                }
                if (qprogress != null)
                {
                    qprogress.IncrementProcessed(logger);
                }
            }
            if (qprogress != null)
            {
                qprogress.EnsureCompleted(logger);
            }
            if (fprogress != null)
            {
                fprogress.SetCompleted(logger);
            }

            IWritableDataStore<IKNNList> knnLists = DataStoreUtil.MakeStorage<IKNNList>
                (ids, DataStoreHints.Static, typeof(IKNNList));
            // FiniteProgress progress = logger.IsVerbose ? new
            // FiniteProgress(this.GetClass().GetName(), relation.Count, logger) :
            // null;
            FiniteProgress pageprog = logger.IsVerbose ? new FiniteProgress("Number of processed data pages", ps_candidates.Count, logger) : null;
            // int processed = 0;
            for (int i = 0; i < ps_candidates.Count; i++)
            {
                N pr = index.GetNode(ps_candidates[(i)]);
                IList<IKNNHeap> pr_heaps = heaps[(i)];

                // Finalize lists
                for (int j = 0; j < pr.GetNumEntries(); j++)
                {
                    knnLists[((ILeafEntry)pr.GetEntry(j)).GetDbId()] = pr_heaps[(j)].ToKNNList();
                }
                // ForGet heaps and pq
                heaps[i] = null;
                // processed += pr.GetNumEntries();

                // if(progress != null) {
                // progress.setProcessed(processed, logger);
                // }
                if (pageprog != null)
                {
                    pageprog.IncrementProcessed(logger);
                }
            }
            // if(progress != null) {
            // progress.ensureCompleted(logger);
            // }
            if (pageprog != null)
            {
                pageprog.EnsureCompleted(logger);
            }
            return knnLists;
        }

        private IList<IKNNHeap> InitHeaps(ISpatialPrimitiveDistanceFunction distFunction,
            bool doubleOptimize, N pr)
        {
            IList<IKNNHeap> pr_heaps;
            // Create for each data object a knn heap
            pr_heaps = new List<IKNNHeap>(pr.GetNumEntries());
            for (int j = 0; j < pr.GetNumEntries(); j++)
            {
                pr_heaps.Add(DbIdUtil.NewHeap(distFunction.DistanceFactory.Infinity, k));
            }
            // Self-join first, as this is expected to improve most and cannot be
            // pruned.
            ProcessDataPagesOptimize(distFunction, doubleOptimize, pr_heaps, null, pr, pr);
            return pr_heaps;
        }

        /**
         * Processes the two data pages pr and ps and determines the k-nearest
         * neighbors of pr in ps.
         * 
         * @param distFunction the distance to use
         * @param doubleOptimize Flag whether to optimize for doubles.
         * @param pr the first data page
         * @param ps the second data page
         * @param pr_heaps the knn lists for each data object in pr
         * @param ps_heaps the knn lists for each data object in ps (if ps != pr)
         */
        private void ProcessDataPagesOptimize(ISpatialPrimitiveDistanceFunction distFunction, bool doubleOptimize,
            IList<IKNNHeap> pr_heaps, IList<IKNNHeap> ps_heaps, N pr, N ps)
        {
            if (doubleOptimize)
            {
                var khp = pr_heaps;
                var khs = ps_heaps;

                //List<?> khp = (List<?>) pr_heaps;
                //List<?> khs = (List<?>) ps_heaps;
                ProcessDataPagesDouble((ISpatialPrimitiveDoubleDistanceFunction)distFunction, pr, ps,
                    (IList<IKNNHeap>)khp, (IList<IKNNHeap>)khs);
            }
            else
            {
                for (int j = 0; j < ps.GetNumEntries(); j++)
                {
                    SpatialPointLeafEntry s_e = ps.GetEntry(j) as SpatialPointLeafEntry;
                    IDbId s_id = s_e.GetDbId();
                    for (int i = 0; i < pr.GetNumEntries(); i++)
                    {
                        SpatialPointLeafEntry r_e = pr.GetEntry(i) as SpatialPointLeafEntry;
                        IDistanceValue distance = distFunction.MinDistance(s_e, r_e);
                        pr_heaps[(i)].Insert(distance, s_id);
                        if (!pr.Equals(ps) && ps_heaps != null)
                        {
                            ps_heaps[(j)].Insert(distance, r_e.GetDbId());
                        }
                    }
                }
            }
        }

        /**
         * Processes the two data pages pr and ps and determines the k-nearest
         * neighbors of pr in ps.
         * 
         * @param df the distance function to use
         * @param pr the first data page
         * @param ps the second data page
         * @param pr_heaps the knn lists for each data object
         * @param ps_heaps the knn lists for each data object in ps
         */
        private void ProcessDataPagesDouble(ISpatialPrimitiveDoubleDistanceFunction df, N pr, N ps,
            IList<IKNNHeap> pr_heaps, IList<IKNNHeap> ps_heaps)
        {
            // Compare pairwise
            for (int j = 0; j < ps.GetNumEntries(); j++)
            {
                SpatialPointLeafEntry s_e = ps.GetEntry(j) as SpatialPointLeafEntry;
                IDbId s_id = s_e.GetDbId();
                for (int i = 0; i < pr.GetNumEntries(); i++)
                {
                    SpatialPointLeafEntry r_e = pr.GetEntry(i) as SpatialPointLeafEntry;
                    double distance = df.MinDoubleDistance(s_e, r_e);
                    (pr_heaps[(i)] as IDoubleDistanceKNNHeap).Insert(distance, s_id);
                    if (!pr.Equals(ps) && ps_heaps != null)
                    {
                        (ps_heaps[(j)] as IDoubleDistanceKNNHeap).Insert(distance, r_e.GetDbId());
                    }
                }
            }
        }

        /**
         * Compute the maximum stop distance
         * 
         * @param heaps
         * @return the k-nearest neighbor distance of pr in ps
         */
        private IDistanceValue ComputeStopDistance(IList<IKNNHeap> heaps)
        {
            // Update pruning distance
            IDistanceValue pr_knn_distance = null;
            foreach (IKNNHeap knnList in heaps)
            {
                // set kNN distance of r
                if (pr_knn_distance == null)
                {
                    pr_knn_distance = knnList.KNNDistance;
                }
                else
                {
                    pr_knn_distance = DistanceUtil.Max(knnList.KNNDistance, pr_knn_distance);
                }
            }
            return pr_knn_distance;
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
         * Task in the processing queue
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        private class Task : IComparable<Task>
        {
            internal IDistanceValue mindist;

            internal int i;

            internal int j;

            /**
             * Constructor.
             * 
             * @param mindist
             * @param i
             * @param j
             */
            public Task(IDistanceValue mindist, int i, int j)
            {

                this.mindist = mindist;
                this.i = i;
                this.j = j;
            }


            public int CompareTo(Task o)
            {
                return mindist.CompareTo(o.mindist);
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPrimitiveDistanceBasedAlgorithm<V>.Parameterizer
        {
            protected int k;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter kP = new IntParameter(K_ID, 1);
                kP.AddConstraint(new GreaterConstraint<int>(0));
                if (config.Grab(kP))
                {
                    k = kP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new KNNJoin<V, N, E>(distanceFunction, k);
            }
        }
    }
}
