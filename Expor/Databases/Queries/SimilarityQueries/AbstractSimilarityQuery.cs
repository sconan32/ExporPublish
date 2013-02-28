using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.SimilarityQueries
{

    public abstract class AbstractSimilarityQuery<O> : AbstractDataBasedQuery, ISimilarityQuery
    {
        /**
         * Constructor.
         * 
         * @param relation Relation to use.
         */
        public AbstractSimilarityQuery(IRelation relation) :
            base(relation)
        {
        }

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param id1 first object id
         * @param id2 second object id
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Similarity(IDbIdRef id1, IDbIdRef id2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param o1 first object
         * @param id2 second object id
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Similarity(O o1, IDbIdRef id2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param id1 first object id
         * @param o2 second object
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Similarity(IDbIdRef id1, O o2);

        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param o1 first object
         * @param o2 second object
         * @return the distance between the two objects specified by their object ids
         */

        public abstract IDistanceValue Similarity(O o1, O o2);

        public abstract IDistanceValue DistanceFactory { get; }

        IDistanceValue ISimilarityQuery.Similarity(IDataVector o1, IDataVector o2)
        {
            return this.Similarity((O)o1, (O)o2);
        }
        IDistanceValue ISimilarityQuery.Similarity(IDbIdRef id, IDataVector o2)
        {
            return this.Similarity(id, (O)o2);
        }
        IDistanceValue ISimilarityQuery.Similarity(IDataVector o1, IDbIdRef id)
        {
            return this.Similarity((O)o1, id);
        }

    }
}
