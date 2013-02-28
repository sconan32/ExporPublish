using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Results
{

    public class OrderingFromDataStore<T> : BasicResult, IOrderingResult where T : IComparable<T>
    {
        /**
         * HashMap with object values
         */
        protected IDataStore<T> map;

        /**
         * Database IDs
         */
        protected IDbIds ids;

        /**
         * Comparator to use when sorting
         */
        protected IComparer<T> comparator;

        /**
         * Factor for ascending (+1) and descending (-1) ordering.
         */
        int ascending;

        /**
         * Constructor with comparator
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param ids DBIDs included
         * @param map data hash map
         * @param comparator comparator to use, may be null
         * @param descending ascending (false) or descending (true) order.
         */
        public OrderingFromDataStore(String name, String shortname, IDbIds ids, IDataStore<T> map,
            IComparer<T> comparator, bool descending) :
            base(name, shortname)
        {
            this.map = map;
            this.ids = ids;
            this.comparator = comparator;
            this.ascending = descending ? -1 : 1;
        }

        /**
         * Constructor without comparator
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param ids DBIDs included
         * @param map data hash map
         * @param descending ascending (false) or descending (true) order.
         */
        public OrderingFromDataStore(String name, String shortname, IDbIds ids,
            IDataStore<T> map, bool descending) :
            base(name, shortname)
        {
            this.map = map;
            this.ids = ids;
            this.comparator = null;
            this.ascending = descending ? -1 : 1;
        }

        /**
         * Minimal Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param ids DBIDs included
         * @param map data hash map
         */
        public OrderingFromDataStore(String name, String shortname, IDbIds ids, IDataStore<T> map) :
            base(name, shortname)
        {
            this.map = map;
            this.ids = ids;
            this.comparator = null;
            this.ascending = 1;
        }


        public  IDbIds GetDbIds()
        {
            return ids;
        }


        public  IArrayModifiableDbIds Iter(IDbIds ids)
        {
            IArrayModifiableDbIds sorted = DbIdUtil.NewArray(ids);
            if (comparator != null)
            {
                sorted.Sort(new DerivedComparator(map,comparator,ascending));
            }
            else
            {
                sorted.Sort(new ImpliedComparator(map,ascending));
            }
            return sorted;
        }

        /**
         * Internal comparator, accessing the map to sort objects
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        protected class ImpliedComparator : IComparer<IDbId>
        {

            IDataStore<T> map;
            int ascending;
            public ImpliedComparator(IDataStore<T> map, int ascending)
            {
                this.map = map;
                this.ascending = ascending;

            }
            public int Compare(IDbId id1, IDbId id2)
            {
                T k1 = (T)map[(id1)];
                T k2 = (T)map[(id2)];
                Debug.Assert(k1 != null);
                Debug.Assert(k2 != null);
                return ascending  * k1.CompareTo(k2);
            }
        }

        /**
         * Internal comparator, accessing the map but then using the provided
         * comparator to sort objects
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        protected class DerivedComparator : IComparer<IDbId>
        {

            IDataStore<T> map;
            IComparer<T> comparator;
            int ascending;
            public DerivedComparator(IDataStore<T> map, IComparer<T> comparator, int ascending)
            {
                this.map = map;
                this.comparator = comparator;
                this.ascending = ascending;
            }
            public int Compare(IDbId id1, IDbId id2)
            {
                T k1 = (T)map[id1];
                T k2 = (T)map[id2];
                Debug.Assert(k1 != null);
                Debug.Assert(k2 != null);
                return ascending * comparator.Compare(k1, k2);
            }
        }
    }
}
