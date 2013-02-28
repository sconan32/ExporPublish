using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes
{

    public abstract class AbstractIndex : IIndex
    {
        /**
         * The representation we are bound to.
         */
        protected IRelation relation;

        /**
         * Constructor.
         *
         * @param relation Relation indexed
         */
        public AbstractIndex(IRelation relation)
        {

            this.relation = relation;
        }


        abstract public String LongName { get; }


        abstract public String ShortName { get; }


        public virtual IPageFileStatistics GetPageFileStatistics()
        {
            // TODO: move this into a separate interface?
            // By default, we are not file based - no statistics available
            return null;
        }


        public virtual void Insert(IDbId id)
        {
            throw new InvalidOperationException("This index does not allow dynamic updates.");
        }


        public virtual void InsertAll(IDbIds ids)
        {
            throw new InvalidOperationException("This index does not allow dynamic updates.");
        }


        public virtual bool Delete(IDbId id)
        {
            throw new InvalidOperationException("This index does not allow dynamic updates.");
        }


        public virtual void DeleteAll(IDbIds id)
        {
            throw new InvalidOperationException("This index does not allow dynamic updates.");
        }
    }
}
