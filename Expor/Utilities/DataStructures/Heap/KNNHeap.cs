using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class KNNHeap : TiedTopBoundedHeap<IDistanceResultPair>
   
    {
        /**
         * Serial version
         */
      //  private static readonly long serialVersionUID = 1L;

        /**
         * Maximum distance, usually infiniteDistance
         */
        private readonly IDistanceValue maxdist;

        /**
         * Constructor.
         * 
         * @param k k Parameter
         * @param maxdist k-distance to return for less than k neighbors - usually
         *        infiniteDistance
         */
        public KNNHeap(int k,IDistanceValue maxdist)
            : base(k, new Comp())
        {
            this.maxdist = maxdist;
        }

        /**
         * Simplified constructor. Will return {@code null} as kNN distance with less
         * than k entries.
         * 
         * @param k k Parameter
         */
        public KNNHeap(int k) :
            this(k, default(IDistanceValue))
        {
        }



        /**
         * Serialize to a {@link KNNList}. This empties the heap!
         * 
         * @return KNNList with the heaps contents.
         */
        public KNNList ToKNNList()
        {
            return new KNNList(this);
        }

        /**
         * Get the K parameter ("maxsize" internally).
         * 
         * @return K
         */
        public int GetK()
        {
            return base.MaxSize;
        }

        /**
         * Get the distance to the k nearest neighbor, or maxdist otherwise.
         * 
         * @return Maximum distance
         */
        public IDistanceValue GetKNNDistance()
        {
            if (Count < GetK())
            {
                return maxdist;
            }
            return Peek().GetDistance();
        }

        /**
         * Get maximum distance in heap
         */
        public IDistanceValue GetMaximumDistance()
        {
            if (Count <= 0)
            {
                return maxdist;
            }
            return Peek().GetDistance();
        }

        /**
         * Add a distance-id pair to the heap unless the distance is too large.
         * 
         * Compared to the super.add() method, this often saves the pair construction.
         * 
         * @param distance Distance value
         * @param id ID number
         * @return success code
         */
        public bool Add(IDistanceValue distance, IDbIdRef id)
        {
            if (Count < maxsize || Peek().GetDistance().CompareTo(distance) >= 0)
            {
                base.Add(new GenericDistanceResultPair(distance, id.DbId));
            }
            return true; /* "success" */
        }

        /**
         * Comparator to use.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Comp : IComparer<IDistanceResultPair>
        {

            public int Compare(IDistanceResultPair o1, IDistanceResultPair o2)
            {
                return -o1.CompareByDistance(o2);
            }
        }
    }
}
