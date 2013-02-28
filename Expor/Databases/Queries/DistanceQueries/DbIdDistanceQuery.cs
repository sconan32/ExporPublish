using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{

    public class DbIdDistanceQuery : AbstractDatabaseDistanceQuery<IDbId>
    {
        /**
         * The distance function we use.
         */
        protected IDbIdDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param relation Database to use.
         * @param distanceFunction Our distance function
         */
        public DbIdDistanceQuery(IRelation relation, IDbIdDistanceFunction distanceFunction) :
            base(relation)
        {
            this.distanceFunction = distanceFunction;
        }


        public override  IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
        {
            if (id1 == null)
            {
                throw new InvalidOperationException(
                    "This distance function can only be used for objects stored in the database.");
            }
            if (id2 == null)
            {
                throw new InvalidOperationException(
                    "This distance function can only be used for objects stored in the database.");
            }
            return distanceFunction.Distance(id1, id2);
        }


        public override IDistanceFunction DistanceFunction
        {
            get { return distanceFunction; }
        }
    }
}
