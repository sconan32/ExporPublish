using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Pair containing a double value and an integer DBID.
     * 
     * @author Erich Schubert
     */
    class Int32DoubleDbIdPair : IDoubleDbIdPair, IDbIdRef
    {
        /**
         * The double value.
         */
        double value;

        /**
         * The DB id.
         */
        int id;

        /**
         * Constructor.
         *
         * @param value Double value
         * @param id DBID
         */
        internal Int32DoubleDbIdPair(double value, int id)
        {

            this.value = value;
            this.id = id;
        }


        public int InternalGetIndex()
        {
            return id;
        }


        public int CompareTo(IDoubleDbIdPair o)
        {
            return value.CompareTo(o.DoubleValue());
        }


        public double DoubleValue()
        {
            return value;
        }


        public Double First
        {
            get { return (value); }
            set { throw new InvalidOperationException(); }
        }


        public IDbId Second
        {
            get { return new Int32DbId(id); }
            set { throw new InvalidOperationException(); }
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
    }

}
