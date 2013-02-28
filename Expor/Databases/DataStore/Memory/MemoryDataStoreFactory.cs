using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class MemoryDataStoreFactory : IDataStoreFactory
    {

        public IWritableDataStore<T> MakeStorage<T>(IDbIds ids, DataStoreHints hints, Type dataclass)
        {
            if (dataclass == typeof(double))
            {
                return (IWritableDataStore<T>)MakeDoubleStorage(ids, hints);
            }
            if (dataclass == typeof(int))
            {
                return (IWritableDataStore<T>)MakeInt32Storage(ids, hints);
            }
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                T[] data = new T[range.Count];
                return new ArrayStore<T>(data, new RangeIdMap(range));
            }
            else
            {
                return new MapInt32DbIdStore<T>(ids.Count);
            }
        }


        public IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints)
        {
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                return new ArrayDoubleStore(range.Count, new RangeIdMap(range));
            }
            else
            {
                return new MapInt32DbIdDoubleStore(ids.Count);
            }
        }


        public IWritableDoubleDataStore MakeDoubleStorage(IDbIds ids, DataStoreHints hints, double def)
        {
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                return new ArrayDoubleStore(range.Count, new RangeIdMap(range), def);
            }
            else
            {
                return new MapInt32DbIdDoubleStore(ids.Count, def);
            }
        }


        public IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints)
        {
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                return new ArrayInt32Store(range.Count, new RangeIdMap(range));
            }
            else
            {
                var store = new MapInt32DbIdInt32Store(ids.Count);
                foreach (var id in ids)
                {
                    store[id] = 0;
                }
                return store;
            }
        }


        public IWritableInt32DataStore MakeInt32Storage(IDbIds ids, DataStoreHints hints, int def)
        {
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                return new ArrayInt32Store(range.Count, new RangeIdMap(range), def);
            }
            else
            {
                var store = new MapInt32DbIdInt32Store(ids.Count, def);
                foreach (var id in ids)
                {
                    store[id] = def;
                }
                return store;

            }
        }


        public IWritableRecordStore MakeRecordStorage(IDbIds ids, DataStoreHints hints, params Type[] dataclasses)
        {
            if (ids is IDbIdRange)
            {
                IDbIdRange range = (IDbIdRange)ids;
                Object[,] data = new Object[range.Count, dataclasses.Length];
                return new ArrayRecordStore(data, new RangeIdMap(range));
            }
            else
            {
                return new MapInt32DbIdRecordStore(ids.Count, dataclasses.Length);
            }
        }



    }
}
