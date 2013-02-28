using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public abstract class AbstractDbIdDistanceFunction : IDbIdDistanceFunction
    {
        /**
         * Provides an abstract DistanceFunction.
         */
        protected AbstractDbIdDistanceFunction()
        {
            // Empty
        }


        abstract public IDistanceValue Distance(IDbIdRef o1, IDbIdRef o2);


        abstract public IDistanceValue DistanceFactory { get; }


        public bool IsSymmetric
        {
            // Assume symmetric by default!
            get { return true; }
        }


        public bool IsMetric
        {
            // Do NOT assume triangle equation by default!
            get { return false; }
        }


        public  ITypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.DBID;
        }



        public IDistanceQuery Instantiate(IRelation database) 
        {
            return (IDistanceQuery)new DbIdDistanceQuery(database, this);
        }
    }
}
