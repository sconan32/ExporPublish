using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Generic
{

    public class UnmodifiableDbIds : IStaticDbIds
    {
        /**
         * The IDbIds we wrap.
         */
        private IDbIds inner;

        /**
         * Constructor.
         * 
         * @param inner Inner IDbId collection.
         */
        public UnmodifiableDbIds(IDbIds inner)
        {

            this.inner = inner;
        }


        public bool Contains(IDbIdRef o)
        {
            return inner.Contains(o);
        }


        public bool IsEmpty()
        {
            return inner.IsEmpty();
        }




        public int Count
        {
            get { return inner.Count; }
        }

        /**
         * Returns a string representation of the inner IDbId collection.
         */

        public override String ToString()
        {
            return inner.ToString();
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }

}
