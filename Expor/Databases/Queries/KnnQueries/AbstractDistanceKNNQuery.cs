using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public abstract class AbstractDistanceKNNQuery<O> : AbstractDataBasedQuery, IKNNQuery
    {
        /**
         * Hold the distance function to be used.
         */
        protected IDistanceQuery distanceQuery;

        /**
         * Constructor.
         * 
         * @param distanceQuery Distance query used
         */
        public AbstractDistanceKNNQuery(IDistanceQuery distanceQuery) :
            base(distanceQuery.Relation)
        {
            this.distanceQuery = distanceQuery;
        }


        abstract public IKNNList GetKNNForDbId(IDbIdRef id, int k);


        abstract public IKNNList GetKNNForObject(O obj, int k);


        public abstract IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k);

        public abstract void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps);

        IKNNList IKNNQuery.GetKNNForObject(IDataVector obj, int k)
        {
            return this.GetKNNForObject((O)obj, k);
        }
    }
}
