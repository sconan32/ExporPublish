using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Generic
{

    /**
     * Heap for collecting kNN candidates with arbitrary distance types.
     * 
     * For double distances, see {@link DoubleDistanceDbIdPairKNNHeap}
     * 
     * <b>To instantiate, use {@link de.lmu.ifi.dbs.elki.database.ids.DbIdUtil#newHeap} instead!</b>
     * 
     * @author Erich Schubert
     * 
     * @param <D> Distance type
     */
    public class DistanceDbIdPairKNNHeap : AbstractKNNHeap
    {
        /**
         * Cached distance to k nearest neighbor (to avoid going through {@link #peek}
         * each time).
         */
        protected IDistanceValue knndistance = null;

        /**
         * Constructor.
         * 
         * <b>To instantiate, use {@link de.lmu.ifi.dbs.elki.database.ids.DbIdUtil#newHeap} instead!</b>
         * 
         * @param k Heap size
         */
        public DistanceDbIdPairKNNHeap(int k) :
            base(k)
        {
        }

        /**
         * Serialize to a {@link DistanceDbIdPairKNNList}. This empties the heap!
         * 
         * @return KNNList with the heaps contents.
         */

        public override IKNNList ToKNNList()
        {
            return new DistanceDbIdPairKNNList(this);
        }


        public override void Insert(IDistanceValue distance, IDbIdRef id)
        {
            if (Count < K)
            {
                heap.Add(DbIdFactoryBase.FACTORY.NewDistancePair(distance, id));
                HeapModified();
                return;
            }
            // size >= maxsize. Insert only when necessary.
            if (knndistance.CompareTo(distance) >= 0)
            {
                // Replace worst element.
                heap.Add(DbIdFactoryBase.FACTORY.NewDistancePair(distance, id));
                HeapModified();
            }
        }


        public override void Insert(IDistanceDbIdPair pair)
        {
            if (Count < K || knndistance.CompareTo(pair.Distance) >= 0)
            {
                heap.Add(pair);
                HeapModified();
            }
        }

        // 
        protected void HeapModified()
        {
            // base.heapModified();
            // Update threshold.
            if (Count >= K)
            {
                knndistance = heap.Peek().Distance;
            }
        }


        public override IDistanceValue KNNDistance
        {
            get { return knndistance; }
        }
    }

}
