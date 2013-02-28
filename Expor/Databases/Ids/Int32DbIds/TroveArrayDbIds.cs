using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    public abstract class TroveArrayDbIds : IArrayDbIds
    {
        /**
         * Get the array store
         * 
         * @return the store
         */
        abstract protected IList<int> GetStore();




        public IDbId Get(int i)
        {
            return new Int32DbId(GetStore()[i]);
        }

        public int BinarySearch(IDbIdRef key)
        {
            return GetStore().IndexOf(key.Int32Id);
        }

        public int Count
        {
            get { return GetStore().Count; }
        }

        public bool Contains(IDbIdRef o)
        {
            return GetStore().Contains(o.Int32Id);
        }
        public bool IsEmpty()
        {
            return GetStore().Count <= 0;
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            foreach (var id in GetStore())
            {
                yield return new Int32DbId(id);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IDbId this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                GetStore()[index] = value.Int32Id;
            }
        }

        public IArrayDbIds Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }
    }
}
