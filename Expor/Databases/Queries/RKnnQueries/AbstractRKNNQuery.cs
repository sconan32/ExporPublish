using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RKnnQueries
{

    public abstract class AbstractRKNNQuery<O> : AbstractDataBasedQuery, IRKNNQuery
        where O : IDataVector
    {
        /// <summary>
        /// Hold the distance function to be used.
        /// </summary>
        protected readonly IDistanceQuery distanceQuery;


        public AbstractRKNNQuery(IDistanceQuery distanceQuery) :
            base(distanceQuery.Relation)
        {
            this.distanceQuery = distanceQuery;
        }

        public abstract IList<IDistanceResultPair> GetRKNNForDbId(IDbIdRef id, int k);


        public abstract IList<IDistanceResultPair> GetRKNNForObject(O obj, int k);


        public abstract IList<IList<IDistanceResultPair>> GetRKNNForBulkDbIds(IArrayDbIds ids, int k);

        IList<IDistanceResultPair> IRKNNQuery.GetRKNNForObject(IDataVector obj, int k)
        {
            return this.GetRKNNForObject((O)obj, k);
        }

    }
}
