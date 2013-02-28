using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public class LinearScanKNNQuery<O> : AbstractDistanceKNNQuery<O>, ILinearScanQuery
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanKNNQuery(IDistanceQuery distanceQuery)
            : base(distanceQuery)
        {
        }

        /**
         * Linear batch knn for arbitrary distance functions.
         * 
         * @param ids DbIds to process
         * @param heaps Heaps to store the results in
         */
        private void linearScanBatchKNN(IArrayDbIds ids, IList<IKNNHeap> heaps)
        {
            // The distance is computed on database IDs
            foreach (IDbId id in relation.GetDbIds())
            {
                IDbId candidateID = id;
                int index = 0;
                foreach (IDbId id2 in ids)
                {
                    IKNNHeap heap = heaps[index];
                    heap.Insert(distanceQuery.Distance(id2, candidateID), candidateID);
                    index++;
                }
            }
        }


        public override IKNNList GetKNNForDbId(IDbIdRef id, int k)
        {
            if (typeof(PrimitiveDistanceQuery<INumberVector>).IsInstanceOfType(distanceQuery))
            {
                // This should have yielded a LinearScanPrimitiveDistanceKNNQuery class!
                return GetKNNForObject((O)relation[(id)], k);
            }
            IKNNHeap heap = DbIdUtil.NewHeap(distanceQuery.DistanceFactory, k);
            if (distanceQuery is PrimitiveDistanceQuery<INumberVector>)
            {
                O obj = (O)relation[id];
                foreach (IDbId id2 in relation.GetDbIds())
                {
                    heap.Insert(distanceQuery.Distance((IDataVector)obj, (IDataVector)relation[id2]), id2);
                }
            }
            else
            {
                foreach (IDbId id3 in relation.GetDbIds())
                {
                    heap.Insert(distanceQuery.Distance(id, id3), id3);
                }
            }
            return heap.ToKNNList();
        }


        public override IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            int size = ids.Count;
            IList<IKNNHeap> heaps = new List<IKNNHeap>(size);
            for (int i = 0; i < size; i++)
            {
                heaps.Add(DbIdUtil.NewHeap(distanceQuery.DistanceFactory, k));
            }
            linearScanBatchKNN(ids, heaps);
            // Serialize heaps
            List<IKNNList> result = new List<IKNNList>(size);
            foreach (IKNNHeap heap in heaps)
            {
                result.Add(heap.ToKNNList());
            }
            return result;
        }


        public override void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            int size = heaps.Count;
            IArrayModifiableDbIds ids = DbIdUtil.NewArray(size);
            List<IKNNHeap> kheaps = new List<IKNNHeap>(size);
            foreach (var ent in heaps)
            {
                ids.Add(ent.Key);
                kheaps.Add(ent.Value);
            }
            linearScanBatchKNN(ids, kheaps);
        }


        public override IKNNList GetKNNForObject(O obj, int k)
        {
            IKNNHeap heap = DbIdUtil.NewHeap(distanceQuery.DistanceFactory, k);
            foreach (IDbId id in relation.GetDbIds())
            {
                heap.Insert(distanceQuery.Distance((IDataVector)obj, id), id);
            }
            return heap.ToKNNList();
        }

    }
}
