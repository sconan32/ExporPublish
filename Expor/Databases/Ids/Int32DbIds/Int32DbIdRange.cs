using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    class Int32DbIdRange : IDbIdRange
    {
        /**
         * Start value
         */
        private readonly int start;

        internal int Start
        {
            get { return start; }
        }
        internal int End
        {
            get { return start + len; }
        }

        /**
         * Length value
         */
        protected readonly int len;

        /**
         * Constructor.
         * 
         * @param start Range start
         * @param len Range length
         */
        public Int32DbIdRange(int start, int len)
            : base()
        {
            this.start = start;
            this.len = len;
        }



        public bool Contains(IDbIdRef o)
        {
            int oid = o.Int32Id;
            if (oid < start)
                return false;
            if (oid >= start + len)
            {
                return false;
            }
            return true;
        }


        /**
         * For storage array offsets.
         * 
         * @param dbid
         * @return array offset
         */

        public int GetOffset(IDbIdRef dbid)
        {
            return dbid.Int32Id - start;
        }


        public int BinarySearch(IDbIdRef key)
        {
            int keyid = key.Int32Id;
            if (keyid < start)
            {
                return -1;
            }
            int off = keyid - start;
            if (off < len)
            {
                return off;
            }
            return -(len + 1);
        }

        public IDbId Get(int i)
        {
            if (i > len || i < 0)
            {
                throw new IndexOutOfRangeException();
            }
            return DbIdFactoryBase.FACTORY.ImportInt32(start + i);
        }

        public int Count
        {
            get { return len; }
        }

        public bool IsEmpty()
        {
            return len <= 0;
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new Int32DbId(start + i);
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
                if (index > len || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return DbIdFactoryBase.FACTORY.ImportInt32(start + index);
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
