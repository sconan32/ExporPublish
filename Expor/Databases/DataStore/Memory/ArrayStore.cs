using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class ArrayStore<T> : IWritableDataStore<T>
    {
        /**
         * Data array
         */
        private T[] data;

        /**
         * DBID to index map
         */
        private IDataStoreIdMap idmap;

        /**
         * Constructor.
         */
        public ArrayStore(T[] data, IDataStoreIdMap idmap)
        {

            this.data = data;
            //if (!typeof(ValueType).IsAssignableFrom(typeof(T)))
            //{
            //    for (int i = 0; i < data.Length; i++)
            //    {

            //        data[i] = (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
            //    }
            //}
            this.idmap = idmap;
        }

        public T this[IDbIdRef id]
        {
            get
            {
                try
                {
                    return data[idmap.Map(id)];
                }
                catch (IndexOutOfRangeException)
                {
                    return default(T);
                }
                catch (NullReferenceException)
                {
                    return default(T);
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
            set
            {
                T ret = Get(id);
                data[idmap.Map(id)] = (T)value;

            }
        }
        public T Get(IDbIdRef id)
        {
            try
            {
                return (T)data[idmap.Map(id)];
            }
            catch (IndexOutOfRangeException)
            {
                return default(T);
            }
            catch (NullReferenceException)
            {
                return default(T);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }


        public T Put(IDbIdRef id, T value)
        {
            T ret = Get(id);
            data[idmap.Map(id)] = value;
            return ret;
        }


        public void Destroy()
        {
            data = null;
            idmap = null;
        }


        public void Delete(IDbIdRef id)
        {
            throw new NotImplementedException("Can't delete from a static array storage.");
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
