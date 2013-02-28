using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RangeQueries
{

    public abstract class AbstractDistanceRangeQuery<O> : AbstractDataBasedQuery, IRangeQuery
        where O : IDataVector
    {
        /**
         * Hold the distance function to be used.
         */
        protected IDistanceQuery distanceQuery;

        /**
         * Constructor.
         * 
         * @param distanceQuery Distance query
         */
        public AbstractDistanceRangeQuery(IDistanceQuery distanceQuery)
            : base(distanceQuery.Relation)
        {
            this.distanceQuery = distanceQuery;
        }


        abstract public IDistanceDbIdList GetRangeForDbId(IDbIdRef id, IDistanceValue range);


        abstract public IDistanceDbIdList GetRangeForObject(O obj, IDistanceValue range);
        IDistanceDbIdList IRangeQuery.GetRangeForObject(IDataVector obj, IDistanceValue range)
        {
            return this.GetRangeForObject((O)obj, range);
        }
    }
}
