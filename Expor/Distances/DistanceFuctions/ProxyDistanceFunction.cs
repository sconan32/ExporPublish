using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public class ProxyDistanceFunction<O> : AbstractDbIdDistanceFunction
    {
        /**
         * Distance query
         */
        IDistanceQuery inner;

        /**
         * Constructor
         * 
         * @param inner Inner distance
         */
        public ProxyDistanceFunction(IDistanceQuery inner)
            : base()
        {

            this.inner = inner;
        }

        /**
         * Static method version.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param inner Inner distance query
         * @return Proxy object
         */
        public static ProxyDistanceFunction<T> Proxy<T>(IDistanceQuery inner)
        {
            return new ProxyDistanceFunction<T>(inner);
        }


        public override IDistanceValue Distance(IDbIdRef o1, IDbIdRef o2)
        {
            return inner.Distance(o1, o2);
        }


        public override IDistanceValue DistanceFactory
        {
            get { return inner.DistanceFactory; }
        }

        /**
         * Get the inner query
         * 
         * @return query
         */
        public IDistanceQuery GetDistanceQuery()
        {
            return inner;
        }

        /**
         * @param inner the inner distance query to set
         */
        public void SetDistanceQuery(IDistanceQuery inner)
        {
            this.inner = inner;
        }

        /**
         * Helper function, to resolve any wrapped Proxy Distances
         * 
         * @param <V> Object type
         * @param <D> Distance type
         * @param dfun Distance function to unwrap.
         * @return unwrapped distance function
         */

        public static IDistanceFunction UnwrapDistance(IDistanceFunction dfun)
        {
            if (typeof(ProxyDistanceFunction<O>).IsInstanceOfType(dfun))
            {
                return UnwrapDistance(((ProxyDistanceFunction<O>)dfun).GetDistanceQuery().DistanceFunction);
            }
            return dfun;
        }



        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            ProxyDistanceFunction<O> other = (ProxyDistanceFunction<O>)obj;
            return this.inner.Equals(other.inner);
        }


        public override int GetHashCode()
        {
            return this.inner.GetHashCode();
        }
    }
}
