using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class KNNList : Wintellect.PowerCollections.ListBase<IDistanceResultPair>, IKNNResult
       
    {
        /**
         * The value of k this was materialized for.
         */
        private readonly int k;

        /**
         * The actual data array.
         */
        private readonly object[] data;

        /**
         * Constructor, to be called from KNNHeap only. Use {@link KNNHeap#toKNNList}
         * instead!
         * 
         * @param heap Calling heap
         */
        internal KNNList(KNNHeap heap)
            : base()
        {
            this.data = new Object[heap.Count];
            this.k = heap.GetK();
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

        /**
         * Constructor. With a KNNHeap, use {@link KNNHeap#toKNNList} instead!
         * 
         * @param heap Calling heap
         * @param k K value
         */
        public KNNList(Queue<IDistanceValue> heap, int k)
            : base()
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
                data[i] = heap.Dequeue();
            }
            Debug.Assert(data.Length == 0 || data[0] != null);
            Debug.Assert(heap.Count == 0);
        }


        public int GetK()
        {
            return k;
        }


        public IDistanceValue GetKNNDistance()
        {
            return Get(GetK() - 1).GetDistance();
        }


        public IArrayDbIds AsDbIds()
        {
            return KNNUtil.AsDbIds(this);
        }


        public IList<IDistanceValue> AsDistanceList()
        {
            return KNNUtil.AsDistanceList(this);
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("KNNList[");
            for (int i = 0; i < this.Count; i++)
            {
                buf.Append(this[i].GetDistance());
                buf.Append(":");
                buf.Append(this[i].DbId.ToString());
                if (i < this.Count - 1)
                    buf.Append(",");
            }

            return buf.ToString();
        }





        public int Size()
        {
            return this.Count;
        }

        public IDistanceResultPair Get(int index)
        {
            return this[index];
        }

        public override void Clear()
        {
            throw new NotSupportedException();
        }

        public override int Count
        {
            get { return  data.Length; }
        }

        public override void Insert(int index, IDistanceResultPair item)
        {
             throw new NotSupportedException();
        }
        

        public override void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public override IDistanceResultPair this[int index]
        {
            get
            {
                return data[index] as IDistanceResultPair;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
