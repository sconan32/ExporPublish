using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Generic
{

    public class UnmodifiableArrayDbIds : IArrayStaticDbIds
    {
        /**
         * The DbIds we wrap.
         */
        private IArrayDbIds inner;

        /**
         * Constructor.
         * 
         * @param inner Inner IDbId collection.
         */
        public UnmodifiableArrayDbIds(IArrayDbIds inner)
        {

            this.inner = inner;
        }


        public  bool Contains(IDbIdRef o)
        {
            return inner.Contains(o);
        }


        public  bool IsEmpty()
        {
            return inner.IsEmpty();
        }


        public  int Count
        {
            get
            {
                return inner.Count;
            }
        }

        /**
         * Returns a string representation of the inner IDbId collection.
         */

        public override String ToString()
        {
            return inner.ToString();
        }


        public  IDbId Get(int i)
        {
            return inner[i];
        }


        public int BinarySearch(IDbIdRef key)
        {
            return inner.BinarySearch(key);
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public IDbId this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public IArrayDbIds Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }
    }
}
