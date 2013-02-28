using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    public class EmptyDbIds : IArrayStaticDbIds, ISetDbIds
    {

        internal EmptyDbIds()
        { }


        public  bool Contains(IDbIdRef o)
        {
            return false;
        }

        public int Count
        {
            get { return 0; }
        }


        public  bool IsEmpty()
        {
            return true;
        }


        public  IDbId Get(int i)
        {
            throw new IndexOutOfRangeException();
        }

        public  int BinarySearch(IDbIdRef key)
        {
            return -1; // Not found
        }


        public IEnumerator<IDbId> GetEnumerator()
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }

        public IDbId this[int index]
        {
            get
            {
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }


        public IArrayDbIds Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }
    }

}