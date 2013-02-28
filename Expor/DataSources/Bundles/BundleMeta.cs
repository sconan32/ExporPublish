using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;

namespace Socona.Expor.DataSources.Bundles
{

    public class BundleMeta : List<SimpleTypeInformation>
    {
        /**
         * Serial version
         */
        //private static long serialVersionUID = 1L;

        /**
         * Constructor.
         */
        public BundleMeta()
            : base()
        {

        }

        /**
         * Constructor.
         * 
         * @param initialCapacity
         */
        public BundleMeta(int initialCapacity)
            : base(initialCapacity)
        {
        }

        /**
         * Constructor.
         * 
         * @param types
         */
        public BundleMeta(params SimpleTypeInformation[] types)
            : base(types)
        {
        }
    }
}
