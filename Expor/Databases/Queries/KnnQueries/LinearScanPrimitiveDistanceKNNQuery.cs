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

    public class LinearScanPrimitiveDistanceKNNQuery : LinearScanKNNQuery<INumberVector>
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanPrimitiveDistanceKNNQuery(PrimitiveDistanceQuery<INumberVector> distanceQuery) :
            base(distanceQuery)
        {
        }

        /**
         * Perform a linear scan batch kNN for primitive distance functions.
         * 
         * @param objs Objects list
         * @param heaps Heaps array
         */
        protected void LinearScanBatchKNN(IList<INumberVector> objs, IList<IKNNHeap> heaps)
        {
            int size = objs.Count;
            // Linear scan style KNN.
            foreach (IDbId id in relation.GetDbIds())
            {
                IDbId candidateID = id;
                INumberVector candidate = (INumberVector)relation[candidateID];
                for (int index = 0; index < size; index++)
                {
                    INumberVector obj = objs[index];
                    IKNNHeap heap = heaps[index];
                    heap.Insert(distanceQuery.Distance(obj, candidate), candidateID);
                }
            }
        }


        public override IKNNList GetKNNForDbId(IDbIdRef id, int k)
        {
            return GetKNNForObject((INumberVector)relation[id], k);
        }


        public override IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            int size = ids.Count;
            IList<IKNNHeap> heaps = new List<IKNNHeap>(size);
            List<INumberVector> objs = new List<INumberVector>(size);
            foreach (var id in ids)
            {
                heaps.Add(DbIdUtil.NewHeap(distanceQuery.DistanceFactory, k));
                objs.Add((INumberVector)relation[id]);
            }
            LinearScanBatchKNN(objs, heaps);

            List<IKNNList> result = new List<IKNNList>(heaps.Count);
            foreach (IKNNHeap heap in heaps)
            {
                result.Add(heap.ToKNNList());
            }
            return result;
        }


        public override void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            IList<INumberVector> objs = new List<INumberVector>(heaps.Count);
            IList<IKNNHeap> kheaps = new List<IKNNHeap>(heaps.Count);
            foreach (var ent in heaps)
            {
                objs.Add((INumberVector)relation[ent.Key]);
                kheaps.Add(ent.Value);
            }
            LinearScanBatchKNN(objs, kheaps);
        }
    }
}
