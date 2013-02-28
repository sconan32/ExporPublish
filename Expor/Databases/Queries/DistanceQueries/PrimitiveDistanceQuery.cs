using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
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
     * @apiviz.landmark
     * @apiviz.uses PrimitiveDistanceFunction
     * 
     * @param O Database object type.
     * @param D Distance result type.
     */

    public class PrimitiveDistanceQuery<O> : AbstractDistanceQuery<O>
    {
        /**
         * The distance function we use.
         */

        protected readonly IPrimitiveDistanceFunction<O> distanceFunction;

        /**
         * Constructor.
         * 
         * @param relation Representation to use.
         * @param distanceFunction Our distance function
         */
        public PrimitiveDistanceQuery(IRelation relation, IPrimitiveDistanceFunction<O> distanceFunction)
            : base(relation)
        {

            this.distanceFunction = distanceFunction;
        }


        public override IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
        {
            O o1 = (O)relation[(id1)];
            O o2 = (O)relation[(id2)];
            return Distance(o1, o2);
        }


        public override IDistanceValue Distance(O o1, IDbIdRef id2)
        {
            O o2 = (O)relation[id2];
            return Distance(o1, o2);
        }


        public override IDistanceValue Distance(IDbIdRef id1, O o2)
        {
            O o1 = (O)relation[id1];
            return Distance(o1, o2);
        }


        public override IDistanceValue Distance(O o1, O o2)
        {
            if (o1 == null)
            {
                throw new InvalidOperationException("This distance function can only be used for object instances.");
            }
            if (o2 == null)
            {
                throw new InvalidOperationException("This distance function can only be used for object instances.");
            }
            return distanceFunction.Distance(o1, o2);
        }


        public override IDistanceFunction DistanceFunction
        {
            get { return distanceFunction; }
        }
    }
}
