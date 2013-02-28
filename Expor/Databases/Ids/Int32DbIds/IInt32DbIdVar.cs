using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    public class Int32DbIdVar : IDbIdVar, IDbIds
    {
        /**
         * The actual value.
         */
        int id;

        /**
         * Constructor.
         */
        internal Int32DbIdVar()
        {
            this.id = -1;
        }

        /**
         * Constructor.
         * 
         * @param val
         */
        internal Int32DbIdVar(IDbIdRef val)
        {
            this.id = val.InternalGetIndex();
        }

        public int InternalGetIndex()
        {
            return id;
        }

        /**
         * Internal set to integer.
         * 
         * @param i integer value
         */
        internal void InternalSetIndex(int i)
        {
            id = i;
        }


        public void Set(IDbIdRef dbidref)
        {
            id = dbidref.InternalGetIndex();
        }


        public IDbId this[int i]
        {
            get
            {
                if (i != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return new Int32DbId(i);
            }
            set { throw new InvalidOperationException(); }
        }


        public int Count
        {
            get { return 1; }
        }


        public bool IsEmpty()
        {
            return false;
        }


        public int BinarySearch(IDbIdRef key)
        {
            int other = key.InternalGetIndex();
            return (other == id) ? 0 : (other < id) ? -1 : -2;
        }


        public bool Contains(IDbIdRef o)
        {
            return id == o.InternalGetIndex();
        }


        public void AssignVar(int i, IDbIdVar var)
        {
            if (var is Int32DbIdVar)
            {
                ((Int32DbIdVar)var).InternalSetIndex(i);
            }
            else
            {
                // Much less efficient:
                var.Set(this[(i)]);
            }
        }


        public IArrayDbIds Slice(int begin, int end)
        {
            if (begin == 0 && end == 1)
            {
                return this;
            }
            else
            {
                return DbIdUtil.EMPTYDBIDS;
            }
        }


        public override String ToString()
        {
            return id.ToString();
        }



        IDbId IDbIdRef.DbId
        {
            get { throw new NotImplementedException(); }
        }

        int IDbIdRef.Int32Id
        {
            get { throw new NotImplementedException(); }
        }

        bool IDbIdRef.IsSameDbId(IDbIdRef other)
        {
            throw new NotImplementedException();
        }

        int IDbIdRef.CompareDbId(IDbIdRef other)
        {
            throw new NotImplementedException();
        }

        int IDbIdRef.InternalGetIndex()
        {
            throw new NotImplementedException();
        }



        IArrayDbIds IArrayDbIds.Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }



        public IEnumerator<IDbId> GetEnumerator()
        {
            yield return this[0];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
