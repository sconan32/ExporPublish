using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{

    /**
     * Run a database query in a database context.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Database object type.
     * @param <D> Distance result type.
     */
    public abstract class AbstractDatabaseDistanceQuery<O> : AbstractDistanceQuery<O>
    {
        /**
         * Constructor.
         * 
         * @param relation Relation to use.
         */
        public AbstractDatabaseDistanceQuery(IRelation relation) :
            base(relation)
        {
        }


        public override IDistanceValue Distance(O o1, IDbIdRef id2)
        {
            if (o1 is IDbId)
            {
                return Distance((IDbId)o1, id2);
            }
            throw new InvalidOperationException("This distance function is only defined for known DBIDs.");
        }


        public override IDistanceValue Distance(IDbIdRef id1, O o2)
        {
            if (o2 is IDbId)
            {
                return Distance(id1, (IDbId)o2);
            }
            throw new InvalidOperationException("This distance function is only defined for known DBIDs.");
        }


        public override IDistanceValue Distance(O o1, O o2)
        {
            if (o1 is IDbId && o2 is IDbId)
            {
                return Distance((IDbId)o1, (IDbId)o2);
            }
            throw new InvalidOperationException("This distance function is only defined for known DBIDs.");
        }

    }
}
