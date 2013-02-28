using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.DataSources.Bundles;

namespace Socona.Expor.Databases
{

    public interface IUpdatableDatabase : IDatabase
    {
        /**
         * Inserts the given object(s) and their associations into the database.
         * 
         * @param objpackages the objects to be inserted
         * @return the IDs assigned to the inserted objects
         * @throws UnableToComplyException if insertion is not possible
         */
        IDbIds Insert(IObjectBundle objpackages);

        /**
         * Removes and returns the specified objects with the given ids from the
         * database.
         * 
         * @param ids the ids of the object to be removed from the database
         * @return the objects that have been removed
         * @throws UnableToComplyException if deletion is not possible
         */
        IObjectBundle Delete(IDbIds ids);
    }
}
