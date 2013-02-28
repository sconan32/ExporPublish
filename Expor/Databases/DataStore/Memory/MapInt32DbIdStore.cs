using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class MapInt32DbIdStore<T> : IWritableDataStore<T>
    {
        /**
         * Storage Map
         */
        private IDictionary<Int32, T> data;

        /**
         * Constructor.
         * 
         * @param data Existing map
         */
        public MapInt32DbIdStore(IDictionary<Int32, T> data)
        {

            this.data = data;
        }

        /**
         * Constructor.
         */
        public MapInt32DbIdStore()
        {

            this.data = new Dictionary<Int32, T>();
        }

        /**
         * Constructor.
         *
         * @param size Expected size
         */
        public MapInt32DbIdStore(int size)
        {
            this.data = new Dictionary<Int32, T>(size);
        }

        public T this[IDbIdRef id]
        {
            get { return data[id.Int32Id]; }
            set
            {
                object obj = value;
                if (value == null)
                {
                    obj = data[id.Int32Id];
                    data.Remove(id.Int32Id);
                }
                data[id.Int32Id] = (T)value;
                
            }
        }
        public  T Get(IDbIdRef id)
        {
            return data[id.Int32Id];
        }


        public  T Put(IDbIdRef id, T value)
        {
            T obj = value;
            if (value == null)
            {
                obj = data[id.Int32Id];
                data.Remove(id.Int32Id);
            }
            data[id.Int32Id] = value;
            return obj;
        }


        public  void Destroy()
        {
            data = null;
        }

        public  void Delete(IDbIdRef id)
        {
            data.Remove(id.Int32Id);
        }


        public String LongName
        {
            get
            {
                return "raw";
            }
        }


        public String ShortName
        {
            get
            {
                return "raw";
            }
        }
    }
}
