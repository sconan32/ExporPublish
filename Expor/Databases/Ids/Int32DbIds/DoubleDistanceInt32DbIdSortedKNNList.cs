using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Track the k nearest neighbors, with insertion sort to ensure the correct
     * order.
     * 
     * @author Erich Schubert
     */
    public class DoubleDistanceInt32DbIdSortedKNNList : DoubleDistanceInt32DbIdKNNList, IDoubleDistanceKNNHeap
    {
        /**
         * Constructor.
         * 
         * @param k K parameter
         */
        public DoubleDistanceInt32DbIdSortedKNNList(int k) :
            base(k, k + 11)
        {
        }

        /**
         * Add a new element to the heap/list.
         * 
         * @param dist Distance
         * @param id Object ID
         */

        protected  void AddInternal(double dist, int id)
        {
            if (Count >= k && dist > this[k - 1].DoubleDistance())
            {
                return;
            }
            InsertionSort(dist, id);
        }

        /**
         * Insertion sort a single object.
         * 
         * @param dist New distance
         * @param id New id
         */
        private void InsertionSort(double dist, int id)
        {
            // Ensure we have enough space.
            var pair = new DoubleDistanceInt32DbIdPair(dist, id);
            base.Add(pair);
            int pos = Count-1;
            while (pos > 0)
            {
                int pre = pos - 1;
                var predist = this[pre];
                if (predist.DoubleDistance() <= pair.DoubleDistance())
                {
                    break;
                }
                this[pos] = predist;
                pos = pre;
            }
            this[pos] = pair;
            return;
        }


        public double Insert(double dist, IDbIdRef id)
        {
            int kminus1 = k - 1;
            int iid = id.InternalGetIndex();
            if (Count >= k && dist > this[kminus1].DoubleDistance())
            {
                return (Count >= k) ? this[kminus1].DoubleDistance() : Double.PositiveInfinity;
            }
            InsertionSort(dist, iid);
            return (Count >= k) ? this[kminus1].DoubleDistance() : Double.PositiveInfinity;
        }


        public override void Add(double dist, IDbIdRef id)
        {
            AddInternal(dist, id.InternalGetIndex());
        }



        public void Insert(IDistanceValue dist, IDbIdRef id)
        {
            AddInternal((dist as DoubleDistanceValue).DoubleValue(), id.InternalGetIndex());
        }


        public void Insert(IDoubleDistanceDbIdPair e)
        {
            AddInternal(e.DoubleDistance(), e.InternalGetIndex());
        }



        public void Insert(DoubleDistanceValue dist, IDbIdRef id)
        {
            AddInternal(dist.DoubleValue(), id.InternalGetIndex());
        }


        public IDistanceDbIdPair Poll()
        {
            var last = this[Count-1];
            base.Remove(last);
            return last;
        }


        public IDistanceDbIdPair Peek()
        {
           // int last = size - 1;
            return this[Count - 1];
        }


        public IKNNList ToKNNList()
        {
            return this;
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("kNNListHeap[");
            foreach (var iter in this)
            {
                var it = iter as IDoubleDistanceDbIdPair;
                buf.Append(it.DoubleDistance()).Append(':').Append(iter.InternalGetIndex());

                buf.Append(',');

            }
            buf.Append(']');
            return buf.ToString();
        }


      
    }

}
