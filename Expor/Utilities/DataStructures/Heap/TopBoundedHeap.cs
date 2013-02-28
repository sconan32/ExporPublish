using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class TopBoundedHeap<E> : Heap<E>
    {
        /**
         * Serial version
         */
        //private static readonly long serialVersionUID = 1L;

        /**
         * Maximum size
         */
        protected int maxsize;

        /**
         * Constructor with maximum size only.
         * 
         * @param maxsize Maximum size
         */
        public TopBoundedHeap(int maxsize) :
            this(maxsize,comp: null)
        { }

        /**
         * Constructor with maximum size and {@link Comparator}.
         * 
         * @param maxsize Maximum size
         * @param comparator Comparator
         */
        public TopBoundedHeap(int maxsize, IComparer<E> comparator)
            : base(maxsize + 1, comparator)
        {
            this.maxsize = maxsize;
            Debug.Assert(maxsize > 0);
        }

        public TopBoundedHeap(int maxsize, Comparison<E> comp)
            : base(maxsize + 1, comp)
        { }

        public override bool Offer(E e)
        {
            // don't add if we hit maxsize and are worse
            if (base.Count >= maxsize)
            {



            }
            bool result = base.Offer(e);
            // purge unneeded entry(s)
            while (base.Count > maxsize)
            {
                handleOverflow(base.Poll());
            }
            return result;
        }

        /**
         * Handle an overflow in the structure. This function can be overridden to get
         * overflow treatment.
         * 
         * @param e Overflowing element.
         */
        protected virtual void handleOverflow(E e)
        {
            // discard extra element
        }

        /**
         * @return the maximum size
         */
        public int MaxSize
        {
            get
            {
                return maxsize;
            }
        }
    }
}
