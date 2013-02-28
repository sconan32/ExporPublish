using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceResultLists;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Ids.Generic
{

    /**
     * Heap used for KNN management.
     * 
     * @author Erich Schubert
     * 
     * @param <P> pair type
     * @param <D> distance type
     */
    public abstract class AbstractKNNHeap : IKNNHeap
    {
        /**
         * The actual heap.
         */
        protected TiedTopBoundedHeap<IDistanceDbIdPair> heap;

        /**
         * Constructor.
         * 
         * @param k Maximum heap size (unless tied)
         */
        public AbstractKNNHeap(int k)
        {
            heap = new TiedTopBoundedHeap<IDistanceDbIdPair>(k, DistanceDbIdResultUtil.BY_REVERSE_DISTANCE);
        }

        /**
         * Add a pair to the heap.
         * 
         * @param pair Pair to add.
         */
        public abstract void Insert(IDistanceDbIdPair pair);


        public virtual int K
        {
            get { return heap.MaxSize; }
        }


        public virtual int Count
        {
            get { return heap.Count; }
        }


        public virtual IDistanceDbIdPair Peek()
        {
            return heap.Peek();
        }


        public virtual bool IsEmpty()
        {
            return heap.Count <= 0;
        }


        public virtual void Clear()
        {
            heap.Clear();
        }


        public virtual IDistanceDbIdPair Poll()
        {
            return heap.Poll();
        }

        public abstract IKNNList ToKNNList();


        public abstract IDistanceValue KNNDistance
        {
            get;
        }

        public abstract void Insert(Distances.DistanceValues.IDistanceValue distance, IDbIdRef id);



    }
}
