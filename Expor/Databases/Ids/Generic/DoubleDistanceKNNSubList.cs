using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Generic
{

    /**
     * Sublist of an existing result to contain only the first k elements.
     * 
     * TOOD: can be optimized slightly better.
     * 
     * @author Erich Schubert
     */
    public class DoubleDistanceKNNSubList : AbstractReadOnlyKnnList<IDoubleDistanceDbIdPair>
    {
        /**
         * Parameter k.
         */
        private int k;

        /**
         * Actual size, including ties.
         */
        private int size;

        /**
         * Wrapped inner result.
         */
        private IDoubleDistanceKNNList inner;

        /**
         * Constructor.
         * 
         * @param inner Inner instance
         * @param k k value
         */
        public DoubleDistanceKNNSubList(IDoubleDistanceKNNList inner, int k)
        {
            this.inner = inner;
            this.k = k;
            // Compute list size
            {
                IDoubleDistanceDbIdPair dist = inner[(k)] as IDoubleDistanceDbIdPair;
                int i = k;
                while (i + 1 < inner.Count)
                {
                    if (dist.CompareByDistance(inner[(i + 1)]) < 0)
                    {
                        break;
                    }
                    i++;
                }
                size = i;
            }
        }


        public override int K
        {
            get { return k; }
        }


        public override IDoubleDistanceDbIdPair this[int index]
        {
            get
            {
                Debug.Assert(index < size, "Access beyond design size of list.");
                return inner[(index)] as IDoubleDistanceDbIdPair;
            }
        
        }

        public double DoubleKNNDistance
        {
            get { return (inner[(k)] as IDoubleDistanceDbIdPair).DoubleDistance(); }
        }




        public bool Contains(IDbIdRef o)
        {
            // for (DBIDIter iter = iter(); iter.valid(); iter.advance())
            foreach (var iter in this)
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
            return size == 0;
        }


        public override int Count
        {
            get { return size; }
        }





        public override IDistanceValue KNNDistance
        {
            get { return inner[k].Distance; }
        }

        public override bool Contains(IDoubleDistanceDbIdPair item)
        {
            return Contains(item.DbId);
        }
    }

}
