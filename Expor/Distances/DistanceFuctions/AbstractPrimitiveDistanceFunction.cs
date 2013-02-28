using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;

namespace Socona.Expor.Distances.DistanceFuctions
{

    /**
     * AbstractDistanceFunction provides some methods valid for any extending class.
     * 
     * @author Arthur Zimek
     * 
     * @param <O> the type of objects to compute the distances in between
     * @param <D> the type of Distance used
     */
    public abstract class AbstractPrimitiveDistanceFunction<O> : IPrimitiveDistanceFunction<O>
    {
        /*
         * Provides an abstract DistanceFunction.
         */
        public AbstractPrimitiveDistanceFunction()
        {
            // EMPTY
        }




        /*
         * Instantiate with a database to get the actual distance query.
         * 
         * @param relation Representation
         * @return Actual distance query.
         */

        public virtual IDistanceQuery Instantiate(IRelation relation)
        {
            return new PrimitiveDistanceQuery<O>(relation, (IPrimitiveDistanceFunction<O>)this);
        }

        abstract public IDistanceValue Distance(O o1, O o2);


        public abstract ITypeInformation GetInputTypeRestriction();


        abstract public IDistanceValue DistanceFactory
        { get; }

        public virtual bool IsSymmetric
        {
            get { return true; }
        }

        public virtual bool IsMetric
        {
            get { return false; }
        }

    }
}
