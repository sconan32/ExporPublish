using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceResultLists;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Default class to keep a list of distance-object pairs.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf DoubleDistanceDbIdPair
     * @apiviz.has DoubleDistanceDbIdListIter
     */
    public class DoubleDistanceDbIdPairList : AbstractDoubleDistanceDbIdList<IDoubleDistanceDbIdPair>
    {
        /**
         * Actual storage.
         */
        List<IDoubleDistanceDbIdPair> storage;

        /**
         * Constructor.
         */
        public DoubleDistanceDbIdPairList()
        {
            storage = new List<IDoubleDistanceDbIdPair>();
        }

        /**
         * Constructor.
         * 
         * @param initialCapacity Capacity
         */
        public DoubleDistanceDbIdPairList(int initialCapacity)
        {
            storage = new List<IDoubleDistanceDbIdPair>(initialCapacity);
        }

        /**
         * Add an element.
         * 
         * @deprecated Pass a double value instead.
         * 
         * @param dist Distance
         * @param id ID
         */

        public virtual void Add(DoubleDistanceValue dist, IDbIdRef id)
        {
            storage.Add(DbIdFactoryBase.FACTORY.NewDistancePair(dist.DoubleValue(), id));
        }

        /**
         * Add an element.
         * 
         * @param dist Distance
         * @param id ID
         */

        public virtual void Add(double dist, IDbIdRef id)
        {
            storage.Add(DbIdFactoryBase.FACTORY.NewDistancePair(dist, id));
        }

        /**
         * Add an element.
         * 
         * @param pair Pair to add
         */

        public override void Add(IDoubleDistanceDbIdPair pair)
        {
            storage.Add(pair);
        }


        public override void Clear()
        {
            storage.Clear();
        }


        public virtual void Sort()
        {
            storage.Sort(DistanceDbIdResultUtil.DistanceComparator());
        }


        public override int Count
        {
            get { return storage.Count; }
        }


        public override IDoubleDistanceDbIdPair this[int off]
        {
            get { return storage[(off)]; }
            set { storage[off] = value; }
        }




        public virtual bool Contains(IDbIdRef o)
        {
            //for(DbIdIter iter = iter(); iter.valid(); iter.advance()) {
            foreach (var iter in this)
            {
                if (DbIdUtil.IsEqual(iter, o))
                {
                    return true;
                }
            }
            return false;
        }


        public virtual bool IsEmpty()
        {
            return Count == 0;
        }


        public override String ToString()
        {
            return DistanceDbIdResultUtil.ToString(this);
        }
        public override bool Contains(IDoubleDistanceDbIdPair item)
        {
            return storage.Contains(item);

        }
        public override bool Remove(IDoubleDistanceDbIdPair pair)
        {
            return storage.Remove(pair);
        }
    }
}
