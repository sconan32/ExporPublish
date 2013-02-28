using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.SimilarityQueries
{

    public abstract class AbstractDbIdSimilarityQuery<O> : AbstractSimilarityQuery<O>
    {
        /**
         * Constructor.
         * 
         * @param relation Relation to use.
         */
        public AbstractDbIdSimilarityQuery(IRelation relation) :
            base(relation)
        {
        }


        public override IDistanceValue Similarity(O o1, IDbIdRef id2)
        {
            throw new InvalidOperationException(
                "This distance function can only be used for objects when referenced by ID.");
        }


        public override IDistanceValue Similarity(IDbIdRef id1, O o2)
        {
            throw new InvalidOperationException(
                "This distance function can only be used for objects when referenced by ID.");
        }


        public override IDistanceValue Similarity(O o1, O o2)
        {
            throw new InvalidOperationException(
                "This distance function can only be used for objects when referenced by ID.");
        }
    }
}
