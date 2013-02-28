using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class UpdatableHeap<O> : Heap<O>
    {
        /**
         * Constant for "not in heap"
         */
        protected static int NO_VALUE = Int32.MinValue;

        /**
         * Constant for "in ties list", for tied heaps.
         */
        protected static int IN_TIES = -1;

        /**
         * Serial version
         */
        //private static long serialVersionUID = 1L;

        /**
         * Holds the indices in the heap of each element.
         */
        protected IDictionary<Object, int> index = new Dictionary<Object, int>(100);

        /**
         * Simple constructor with default size.
         */
        public UpdatableHeap() :
            base()
        {
        }

        /**
         * Constructor with predefined size.
         * 
         * @param size Size
         */
        public UpdatableHeap(int size)
            : base(size)
        {
        }

        /**
         * Constructor with comparator
         * 
         * @param comparator Comparator
         */
        public UpdatableHeap(IComparer<O> comparator) :
            base(comparator)
        {
        }

        /**
         * Constructor with predefined size and comparator
         * 
         * @param size Size
         * @param comparator Comparator
         */
        public UpdatableHeap(int size, IComparer<O> comparator) :
            base(size, comparator)
        {
        }


        public new void Clear()
        {
            base.Clear();
            index.Clear();
        }


        public override bool Offer(O e)
        {
            int pos = index[(e)];
            return base.Offer(e);
        }

        //protected bool OfferAt(int pos, O e)
        //{
        //if (pos == NO_VALUE)
        //{
        //    // resize when needed
        //    //if (size + 1 > base.Count)
        //    //{
        //    //    Resize(size + 1);
        //    //}
        //    int size = base.Count;

        //    this.a
        //    index[e] = size;
        //    size += 1;
        //    // We do NOT YET update the heap. This is done lazily.
        //    // We have changed - return true according to {@link Collection#put}
        //    modCount++;
        //    return true;
        //}
        //else
        //{
        //    Debug.Assert(pos >= 0, "Unexpected negative position.");
        //    Debug.Assert(this[pos].Equals(e));
        //    // Did the value improve?
        //    if (comparator == null)
        //    {

        //        IComparable<Object> c = (IComparable<Object>)e;
        //        if (c.CompareTo(queue[pos]) >= 0)
        //        {
        //            // Ignore, but return true according to {@link Collection#put}
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if (comparator.compare(e, queue[pos]) >= 0)
        //        {
        //            // Ignore, but return true according to {@link Collection#put}
        //            return true;
        //        }
        //    }
        //    if (pos >= validSize)
        //    {
        //        queue[pos] = e;
        //        // validSize = Math.min(pos, validSize);
        //    }
        //    else
        //    {
        //        // ensureValid();
        //        heapifyUp(pos, e);
        //    }
        //    modCount++;
        //    // We have changed - return true according to {@link Collection#put}
        //    return true;
        //}
        //}


        //protected override O RemoveAt(int pos)
        //{
        //    if (pos < 0 || pos >= size)
        //    {
        //        return null;
        //    }
        //    O ret = CastQueueElement(pos);
        //    // Replacement object:
        //    Object reinsert = queue[size - 1];
        //    queue[size - 1] = null;
        //    // Keep heap in sync?
        //    if (validSize == size)
        //    {
        //        size -= 1;
        //        validSize -= 1;
        //        if (comparator != null)
        //        {
        //            if (comparator.compare(ret, reinsert) > 0)
        //            {
        //                HeapifyUpComparator(pos, reinsert);
        //            }
        //            else
        //            {
        //                HeapifyDownComparator(pos, reinsert);
        //            }
        //        }
        //        else
        //        {

        //            IComparable<Object> comp = (Comparable<Object>)ret;
        //            if (comp.CompareTo(reinsert) > 0)
        //            {
        //                HeapifyUpComparable(pos, reinsert);
        //            }
        //            else
        //            {
        //                HeapifyDownComparable(pos, reinsert);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        size -= 1;
        //        validSize = Math.Min(pos >> 1, validSize);
        //        queue[pos] = reinsert;
        //        index.put(reinsert, pos);
        //    }
        //    modCount++;
        //    // Keep index up to date
        //    index.Remove(ret);
        //    return ret;
        //}

        /**
         * Remove the given object from the queue.
         * 
         * @param e Object to remove
         * @return Existing entry
         */
        public O RemoveObject(O e)
        {
            int pos = index[(e)];
            if (pos >= 0)
            {
                return RemoveAt(pos);
            }
            else
            {
                return default(O);
            }
        }


        public override O Poll()
        {
            O node = base.Poll();
            index.Remove(node);
            return node;
        }

        /**
         * Execute a "Heapify Upwards" aka "SiftUp". Used in insertions.
         * 
         * @param pos insertion position
         * @param elem Element to insert
         */


        //protected void HeapifyUpComparable(int pos, Object elem)
        //{
        //    IComparable<Object> cur = (IComparable<Object>)elem; // queue[pos];
        //    while (pos > 0)
        //    {
        //        int parent = (pos - 1) >> 1;
        //        Object par = queue[parent];

        //        if (cur.CompareTo(par) >= 0)
        //        {
        //            break;
        //        }
        //        queue[pos] = par;
        //        index.put(par, pos);
        //        pos = parent;
        //    }
        //    queue[pos] = cur;
        //    index.put(cur, pos);
        //}

        ///**
        // * Execute a "Heapify Upwards" aka "SiftUp". Used in insertions.
        // * 
        // * @param pos insertion position
        // * @param cur Element to insert
        // */

        //protected void HeapifyUpComparator(int pos, Object cur)
        //{
        //    while (pos > 0)
        //    {
        //        int parent = (pos - 1) >> 1;
        //        Object par = queue[parent];

        //        if (comparator.compare(cur, par) >= 0)
        //        {
        //            break;
        //        }
        //        queue[pos] = par;
        //        index.put(par, pos);
        //        pos = parent;
        //    }
        //    queue[pos] = cur;
        //    index.put(cur, pos);
        //}



        //protected bool HeapifyDownComparable(int ipos, Object reinsert)
        //{
        //    IComparable<Object> cur = (IComparable<Object>)reinsert;
        //    int pos = ipos;
        //    int half = size >> 1;
        //    while (pos < half)
        //    {
        //        // Get left child (must exist!)
        //        int cpos = (pos << 1) + 1;
        //        Object child = queue[cpos];
        //        // Test right child, if present
        //        int rchild = cpos + 1;
        //        if (rchild < size)
        //        {
        //            Object right = queue[rchild];
        //            if (((Comparable<Object>)child).CompareTo(right) > 0)
        //            {
        //                cpos = rchild;
        //                child = right;
        //            }
        //        }

        //        if (cur.CompareTo(child) <= 0)
        //        {
        //            break;
        //        }
        //        queue[pos] = child;
        //        index.put(child, pos);
        //        pos = cpos;
        //    }
        //    queue[pos] = cur;
        //    index.put(cur, pos);
        //    return (pos == ipos);
        //}


        //protected bool HeapifyDownComparator(int ipos, Object cur)
        //{
        //    int pos = ipos;
        //    int half = size >> 1;
        //    while (pos < half)
        //    {
        //        int min = pos;
        //        Object best = cur;

        //        int lchild = (pos << 1) + 1;
        //        Object left = queue[lchild];
        //        if (comparator.compare(best, left) > 0)
        //        {
        //            min = lchild;
        //            best = left;
        //        }
        //        int rchild = lchild + 1;
        //        if (rchild < size)
        //        {
        //            Object right = queue[rchild];
        //            if (comparator.compare(best, right) > 0)
        //            {
        //                min = rchild;
        //                best = right;
        //            }
        //        }
        //        if (min == pos)
        //        {
        //            break;
        //        }
        //        queue[pos] = best;
        //        index.put(best, pos);
        //        pos = min;
        //    }
        //    queue[pos] = cur;
        //    index.put(cur, pos);
        //    return (pos == ipos);
        //}
    }
}
