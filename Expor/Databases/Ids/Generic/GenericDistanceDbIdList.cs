using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Generic
{

    public class GenericDistanceDbIdList : AbstractDistanceDbIdList<IDistanceDbIdPair>, IDistanceDbIdList
    {
        /**
         * Serialization Version
         */
        private List<IDistanceDbIdPair> store;

        /**
         * Constructor.
         */
        public GenericDistanceDbIdList()
        {
            store = new List<IDistanceDbIdPair>();
        }

        /**
         * Constructor.
         * 
         * @param c existing collection
         */
        public GenericDistanceDbIdList(IEnumerable<IDistanceDbIdPair> c)
        {
            store = new List<IDistanceDbIdPair>(c);
        }

        /**
         * Constructor.
         * 
         * @param initialCapacity Capacity
         */
        public GenericDistanceDbIdList(int initialCapacity)
        {
            store = new List<IDistanceDbIdPair>(initialCapacity);
        }

        public void Add(IDistanceValue distance, IDbIdRef id)
        {
            store.Add(new DistanceInt32DbIdPair(distance, id.Int32Id));
        }


        public bool Contains(IDbIdRef o)
        {
            return (this as IEnumerable<IDistanceDbIdPair>).Select(v => v.DbId).Contains(o);
        }

        public bool IsEmpty()
        {
            return store.Count <= 0;
        }
        public void Sort()
        {
            store.Sort((v1, v2) => v1.Distance.CompareTo(v2.Distance));
        }


        public override IDistanceDbIdPair this[int off]
        {
            get { return store[off]; }
            set { store[off] = value; }
        }

        public override int Count
        {
            get { return store.Count; }
        }

        public override void Add(IDistanceDbIdPair pair)
        {
            store.Add(pair);
        }

        public override void Clear()
        {
            store.Clear();
        }

        public override bool Contains(IDistanceDbIdPair item)
        {
            return store.Contains(item);
        }

        public override bool Remove(IDistanceDbIdPair item)
        {
            return store.Remove(item);
        }
    }
}
