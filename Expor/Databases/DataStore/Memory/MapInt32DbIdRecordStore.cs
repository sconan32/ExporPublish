using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class MapInt32DbIdRecordStore : IWritableRecordStore
    {
        /**
         * Record length
         */
        private int rlen;

        /**
         * Storage Map
         */
        private IDictionary<int, object[]> data;

        /**
         * Constructor with existing data.
         * 
         * @param rlen Number of columns (record length)
         * @param data Existing data map
         */
        public MapInt32DbIdRecordStore(int rlen, IDictionary<int, object[]> data)
        {

            this.rlen = rlen;
            this.data = data;
        }

        /**
         * Constructor without existing data.
         * 
         * @param rlen Number of columns (record length)
         */
        public MapInt32DbIdRecordStore(int rlen) :
            this(rlen, new Dictionary<int, object[]>())
        {
        }

        /**
         * Constructor without existing data.
         * 
         * @param size Expected size
         * @param rlen Number of columns (record length)
         */
        public MapInt32DbIdRecordStore(int size, int rlen) :
            this(rlen, new Dictionary<int, object[]>(size))
        {
        }


        public  IDataStore<T> GetStorage<T>(int col, Type datatype)
        {
            // TODO: add type checking?
            return new StorageAccessor<T>(this, col);
        }

        /**
         * Actual getter
         * 
         * @param id Database ID
         * @param index column index
         * @return current value
         */

        protected  T Get<T>(IDbIdRef id, int index)
        {
            Object[] d = data[id.Int32Id];
            if (d == null)
            {
                return default(T);
            }
            try
            {
                return (T)d[index];
            }
            catch (InvalidCastException )
            {
                return default(T);
            }
            catch (IndexOutOfRangeException )
            {
                return default(T);
            }
        }

        /**
         * Actual setter
         * 
         * @param id Database ID
         * @param index column index
         * @param value new value
         * @return previous value
         */

        protected T Set<T>(IDbIdRef id, int index, T value)
        {
            Object[] d = data[id.Int32Id];
            if (d == null)
            {
                d = new Object[rlen];
                data[id.Int32Id] = d;
            }
            T ret = (T)d[index];
            d[index] = value;
            return ret;
        }



        public  bool Remove(IDbIdRef id)
        {
            return data.Remove(id.Int32Id);
        }
        /**
       * Access a single record in the given data.
       * 
       * @author Erich Schubert
       * 
       * @param <T> Object data type to access
       */
        protected class StorageAccessor<T> : IWritableDataStore<T>
        {
            /**
             * Representation index.
             */
            private int index;
            MapInt32DbIdRecordStore store;
            /**
             * Constructor.
             * 
             * @param index In-record index
             */
            public StorageAccessor(MapInt32DbIdRecordStore store, int index)
            {
                this.store = store;
                this.index = index;
            }
            public T this[IDbIdRef id]
            {
                get { return store.Get<T>(id, index); }
                set {  store.Set<T>(id, index, value); }
            }
            public  T Get(IDbIdRef id)
            {
                return (T)store.Get<T>(id, index);
            }


            public T put(IDbIdRef id, T value)
            {
                return store.Set<T>(id, index, value);
            }


            public  void Destroy()
            {
                throw new InvalidOperationException("Record storage accessors cannot be destroyed.");
            }


            public void delete(IDbIdRef id)
            {
                throw new InvalidOperationException("Record storage values cannot be deleted.");
            }


            public  String GetLongName()
            {
                return "raw";
            }


            public  String GetShortName()
            {
                return "raw";
            }

            public T Put(IDbIdRef id, T value)
            {
                throw new NotImplementedException();
            }

            public void Delete(IDbIdRef id)
            {
                throw new NotImplementedException();
            }

            public string LongName
            {
                get { throw new NotImplementedException(); }
            }

            public string ShortName
            {
                get { throw new NotImplementedException(); }
            }
        }

    }
}
