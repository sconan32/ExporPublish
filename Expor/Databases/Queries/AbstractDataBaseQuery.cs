using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Databases.Queries
{

    /**
     * Abstract query bound to a certain representation.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Database object type
     */
    public abstract class AbstractDataBasedQuery : IDatabaseQuery
    {
        /**
         * The data to use for this query
         */
        protected readonly IRelation relation;

        /**
         * Database this query works on.
         * 
         * @param relation Representation
         */
        public AbstractDataBasedQuery(IRelation relation)
            : base()
        {

            this.relation = relation;

        }
        /**
         * Get the queries relation.
         * 
         * @return Relation
         */
        public  IRelation Relation
        {
            get { return relation; }
        }
    }
}
