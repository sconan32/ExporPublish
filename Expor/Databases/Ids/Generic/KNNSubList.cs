using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Distances.DistanceValues;


namespace Socona.Expor.Databases.Ids.Generic
{

    /**
     * Sublist of an existing result to contain only the first k elements.
     * 
     * @author Erich Schubert
     * 
     * @param <D> Distance
     */
    public class KNNSubList : AbstractReadOnlyKnnList<IDistanceDbIdPair>
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
        private IKNNList inner;

        /**
         * Constructor.
         * 
         * @param inner Inner instance
         * @param k k value
         */
        public KNNSubList(IKNNList inner, int k)
        {
            this.inner = inner;
            this.k = k;
            // Compute list size
            // TODO: optimize for double distances.
            {
                IDistanceDbIdPair dist = inner[(k)];
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


        public override IDistanceDbIdPair this[int index]
        {
            get
            {
                Debug.Assert(index < size, "Access beyond design size of list.");
                return inner[(index)];
            }
        }


        public override IDistanceValue KNNDistance
        {
            get { return inner[(k)].Distance; }
        }


        // public  DistanceDbIdListIter<D> iter() {
        //    return new Itr();
        // }


        public bool Contains(IDbIdRef o)
        {
            //for (DbIdIter iter = iter(); iter.valid(); iter.advance()) {
            foreach (var iter in this)
            {
                if (DbIdUtil.IsEqual(iter, o))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Contains(IDistanceDbIdPair item)
        {
            return inner.Contains(item);
        }
        public bool IsEmpty()
        {
            return size == 0;
        }



        public override int Count
        {
            get { return size; }
        }
    }

}
