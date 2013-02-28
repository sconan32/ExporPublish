using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Maths;
using Socona.Expor.Utilities.Extenstions;
namespace Socona.Expor.Utilities.DataStructures.Heap
{

    /**
     * Binary heap for primitive types.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.has UnsortedIter
     */
    public class DoubleInt32MaxHeap : IDoubleInt32Heap
    {
        /**
         * Base heap.
         */
        protected double[] twoheap;

        /**
         * Base heap values.
         */
        protected int[] twovals;

        /**
         * Current size of heap.
         */
        protected int size;

        /**
         * Initial size of the 2-ary heap.
         */
        private static int TWO_HEAP_INITIAL_SIZE = (1 << 5) - 1;

        /**
         * Constructor, with default size.
         */
        public DoubleInt32MaxHeap()
        {

            double[] twoheap = new double[TWO_HEAP_INITIAL_SIZE];
            int[] twovals = new int[TWO_HEAP_INITIAL_SIZE];

            this.twoheap = twoheap;
            this.twovals = twovals;
        }

        /**
         * Constructor, with given minimum size.
         * 
         * @param minsize Minimum size
         */
        public DoubleInt32MaxHeap(int minsize)
        {

            int size = MathUtil.NextPow2Int(minsize + 1) - 1;
            double[] twoheap = new double[size];
            int[] twovals = new int[size];

            this.twoheap = twoheap;
            this.twovals = twovals;
        }


        public void Clear()
        {
            size = 0;
            twoheap.Fill(0.0);
            twovals.Fill(0);
        }


        public int Count
        {
            get { return size; }
        }


        public bool IsEmpty()
        {
            return (size == 0);
        }


        public void Add(double o, int v)
        {
            double co = o;
            int cv = v;
            // System.err.println("Add: " + o);
            if (size >= twoheap.Length)
            {
                // Grow by one layer.
                var ntwoheap = new double[twoheap.Length * 2 + 1];
                var ntwovals = new int[twovals.Length * 2 + 1];
                Array.Copy(twoheap, ntwoheap, twoheap.Length);
                Array.Copy(twovals, ntwovals, twovals.Length);
                twoheap = ntwoheap;
                twovals = ntwovals;
            }
            int twopos = size;
            twoheap[twopos] = co;
            twovals[twopos] = cv;
            ++size;
            HeapifyUp(twopos, co, cv);
        }


        public void Add(double key, int val, int max)
        {
            if (size < max)
            {
                Add(key, val);
            }
            else if (twoheap[0] >= key)
            {
                ReplaceTopElement(key, val);
            }
        }


        public void ReplaceTopElement(double reinsert, int val)
        {
            HeapifyDown(reinsert, val);
        }

        /**
         * Heapify-Up method for 2-ary heap.
         * 
         * @param twopos Position in 2-ary heap.
         * @param cur Current object
         * @param val Current value
         */
        private void HeapifyUp(int twopos, double cur, int val)
        {
            while (twopos > 0)
            {
                int parent = (twopos - 1) >> 1;
                double par = twoheap[parent];
                if (cur <= par)
                {
                    break;
                }
                twoheap[twopos] = par;
                twovals[twopos] = twovals[parent];
                twopos = parent;
            }
            twoheap[twopos] = cur;
            twovals[twopos] = val;
        }


        public void Poll()
        {
            --size;
            // Replacement object:
            if (size > 0)
            {
                double reinsert = twoheap[size];
                int reinsertv = twovals[size];
                twoheap[size] = 0.0;
                twovals[size] = 0;
                HeapifyDown(reinsert, reinsertv);
            }
            else
            {
                twoheap[0] = 0.0;
                twovals[0] = 0;
            }
        }

        /**
         * Invoke heapify-down for the root object.
         * 
         * @param cur Object to insert.
         * @param val Value to reinsert.
         */
        private void HeapifyDown(double cur, int val)
        {
            int stop = size >> 1;
            int twopos = 0;
            while (twopos < stop)
            {
                int bestchild = (twopos << 1) + 1;
                double best = twoheap[bestchild];
                int right = bestchild + 1;
                if (right < size && best < twoheap[right])
                {
                    bestchild = right;
                    best = twoheap[right];
                }
                if (cur >= best)
                {
                    break;
                }
                twoheap[twopos] = best;
                twovals[twopos] = twovals[bestchild];
                twopos = bestchild;
            }
            twoheap[twopos] = cur;
            twovals[twopos] = val;
        }


        public double PeekKey()
        {
            return twoheap[0];
        }


        public int PeekValue()
        {
            return twovals[0];
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(typeof(DoubleInt32MaxHeap).Name).Append(" [");
            foreach (var iter in this)
            {
                buf.Append(iter.Key).Append(':').Append(iter.Value).Append(',');
            }
            buf.Append(']');
            return buf.ToString();
        }




        public IEnumerator<KeyValuePair<double, int>> GetEnumerator()
        {
            for (int i = 0; i < size; i++)
            {
                yield return new KeyValuePair<double, int>(twoheap[i], twovals[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
