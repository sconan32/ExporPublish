using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wintellect.PowerCollections;

namespace Socona.Expor.Utilities.DataStructures.Heap
{
    [Serializable]
    public class Heap<E> : Wintellect.PowerCollections.OrderedBag<E>
    {
        int maxcount;
        ///**
        // * Default initial capacity
        // */
        private static readonly int DEFAULT_INITIAL_CAPACITY = 11;

        /**
         * Default constructor: default capacity, natural ordering.
         */
        public Heap()
            : this(DEFAULT_INITIAL_CAPACITY)
        {
        }

        /**
         * Constructor with initial capacity, natural ordering.
         * 
         * @param size initial size
         */
        public Heap(int size) :
            this(size, comp: null)
        {
        }

        /**
         * Constructor with {@link Comparator}.
         * 
         * @param comparator Comparator
         */
        public Heap(IComparer<E> comparator) :
            this(DEFAULT_INITIAL_CAPACITY, comparator)
        {

        }

        /**
         * Constructor with initial capacity and {@link Comparator}.
         * 
         * @param size initial capacity
         * @param comparator Comparator
         */

        public Heap(int size, IComparer<E> comparator)
            : base(comparator)
        {
            this.maxcount = size;
        }
        public Heap(int size, Comparison<E> comp)
            : base(comp)
        {
            this.maxcount = size;
        }

        public virtual bool Offer(E e)
        {

            if (Count < maxcount)
            {
                base.Add(e);
                return true;
            }
            else if (base.Comparer.Compare(e, base.GetLast()) < 0)
            {
                base.RemoveLast();
                base.Add(e);
                return true;
            }
            return false;

        }

        public E Peek()
        {

            return base.GetFirst();
        }
        public E PeekLast()
        {
            return base.GetLast();
        }
        public virtual E Poll()
        {
            E tmp = base.GetFirst();
            base.RemoveFirst();
            return tmp;
        }

        /**
         * Remove the element at the given position.
         * 
         * @param pos Element position.
         */
        protected virtual E RemoveAt(int pos)
        {
            E tmp = default(E);
            for (int i = 0; i < base.Count; i++)
            {
                if (i == pos)
                {
                    tmp = base[i];
                    break;
                }
            }
            if (tmp.Equals(default(E)))
            {
                base.Remove(tmp);
            }
            return tmp;
        }

        /**
  * Return the heap as a sorted array list, by repeated polling. This will
  * empty the heap!
  * 
  * @return new array list
  */
        public List<E> ToSortedArrayList()
        {
            OrderedBag<E> newh = (OrderedBag<E>)this.Clone();
            List<E> ret = new List<E>(Count);
            while (newh.Count > 0)
            {
                ret.Add(newh.RemoveFirst());
            }
            return ret;
        }




        public bool AddAll(ICollection<E> c)
        {
            int addsize = c.Count;
            if (addsize <= 0)
            {
                return false;
            }

            foreach (E elem in c)
            {
                Offer(elem);
            }
            return true;
        }

    }
}
