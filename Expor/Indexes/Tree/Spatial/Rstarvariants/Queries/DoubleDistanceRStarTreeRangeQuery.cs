using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Generic;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Tree.Queries;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Queries
{

    [Reference(Authors = "J. Kuan, P. Lewis",
        Title = "Fast k nearest neighbour search for R-tree family",
        BookTitle = "Proc. Int. Conf Information, Communications and Signal Processing, ICICS 1997",
        Url = "http://dx.doi.org/10.1109/ICICS.1997.652114")]
    public class DoubleDistanceRStarTreeRangeQuery<O, N, E> : AbstractDistanceRangeQuery<O>
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
        public DoubleDistanceRStarTreeRangeQuery(AbstractRStarTree<N, E> tree, IDistanceQuery distanceQuery,
            ISpatialPrimitiveDoubleDistanceFunction distanceFunction) :
            base(distanceQuery)
        {
            this.tree = tree;
            this.distanceFunction = distanceFunction;
        }

        /**
         * Perform the actual query process.
         * 
         * @param object Query object
         * @param epsilon Query range
         * @return Objects contained in query range.
         */
        protected GenericDistanceDbIdList DoRangeQuery(O obj, double epsilon)
        {
            GenericDistanceDbIdList result = new GenericDistanceDbIdList();
            Heap<DoubleDistanceSearchCandidate> pq = new Heap<DoubleDistanceSearchCandidate>();

            // push root
            pq.Add(new DoubleDistanceSearchCandidate(0.0, tree.GetRootID()));

            // search in tree
            while (pq.Count > 0)
            {
                DoubleDistanceSearchCandidate pqNode = pq.Poll();
                if (pqNode.mindist > epsilon)
                {
                    break;
                }

                AbstractRStarTreeNode<N, E> node = tree.GetNode(pqNode.nodeID);
                int numEntries = node.GetNumEntries();

                for (int i = 0; i < numEntries; i++)
                {
                    double distance = distanceFunction.MinDoubleDistance(obj, node.GetEntry(i));
                    tree.distanceCalcs++;
                    if (distance <= epsilon)
                    {
                        if (node.IsLeaf())
                        {
                            ILeafEntry entry = (ILeafEntry)node.GetEntry(i);
                            result.Add(new DoubleDistanceInt32DbIdPair(distance, entry.GetDbId().Int32Id));
                        }
                        else
                        {
                            IDirectoryEntry entry = (IDirectoryEntry)node.GetEntry(i);
                            pq.Add(new DoubleDistanceSearchCandidate(distance, entry.GetEntryID()));
                        }
                    }
                }
            }

            // sort the result according to the distances
            (result).Sort();
            return result;
        }


        public IDistanceDbIdList GetRangeForObject(O obj, DoubleDistanceValue range)
        {
            return DoRangeQuery(obj, range.DoubleValue());
        }


        public IDistanceDbIdList GetRangeForDbId(IDbIdRef id, DoubleDistanceValue range)
        {
            return GetRangeForObject((O)relation[(id)], range);
        }

        public override IDistanceDbIdList GetRangeForDbId(IDbIdRef id, IDistanceValue range)
        {
            return GetRangeForDbId(id, (DoubleDistanceValue)range);
        }

        public override IDistanceDbIdList GetRangeForObject(O obj, IDistanceValue range)
        {
            return GetRangeForObject(obj, (DoubleDistanceValue)range);
        }
    }
}
