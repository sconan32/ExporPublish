using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{

    public interface IWritableDataStore<T> : IDataStore<T>
    {
        /**
         * Associates the specified value with the specified id in this storage. If
         * the storage previously contained a value for the id, the previous value is
         * replaced by the specified value.
         * 
         * @param id Database ID.
         * @param value Value to store.
         * @return previous value
         */

      
        //T Put(IDbIdRef id, T value);

        /**
         * Deallocate the storage, freeing the memory and notifies the registered
         * listeners.
         */
        void Destroy();

        /**
         * Delete the contents for a particular ID and notifies the registered
         * listeners.
         * 
         * @param id Database ID.
         */
        void Delete(IDbIdRef id);
    }


}
