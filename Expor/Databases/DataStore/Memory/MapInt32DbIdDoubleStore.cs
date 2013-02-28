using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class MapInt32DbIdDoubleStore : IWritableDoubleDataStore
    {
        /**
         * Data storage
         */
        private IDictionary<int, double> map;

        /**
         * Constructor.
         * 
         * @param size Expected size
         */
        public MapInt32DbIdDoubleStore(int size) :
            this(size, Double.NaN)
        {
        }

        /**
         * Constructor.
         * 
         * @param size Expected size
         * @param def Default value
         */
        public MapInt32DbIdDoubleStore(int size, double def)
            : base()
        {

            map = new Dictionary<int, double>(size);


        }

        public double this[IDbIdRef id]
        {
            get { return map[id.Int32Id]; }
            set { map[id.Int32Id] = (double)value; }
        }
        public Double Get(IDbIdRef id)
        {
            return map[id.Int32Id];
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


        public Double Put(IDbIdRef id, Double value)
        {
            return map[id.Int32Id] = value;
        }

        public void Destroy()
        {
            map.Clear();
            map = null;
        }


        public void Delete(IDbIdRef id)
        {
            map.Remove(id.Int32Id);
        }




    }

}
