using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Databases.Queries
{

    /**
     * General interface for database queries.
     * Will only contain elemental stuff such as some hints.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     */
    public interface IDatabaseQuery
    {
        /// <summary>
        ///  Access the underlying data query.
        /// </summary>
        IRelation Relation { get; }
    }
}
