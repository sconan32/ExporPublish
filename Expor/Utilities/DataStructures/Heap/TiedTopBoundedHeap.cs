using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class TiedTopBoundedHeap<E> : TopBoundedHeap<E>
    {
        /**
         * Serial version
         */
        //private static readonly long serialVersionUID = 1L;

        /**
         * List to keep ties in.
         */
        private IList<E> ties = new List<E>();

        /**
         * Constructor with comparator.
         * 
         * @param maxsize Maximum size of heap (unless tied)
         * @param comparator Comparator
         */
        public TiedTopBoundedHeap(int maxsize, IComparer<E> comparator)
            : base(maxsize, comparator)
        { }

        /**
         * Constructor for Comparable objects.
         * 
         * @param maxsize Maximum size of heap (unless tied)
         */
        public TiedTopBoundedHeap(int maxsize) :
            this(maxsize, comp: null)
        { }
        public TiedTopBoundedHeap(int maxsize, Comparison<E> comp)
            : base(maxsize, comp)
        { }

        public new int Count
        {
            get
            {
                return base.Count + ties.Count;
            }
        }


        public new void Clear()
        {
            base.Clear();
            ties.Clear();
        }

        public new bool Contains(E o)
        {
            return ties.Contains(o) || base.Contains(o);
        }



        public new E Peek()
        {
            if (ties.Count <= 0)
            {
                return base.Peek();
            }
            else
            {
                return ties[ties.Count - 1];
            }
        }


        public override E Poll()
        {
            if (ties.Count <= 0)
            {
                return base.Poll();
            }
            else
            {
                E tmp = ties[ties.Count - 1];

                ties.RemoveAt(ties.Count - 1);
                return tmp;
            }
        }

        protected override void handleOverflow(E e)
        {
            bool tied = false;
            if (base.Comparer == null)
            {

                IComparable<Object> c = (IComparable<Object>)e;
                if (c.CompareTo(base[0]) == 0)
                {
                    tied = true;
                }
            }
            else
            {
                if (base.Comparer.Compare(e, base[0]) == 0)
                {
                    tied = true;
                }
            }
            if (tied)
            {
                ties.Add(e);
            }
            else
            {
                // Also remove old ties.
                ties.Clear();
            }
        }
    }
}
