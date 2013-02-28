using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Relations;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed
{

    public abstract class AbstractPreprocessorIndex<R> : AbstractIndex
    {
        /**
         * The data store
         */
        protected IWritableDataStore<R> storage = null;

        /**
         * Constructor.
         */
        public AbstractPreprocessorIndex(IRelation relation) :
            base(relation)
        {
        }

        /**
         * Get the classes static logger.
         * 
         * @return Logger
         */
        abstract protected Logging GetLogger();
    }

}
