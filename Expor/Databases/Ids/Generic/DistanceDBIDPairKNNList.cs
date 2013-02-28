using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Ids.Generic
{

    /**
     * Finalized KNN List.
     * 
     * @author Erich Schubert
     * 
     * @param <D> Distance type
     */
    public class DistanceDbIdPairKNNList : AbstractReadOnlyKnnList<IDistanceDbIdPair>
    {
        /**
         * The value of k this was materialized for.
         */
        private int k;

        /**
         * The actual data array.
         */
        private Object[] data;

        /**
         * Constructor, to be called from KNNHeap only. Use {@link KNNHeap#toKNNList}
         * instead!
         * 
         * @param heap Calling heap
         */
        internal DistanceDbIdPairKNNList(IKNNHeap heap)
        {

            this.data = new Object[heap.Count];
            this.k = heap.K;
            // Get sorted data from heap; but in reverse.
            int i = heap.Count;
            while (heap.Count > 0)
            {
                i--;
                Debug.Assert(i >= 0);
                data[i] = heap.Poll();
            }
            Debug.Assert(data.Length == 0 || data[0] != null);
            Debug.Assert(heap.Count == 0);
        }

        /**
         * Constructor. With a KNNHeap, use {@link KNNHeap#toKNNList} instead!
         * 
         * @param heap Calling heap
         * @param k K value
         */
        public DistanceDbIdPairKNNList(Heap<IDistanceDbIdPair> heap, int k)
        {

            this.data = new Object[heap.Count];
            this.k = k;
            Debug.Assert(heap.Count >= this.k, "Heap doesn't contain enough objects!");
            // Get sorted data from heap; but in reverse.
            int i = heap.Count;
            while (heap.Count > 0)
            {
                i--;
                Debug.Assert(i >= 0);
                data[i] = heap.Poll();
            }
            Debug.Assert(data.Length == 0 || data[0] != null);
            Debug.Assert(heap.Count == 0);
        }


        public override int K
        {
            get { return k; }
        }


        public override IDistanceValue KNNDistance
        {
            get { return this[(K - 1)].Distance; }
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("kNNList[");
            foreach (var iter in this)
            {
                buf.Append(iter.Distance).Append(':').Append(iter.ToString());

                buf.Append(',');

            }
            buf.Append(']');
            return buf.ToString();
        }



        public override IDistanceDbIdPair this[int index]
        {
            get { return (IDistanceDbIdPair)data[index]; }
        }




        public override int Count
        {
            get { return data.Length; }
        }


        public bool Contains(IDbIdRef o)
        {
            //   for (DbIdIter iter = iter(); iter.valid(); iter.advance()) {
            foreach (var iter in (IEnumerable<IDbId>)this)
            {
                if (DbIdUtil.IsEqual(iter, o))
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsEmpty()
        {
            return Count == 0;
        }

        public override bool Contains(IDistanceDbIdPair item)
        {
            return this.Contains(item.DbId);
        }
    }

}
