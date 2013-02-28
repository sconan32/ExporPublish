using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Distances;
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
    public class GenericRStarTreeKNNQuery<O, N, E> : AbstractDistanceKNNQuery<O>
        where O : ISpatialComparable
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
        protected ISpatialPrimitiveDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param tree Index to use
         * @param distanceQuery Distance query to use
         */
        public GenericRStarTreeKNNQuery(AbstractRStarTree<N, E> tree, ISpatialDistanceQuery distanceQuery) :
            base(distanceQuery)
        {
            this.tree = tree;
            this.distanceFunction = (ISpatialPrimitiveDistanceFunction)distanceQuery.DistanceFunction;
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
            Heap<GenericDistanceSearchCandidate> pq =
                new Heap<GenericDistanceSearchCandidate>(Math.Min(knnList.K * 2, 20));

            // push root
            pq.Add(new GenericDistanceSearchCandidate(distanceFunction.DistanceFactory.Empty, tree.GetRootID()));
            IDistanceValue maxDist = distanceFunction.DistanceFactory.Infinity;

            // search in tree
            while (pq.Count > 0)
            {
                GenericDistanceSearchCandidate pqNode = pq.Poll();

                if (pqNode.mindist.CompareTo(maxDist) > 0)
                {
                    return;
                }
                maxDist = ExpandNode(obj, knnList, pq, maxDist, pqNode.nodeID);
            }
        }

        private IDistanceValue ExpandNode(O obj, IKNNHeap knnList, Heap<GenericDistanceSearchCandidate> pq,
            IDistanceValue maxDist, Int32 nodeID)
        {
            AbstractRStarTreeNode<N, E> node = tree.GetNode(nodeID);
            // data node
            if (node.IsLeaf())
            {
                for (int i = 0; i < node.GetNumEntries(); i++)
                {
                    ISpatialEntry entry = node.GetEntry(i);
                    IDistanceValue distance = distanceFunction.MinDistance(entry, obj);
                    tree.distanceCalcs++;
                    if (distance.CompareTo(maxDist) <= 0)
                    {
                        knnList.Insert(distance, ((ILeafEntry)entry).GetDbId());
                        maxDist = knnList.KNNDistance;
                    }
                }
            }
            // directory node
            else
            {
                for (int i = 0; i < node.GetNumEntries(); i++)
                {
                    ISpatialEntry entry = node.GetEntry(i);
                    IDistanceValue distance = distanceFunction.MinDistance(entry, obj);
                    tree.distanceCalcs++;
                    // Greedy expand, bypassing the queue
                    if (distance.IsEmpty)
                    {
                        ExpandNode(obj, knnList, pq, maxDist, ((IDirectoryEntry)entry).GetPageID());
                    }
                    else
                    {
                        if (distance.CompareTo(maxDist) <= 0)
                        {
                            pq.Add(new GenericDistanceSearchCandidate(distance, ((IDirectoryEntry)entry).GetPageID()));
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
                        IDistanceValue knn_q_maxDist = knns_q.KNNDistance;

                        IDbId pid = ((ILeafEntry)p).GetDbId();
                        // FIXME: objects are NOT accessible by DbId in a plain rtree context!
                        IDistanceValue dist_pq = distanceQuery.Distance(pid, q);
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
                IList<DistanceEntry<ISpatialEntry>> entries = GetSortedEntries(node, ids);
                foreach (DistanceEntry<ISpatialEntry> distEntry in entries)
                {
                    IDistanceValue minDist = distEntry.GetDistance();
                    foreach (var ent in knnLists)
                    {
                        IKNNHeap knns_q = ent.Value;
                        IDistanceValue knn_q_maxDist = knns_q.KNNDistance;

                        if (minDist.CompareTo(knn_q_maxDist) <= 0)
                        {
                            ISpatialEntry entry = distEntry.GetEntry();
                            AbstractRStarTreeNode<N, E> child = tree.GetNode(((IDirectoryEntry)entry).GetPageID());
                            BatchNN(child, knnLists);
                            break;
                        }
                    }
                }
            }
        }


        public override void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            AbstractRStarTreeNode<N, E> root = tree.GetRoot();
            BatchNN(root, heaps);
        }

        /**
         * Sorts the entries of the specified node according to their minimum distance
         * to the specified objects.
         * 
         * @param node the node
         * @param ids the id of the objects
         * @return a list of the sorted entries
         */
        protected IList<DistanceEntry<ISpatialEntry>> GetSortedEntries(AbstractRStarTreeNode<N, E> node, IDbIds ids)
        {
            List<DistanceEntry<ISpatialEntry>> result = new List<DistanceEntry<ISpatialEntry>>();

            for (int i = 0; i < node.GetNumEntries(); i++)
            {
                ISpatialEntry entry = node.GetEntry(i);
                IDistanceValue minMinDist = distanceQuery.DistanceFactory.Infinity;
                //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var iter in ids)
                {
                    IDistanceValue minDist = distanceFunction.MinDistance(entry, (O)relation[(iter)]);
                    minMinDist = DistanceUtil.Min(minDist, minMinDist);
                }
                result.Add(new DistanceEntry<ISpatialEntry>(entry, minMinDist, i));
            }

            (result).Sort();
            return result;
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
            return GetKNNForObject((O)relation[(id)], k);
        }


        public override IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            if (k < 1)
            {
                throw new ArgumentException("At least one enumeration has to be requested!");
            }
            // While this works, it seems to be slow at least for large sets!
            IDictionary<IDbId, IKNNHeap> knnLists = new Dictionary<IDbId, IKNNHeap>(ids.Count);
            // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                knnLists[iter.DbId] = DbIdUtil.NewHeap(distanceFunction.DistanceFactory.Infinity, k);
            }

            BatchNN(tree.GetRoot(), knnLists);

            List<IKNNList> result = new List<IKNNList>();
            //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                result.Add(knnLists[iter.DbId].ToKNNList());
            }
            return result;
        }
    }
}
