using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.DataStore.Memory;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{

    public interface IDataStoreFactory
    {

        /**
         * Make a new storage, to associate the given ids with an object of class
         * dataclass.
         * 
         * @param <T> stored data type
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param dataclass class to store
         * @return new data store
         */
        IWritableDataStore<T> MakeStorage<T>(IDbIds ids, DataStoreHints hints, Type dataclass);

        /**
         * Make a new storage, to associate the given ids with an object of class
         * dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @return new data store
         */
        IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints);

        /**
         * Make a new storage, to associate the given ids with an object of class
         * dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param def Default value
         * @return new data store
         */
        IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints, double def);

        /**
         * Make a new storage, to associate the given ids with an object of class
         * dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @return new data store
         */
        IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints);

        /**
         * Make a new storage, to associate the given ids with an object of class
         * dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param def Default value
         * @return new data store
         */
        IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints, int def);

        /**
         * Make a new record storage, to associate the given ids with an object of
         * class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param dataclasses classes to store
         * @return new record store
         */
        IWritableRecordStore MakeRecordStorage(IDbIds ids, DataStoreHints hints, params Type[] dataclasses);
    }
}
