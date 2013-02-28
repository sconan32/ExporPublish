using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Data;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{

    /**
     * A distance query serves as adapter layer for database and primitive
     * distances.
     * 
     * @author Erich Schubert
     * 
     * @param O Input object type
     * @param D Distance result type
     */
    public abstract class AbstractDistanceQuery<O> : AbstractDataBasedQuery, IDistanceQuery
    {
        /**
         * Constructor.
         * 
         * @param relation Relation to use.
         */
        public AbstractDistanceQuery(IRelation relation)
            : base(relation)
        {

        }

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param id1 first object id
         * @param id2 second object id
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param o1 first object
         * @param id2 second object id
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Distance(O o1, IDbIdRef id2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param id1 first object id
         * @param o2 second object
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Distance(IDbIdRef id1, O o2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param o1 first object
         * @param o2 second object
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Distance(O o1, O o2);

        public IDistanceValue DistanceFactory
        {
            get { return DistanceFunction.DistanceFactory; }
        }

        /**
         * Provides an infinite distance.
         * 
         * @return an infinite distance
         */

        public IDistanceValue Infinity
        {
            get { return (IDistanceValue)DistanceFunction.DistanceFactory.Infinity; }
        }

        /**
         * Provides a null distance.
         * 
         * @return a null distance
         */

        public IDistanceValue Empty
        {
            get { return (IDistanceValue)DistanceFunction.DistanceFactory.Empty; }
        }

        /**
         * Provides an undefined distance.
         * 
         * @return an undefined distance
         */

        public IDistanceValue Undefined
        {
            get { return (IDistanceValue)DistanceFunction.DistanceFactory.Undefined; }
        }


        public abstract IDistanceFunction DistanceFunction { get; }

        IDistanceValue IDistanceQuery.Distance(IDataVector obj1,IDataVector obj2)
        {
            return this.Distance((O)obj1, (O)obj2);
        }
        IDistanceValue IDistanceQuery.Distance(IDbIdRef id,IDataVector obj2)
        {
            return this.Distance(id, (O)obj2);
        }
        IDistanceValue IDistanceQuery.Distance(IDataVector obj1, IDbIdRef id)
        {
            return this.Distance((O)obj1, id);
        }

    }
}
