using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree
{

    /**
     * Factory interface for indexes.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.stereotype factory,interface
     * @apiviz.has Index oneway - - 芦create禄
     *
     * @param <V> Input object type
     * @param <I> Index type
     */
    public interface IndexFactory : IParameterizable
    {
        /**
         * Sets the database in the distance function of this index (if existing).
         * 
         * @param relation the relation to index
         */
        IIndex Instantiate(IRelation relation);

        /**
         * Get the input type restriction used for negotiating the data query.
         * 
         * @return Type restriction
         */
        ITypeInformation GetInputTypeRestriction();
    }
}
