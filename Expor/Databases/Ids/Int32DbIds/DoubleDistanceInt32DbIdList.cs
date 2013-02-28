using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class to store double distance, integer DbId results.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses DoubleInt32ArrayQuickSort
     */
    public class DoubleDistanceInt32DbIdList : AbstractDoubleDistanceDbIdList<DoubleDistanceInt32DbIdPair>
    {
        protected List<DoubleDistanceInt32DbIdPair> store;
        /**
         * Constructor.
         */
        public DoubleDistanceInt32DbIdList()
        {

        }

        /**
         * Constructor.
         * 
         * @param size Initial size
         */
        public DoubleDistanceInt32DbIdList(int size)
        {
            store = new List<DoubleDistanceInt32DbIdPair>(size);
            // this.dists = new double[size];
            // this.ids = new int[size];
            // This is default anyway: this.size = 0;
        }


        //public Itr iter()
        //{
        //    return new Itr();
        //}


        public virtual bool Contains(IDbIdRef o)
        {

            int q = o.InternalGetIndex();
            return store.Count(t => t.Int32Id == q) > 0;

        }


        public virtual bool IsEmpty()
        {
            return store.Count == 0;
        }



        public virtual void Add(DoubleDistanceValue dist, IDbIdRef id)
        {
            Add(new DoubleDistanceInt32DbIdPair(dist.DoubleValue(), id.InternalGetIndex()));
        }


        public virtual void Add(double dist, IDbIdRef id)
        {
            Add(new DoubleDistanceInt32DbIdPair(dist, id.InternalGetIndex()));
        }


        public override void Add(DoubleDistanceInt32DbIdPair pair)
        {
           store. Add(pair);
        }
        public virtual void Sort()
        {
             store.Sort((t1, t2) => t1.DoubleDistance().CompareTo(t2.DoubleDistance()));
        }
        /**
         * Truncate the list to the given size.
         * 
         * @param newsize New size
         */
        public virtual void Truncate(int newsize)
        {
            store.Clear();
        }

        public override bool Remove(DoubleDistanceInt32DbIdPair pair)
        {
            return store.Remove(pair);
        }
        /**
         * Get the distance of the object at position pos.
         * 
         * Usually, you should be using an iterator instead. This part of the API is
         * not stable.
         * 
         * @param pos Position
         * @return Double distance.
         */
        public virtual double GetDoubleDistance(int pos)
        {
            return store[pos].DoubleDistance();
        }

        public override int Count
        {
            get { return store.Count; }
        }
        public override void Clear()
        {
            store.Clear();
        }
        public override DoubleDistanceInt32DbIdPair this[int index]
        {
            get { return store[index]; }
            set { store[index] = value; }
        }
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("DistanceDbIdList[");
            // for (DoubleDistanceDbIdListIter iter = this.iter(); iter.valid(); )
            foreach (var iter in this)
            {
                var it = iter as IDoubleDistanceDbIdPair;
                buf.Append(it.DoubleDistance()).Append(':').Append(iter.InternalGetIndex());
                buf.Append(',');

            }
            buf.Append(']');
            return buf.ToString();
        }


        public override bool Contains(DoubleDistanceInt32DbIdPair item)
        {
            return store.Contains(item);
        }


    }

}
