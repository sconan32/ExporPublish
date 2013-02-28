using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class ArrayDoubleStore : IWritableDoubleDataStore
    {
        /**
         * Data array
         */
        private double[] data;

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
        public ArrayDoubleStore(int size, IDataStoreIdMap idmap) :
            this(size, idmap, Double.NaN)
        {
        }

        /**
         * Constructor.
         * 
         * @param size Size
         * @param idmap ID map
         * @param def Default value
         */
        public ArrayDoubleStore(int size, IDataStoreIdMap idmap, double def)
            : base()
        {

            this.data = new double[size];
            if (def != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    data[i] = def;
                }
            }
            this.idmap = idmap;
        }

        public double this[IDbIdRef id]
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
                double ret = data[off];
                data[off] = (double)value;

            }
        }
        public Double get(IDbIdRef id)
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



        public double GetDouble(IDbIdRef id)
        {
            return data[idmap.Map(id)];
        }


        public double PutDouble(IDbIdRef id, double value)
        {
            int off = idmap.Map(id);
            double ret = data[off];
            data[off] = value;
            return ret;
        }


        public double Put(IDbIdRef id, double value)
        {
            int off = idmap.Map(id);
            double ret = data[off];
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



        public double Get(IDbIdRef id)
        {
            throw new NotImplementedException();
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
