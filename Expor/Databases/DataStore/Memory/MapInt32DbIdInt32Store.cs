using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore.Memory
{

    public class MapInt32DbIdInt32Store : IWritableInt32DataStore
    {
        /**
         * Data storage
         */
        private IDictionary<int, int> map;

        /**
         * Constructor.
         * 
         * @param size Expected size
         */
        public MapInt32DbIdInt32Store(int size) :
            this(size, 0)
        {
        }

        /**
         * Constructor.
         * 
         * @param size Expected size
         * @param def Default value
         */
        public MapInt32DbIdInt32Store(int size, int def)
            : base()
        {
            map = new Dictionary<int, int>(size);

        }

        public int this[IDbIdRef id]
        {
            get { return map[id.Int32Id]; }
            set
            {
                map[id.Int32Id] = (int)value;
            }
        }
        public Int32 Get(IDbIdRef id)
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


        public Int32 Put(IDbIdRef id, Int32 value)
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
