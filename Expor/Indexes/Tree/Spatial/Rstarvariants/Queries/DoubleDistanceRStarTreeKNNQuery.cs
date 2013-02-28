using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Tree.Queries;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Queries
{

    [Reference(Authors = "G. R. Hjaltason, H. Samet",
        Title = "Ranking in spatial databases",
        BookTitle = "Advances in Spatial Databases - 4th Symposium, SSD'95",
        Url = "http://dx.doi.org/10.1007/3-540-60159-7_6")]
    public class DoubleDistanceRStarTreeKNNQuery<O, N, E> : AbstractDistanceKNNQuery<O>
        where O : ISpatialComparable,IDataVector
        where N : AbstractRStarTreeNode<N, E>
        where E : ISpatialEntry
    {
        /**
         * The index to use
         */
        protected AbstractRStarTree<N, E> tree;

        /**
         * Spatial primitive distance function
         */
        protected ISpatialPrimitiveDoubleDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param tree Index to use
         * @param distanceQuery Distance query to use
         * @param distanceFunction Distance function
         */
        public DoubleDistanceRStarTreeKNNQuery(AbstractRStarTree<N, E> tree, IDistanceQuery distanceQuery,
            ISpatialPrimitiveDoubleDistanceFunction distanceFunction) :
            base(distanceQuery)
        {
            this.tree = tree;
            this.distanceFunction = distanceFunction;
        }

        /**
         * Performs a k-nearest neighbor query for the given NumberVector with the
         * given parameter k and the according distance function. The query result is
         * in ascending order to the distance to the query object.
         * 
         * @param object the query object
         * @param knnList the knn list containing the result
         */
        protected void DoKNNQuery(O obj, IKNNHeap knnList)
        {
            Heap<DoubleDistanceSearchCandidate> pq = new Heap<DoubleDistanceSearchCandidate>
                ((int)Math.Min(knnList.K * 2, 20));

            // push root
            pq.Add(new DoubleDistanceSearchCandidate(0.0, tree.GetRootID()));
            double maxDist = Double.MaxValue;

            // search in tree
            while (pq.Count > 0)
            {
                DoubleDistanceSearchCandidate pqNode = pq.Poll();

                if (pqNode.mindist > maxDist)
                {
                    return;
                }
                maxDist = ExpandNode(obj, knnList, pq, maxDist, pqNode.nodeID);
            }
        }

        private double ExpandNode(O obj, IKNNHeap knnList, Heap<DoubleDistanceSearchCandidate> pq,
            double maxDist, int nodeID)
        {
            AbstractRStarTreeNode<N, E> node = tree.GetNode(nodeID);
            // data node
            if (node.IsLeaf())
            {
                for (int i = 0; i < node.GetNumEntries(); i++)
                {
                    ISpatialEntry entry = node.GetEntry(i);
                    double distance = distanceFunction.MinDoubleDistance(entry, obj);
                    tree.distanceCalcs++;
                    if (distance <= maxDist)
                    {
                        (knnList as IDoubleDistanceKNNHeap).Insert(distance, ((ILeafEntry)entry).GetDbId());
                        maxDist = ((DoubleDistanceValue)knnList.KNNDistance).DoubleValue();
                    }
                }
            }
            // directory node
            else
            {
                for (int i = 0; i < node.GetNumEntries(); i++)
                {
                    ISpatialEntry entry = node.GetEntry(i);
                    double distance = distanceFunction.MinDoubleDistance(entry, obj);
                    tree.distanceCalcs++;
                    // Greedy expand, bypassing the queue
                    if (distance <= 0)
                    {
                        ExpandNode(obj, knnList, pq, maxDist, ((IDirectoryEntry)entry).GetPageID());
                    }
                    else
                    {
                        if (distance <= maxDist)
                        {
                            pq.Add(new DoubleDistanceSearchCandidate(distance, ((IDirectoryEntry)entry).GetPageID()));
                        }
                    }
                }
            }
            return maxDist;
        }

        /**
         * Performs a batch knn query.
         * 
         * @param node the node for which the query should be performed
         * @param knnLists a map containing the knn lists for each query objects
         */
        protected void BatchNN(AbstractRStarTreeNode<N, E> node, IDictionary<IDbId, IKNNHeap> knnLists)
        {
            if (node.IsLeaf())
            {
                for (int i = 0; i < node.GetNumEntries(); i++)
                {
                    ISpatialEntry p = node.GetEntry(i);
                    foreach (var ent in knnLists)
                    {
                        IDbId q = ent.Key;
                        IKNNHeap knns_q = ent.Value;
                        DoubleDistanceValue knn_q_maxDist = (DoubleDistanceValue)knns_q.KNNDistance;

                        IDbId pid = ((ILeafEntry)p).GetDbId();
                        // FIXME: objects are NOT accessible by IDbId in a plain rtree context!
                        DoubleDistanceValue dist_pq = (DoubleDistanceValue)distanceFunction.Distance(
                            (O)relation[(pid)], (O)relation[(q)]);
                        tree.distanceCalcs++;
                        if (dist_pq.CompareTo(knn_q_maxDist) <= 0)
                        {
                            knns_q.Insert(dist_pq, pid);
                        }
                    }
                }
            }
            else
            {
                IModifiableDbIds ids = DbIdUtil.NewArray(knnLists.Count);
                foreach (IDbId id in knnLists.Keys)
                {
                    ids.Add(id);
                }
                IList<DoubleDistanceEntry> entries = GetSortedEntries(node, ids);
                foreach (DoubleDistanceEntry distEntry in entries)
                {
                    double minDist = distEntry.distance;
                    foreach (var ent in knnLists)
                    {
                        IKNNHeap knns_q = ent.Value;
                        double knn_q_maxDist = ((DoubleDistanceValue)knns_q.KNNDistance).DoubleValue();

                        if (minDist <= knn_q_maxDist)
                        {
                            ISpatialEntry entry = distEntry.entry;
                            AbstractRStarTreeNode<N, E> child = tree.GetNode(((IDirectoryEntry)entry).GetPageID());
                            BatchNN(child, knnLists);
                            break;
                        }
                    }
                }
            }
        }

        /**
         * Sorts the entries of the specified node according to their minimum distance
         * to the specified objects.
         * 
         * @param node the node
         * @param ids the id of the objects
         * @return a list of the sorted entries
         */
        protected IList<DoubleDistanceEntry> GetSortedEntries(AbstractRStarTreeNode<N, E> node, IDbIds ids)
        {
            List<DoubleDistanceEntry> result = new List<DoubleDistanceEntry>();

            for (int i = 0; i < node.GetNumEntries(); i++)
            {
                ISpatialEntry entry = node.GetEntry(i);
                double minMinDist = Double.MaxValue;
                // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var iter in ids)
                {
                    double minDist = distanceFunction.MinDoubleDistance(entry, (O)relation[(iter)]);
                    tree.distanceCalcs++;
                    minMinDist = Math.Min(minDist, minMinDist);
                }
                result.Add(new DoubleDistanceEntry(entry, minMinDist));
            }

            (result).Sort();
            return result;
        }

        /**
         * Optimized double distance entry implementation.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.hidden
         */
        public class DoubleDistanceEntry : IComparable<DoubleDistanceEntry>
        {
            /**
             * Referenced entry
             */
            internal ISpatialEntry entry;

            /**
             * Distance value
             */
            internal double distance;

            /**
             * Constructor.
             * 
             * @param entry Entry
             * @param distance Distance
             */
            public DoubleDistanceEntry(ISpatialEntry entry, double distance)
            {
                this.entry = entry;
                this.distance = distance;
            }


            public int CompareTo(DoubleDistanceEntry o)
            {
                return this.distance.CompareTo(o.distance);
            }
        }


        public override IKNNList GetKNNForObject(O obj, int k)
        {
            if (k < 1)
            {
                throw new ArgumentException("At least one enumeration has to be requested!");
            }

            IKNNHeap knnList = DbIdUtil.NewHeap(distanceFunction.DistanceFactory.Infinity, k);
            DoKNNQuery(obj, knnList);
            return knnList.ToKNNList();
        }


        public override IKNNList GetKNNForDbId(IDbIdRef id, int k)
        {
            return GetKNNForObject((O)relation[id], k);
        }


        public override IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            if (k < 1)
            {
                throw new ArgumentException("At least one enumeration has to be requested!");
            }

            // While this works, it seems to be slow at least for large sets!
            IDictionary<IDbId, IKNNHeap> knnLists = new Dictionary<IDbId, IKNNHeap>(ids.Count);
            //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                IDbId id = iter.DbId;
                knnLists[id] = DbIdUtil.NewHeap(distanceFunction.DistanceFactory.Infinity, k);
            }

            BatchNN(tree.GetRoot(), knnLists);

            List<IKNNList> result = new List<IKNNList>();
            //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                IDbId id = iter.DbId;
                result.Add(knnLists[(id)].ToKNNList());
            }
            return result;
        }


        public override void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            AbstractRStarTreeNode<N, E> root = tree.GetRoot();
            BatchNN(root, heaps);
        }
    }
}
