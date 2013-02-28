using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class ArrayRecordStore : IWritableRecordStore
    {
        /**
         * Data array
         */
        private Object[,] data;

        /**
         * DBID to index map
         */
        private IDataStoreIdMap idmap;

        /**
         * Constructor with existing data
         * 
         * @param data Existing data
         * @param idmap Map for array offsets
         */
        public ArrayRecordStore(Object[,] data, IDataStoreIdMap idmap)
        {

            this.data = data;
            this.idmap = idmap;
        }


        public  IWritableDataStore<T> GetStorage<T>(int col, Type datatype)
        {
            // TODO: add type checking safety?
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
            try
            {
                return (T)data[idmap.Map(id), index];
            }
            catch (IndexOutOfRangeException )
            {
                return default(T);
            }
            catch (NullReferenceException )
            {
                return default(T);
            }
            catch (InvalidCastException )
            {
                return default(T);
            }
        }

        /**
         * Actual setter
         * 
         * @param id Database ID
         * @param index column index
         * @param value New value
         * @return old value
         */

        protected T Set<T>(IDbIdRef id, int index, T value)
        {
            T ret = (T)data[idmap.Map(id), index];
            data[idmap.Map(id), index] = value;
            return ret;
        }



        public  bool Remove(IDbIdRef id)
        {
            throw new InvalidOperationException("ArrayStore records cannot be removed.");
        }

        IDataStore<T> IRecordStore.GetStorage<T>(int col, Type datatype)
        {
            throw new NotImplementedException();
        }
        /**
         * Access a single record in the given data.
         * 
         * @author Erich Schubert
         * 
         * @param <T> Object data type to access
         */
        public class StorageAccessor<T> : IWritableDataStore<T>
        {
            /**
             * Representation index.
             */
            private int index;
            ArrayRecordStore store;
            /**
             * Constructor.
             * 
             * @param index In-record index
             */
            internal StorageAccessor(ArrayRecordStore store, int index)
            {
                this.store = store;
                this.index = index;
            }

            public T this[IDbIdRef id]
            {
                get { return store.Get<T>(id, index); }
                set
                {
                    store.Set(id, index, value);
                }
            }
            public  T Get(IDbIdRef id)
            {
                return (T)store.Get<T>(id, index);
            }


            public T Put(IDbIdRef id, T value)
            {
                return store.Set(id, index, value);
            }


            public  void Destroy()
            {
                throw new InvalidOperationException("ArrayStore record columns cannot be destroyed.");
            }


            public  void Delete(IDbIdRef id)
            {
                throw new InvalidOperationException("ArrayStore record values cannot be deleted.");
            }

            public string LongName
            {
                get { return "raw"; }
            }

            public string ShortName
            {
                get { return "raw"; }
            }
        }


        
    }
}
