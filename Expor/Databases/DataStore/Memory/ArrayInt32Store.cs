using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class ArrayInt32Store : IWritableInt32DataStore
    {
        /**
         * Data array
         */
        private int[] data;

        /**
         * DBID to index map
         */
        private IDataStoreIdMap idmap;

        /**
         * Constructor.
         * 
         * @param size Size
         * @param idmap ID map
         */
        public ArrayInt32Store(int size, IDataStoreIdMap idmap) :
            this(size, idmap, 0)
        {
        }

        /**
         * Constructor.
         * 
         * @param size Size
         * @param idmap ID map
         * @param def Default value
         */
        public ArrayInt32Store(int size, IDataStoreIdMap idmap, int def)
            : base()
        {

            this.data = new int[size];
            if (def != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    data[i] = def;
                }
            }
            this.idmap = idmap;
        }

        public int this[IDbIdRef id]
        {
            get
            {
                try
                {
                    return data[idmap.Map(id)];
                }
                catch (IndexOutOfRangeException)
                {
                    return 0;
                }
            }
            set
            {
                int off = idmap.Map(id);
              
                data[off] = (int)value;
               
            }
        }
        public Int32 Get(IDbIdRef id)
        {
            try
            {
                return data[idmap.Map(id)];
            }
            catch (IndexOutOfRangeException )
            {
                return 0;
            }
        }


      

        public int Put(IDbIdRef id, int value)
        {
            int off = idmap.Map(id);
            int ret = data[off];
            data[off] = value;
            return ret;
        }


        public void Destroy()
        {
            data = null;
            idmap = null;
        }


        public void Delete(IDbIdRef id)
        {
            throw new InvalidOperationException("Can't delete from a static array storage.");
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
