using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public class LinearScanRawDoubleDistanceKNNQuery : LinearScanPrimitiveDistanceKNNQuery
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanRawDoubleDistanceKNNQuery(PrimitiveDistanceQuery<INumberVector> distanceQuery)
            : base(distanceQuery)
        {
            if (!(distanceQuery.DistanceFunction is IPrimitiveDoubleDistanceFunction<INumberVector>))
            {
                throw new InvalidOperationException("LinearScanRawDoubleDistance instantiated for non-RawDoubleDistance!");
            }
        }


        public override IKNNList GetKNNForDbId(IDbIdRef id, int k)
        {
            return GetKNNForObject((INumberVector)relation[id], k);
        }


        public override IKNNList GetKNNForObject(INumberVector obj, int k)
        {

            IPrimitiveDoubleDistanceFunction<ISpatialComparable> rawdist = (IPrimitiveDoubleDistanceFunction<ISpatialComparable>)distanceQuery.DistanceFunction;
            // Optimization for double distances.
            IKNNHeap heap = DbIdUtil.NewHeap(distanceQuery.DistanceFactory, k);
            double max = Double.PositiveInfinity;
            foreach (var id in relation.GetDbIds())
            {
                double doubleDistance = rawdist.DoubleDistance(obj, (INumberVector)relation[id]);
                if (doubleDistance <= max)
                {
                    (heap as IDoubleDistanceKNNHeap).Insert(doubleDistance, id.DbId);
                    // Update cutoff
                    if (heap.Count >= heap.K)
                    {
                        max = ((IDoubleDistanceDbIdPair)heap.Peek()).DoubleDistance();
                    }
                }
            }
            return heap.ToKNNList();
        }


        protected void linearScanBatchKNN(List<INumberVector> objs, List<KNNHeap> heaps)
        {
            int size = objs.Count;

            IPrimitiveDoubleDistanceFunction<INumberVector> rawdist = (IPrimitiveDoubleDistanceFunction<INumberVector>)distanceQuery.DistanceFunction;
            // Track the max ourselves to reduce object access for comparisons.
            double[] max = new double[size];
            for (int i = 0; i < max.Length; i++)
            {
                max[i] = double.PositiveInfinity;
            }

            // The distance is computed on arbitrary vectors, we can reduce object
            // loading by working on the actual vectors.
            foreach (IDbId id in relation.GetDbIds())
            {
                INumberVector candidate = (INumberVector)relation[id];
                for (int index = 0; index < size; index++)
                {
                    KNNHeap heap = heaps[index];
                    double doubleDistance = rawdist.DoubleDistance(objs[index], candidate);
                    if (doubleDistance <= max[index])
                    {
                        heap.Add(new DoubleDistanceResultPair(doubleDistance, id));
                        if (heap.Count >= heap.GetK())
                        {
                            max[index] = ((DoubleDistanceResultPair)heap.Peek()).GetDoubleDistance();
                        }
                    }
                }
            }
        }
    }
}
