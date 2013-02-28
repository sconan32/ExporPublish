using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{
    public class DataStoreUtil
    {
        /**
         * Make a new storage, to associate the given ids with an object of class dataclass.
         * 
         * @param <T> stored data type
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param dataclass class to store
         * @return new data store
         */
        public static IWritableDataStore<T> MakeStorage<T>(IDbIds ids, DataStoreHints hints, Type dataclass)
        {
            return DataStoreFactoryBase.FACTORY.MakeStorage<T>(ids, hints, dataclass);
        }

        /**
         * Make a new storage, to associate the given ids with an object of class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @return new data store
         */
        public static IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints)
        {
            return DataStoreFactoryBase.FACTORY.MakeDoubleStorage(ids, hints);
        }

        /**
         * Make a new storage, to associate the given ids with an object of class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param def Default value
         * @return new data store
         */
        public static IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints, double def)
        {
            return DataStoreFactoryBase.FACTORY.MakeDoubleStorage(ids, hints, def);
        }

        /**
         * Make a new storage, to associate the given ids with an object of class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @return new data store
         */
        public static IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints)
        {
            return DataStoreFactoryBase.FACTORY.MakeInt32Storage(ids, hints);
        }

        /**
         * Make a new storage, to associate the given ids with an object of class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param def Default value
         * @return new data store
         */
        public static IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints, int def)
        {
            return DataStoreFactoryBase.FACTORY.MakeInt32Storage(ids, hints, def);
        }

        /**
         * Make a new record storage, to associate the given ids with an object of class dataclass.
         * 
         * @param ids IDbIds to store data for
         * @param hints Hints for the storage manager
         * @param dataclasses classes to store
         * @return new record store
         */
        public static IWritableRecordStore MakeRecordStorage(IDbIds ids, DataStoreHints hints, params Type[] dataclasses)
        {
            return DataStoreFactoryBase.FACTORY.MakeRecordStorage(ids, hints, dataclasses);
        }
    }
}
