using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.DataStore.Memory;

namespace Socona.Expor.Databases.DataStore
{
    public abstract class DataStoreFactoryBase : IDataStoreFactory
    {
        /**
           * Static storage factory
           */
        public static IDataStoreFactory FACTORY = new MemoryDataStoreFactory();


        abstract public IWritableDataStore<T> MakeStorage<T>(Ids.IDbIds ids, DataStoreHints hints, Type dataclass);


        abstract public IWritableDoubleDataStore MakeDoubleStorage(Ids.IDbIds ids, DataStoreHints hints);

        abstract public IWritableDoubleDataStore MakeDoubleStorage(Ids.IDbIds ids, DataStoreHints hints, double def);

        abstract public IWritableInt32DataStore MakeInt32Storage(Ids.IDbIds ids, DataStoreHints hints);

        abstract public IWritableInt32DataStore MakeInt32Storage(Ids.IDbIds ids, DataStoreHints hints, int def);

        abstract public IWritableRecordStore MakeRecordStorage(Ids.IDbIds ids, DataStoreHints hints, params Type[] dataclasses);
    }
    [Flags]
    public enum DataStoreHints
    {
        /// <summary>
        ///  No Hints (Default Value)
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Storage will be used only temporary.
        /// </summary>
        Temp = 0x01,

        /// <summary>
        /// "Hot" data, that will be used a lot, preferring memory storage.
        /// </summary>
        Hot = 0x02,

        /// <summary>
        /// "static" data, that will not change often
        /// </summary>
        Static = 0x04,

        /// <summary>
        /// Data that might require sorted access (so hashmaps are suboptimal)
        /// </summary>
        Sorted = 0x08,

        /// <summary>
        /// Data that is the main database. Includes HOT, STATIC, SORTED
        /// </summary>
        Database = 0x1E,
    }
}
