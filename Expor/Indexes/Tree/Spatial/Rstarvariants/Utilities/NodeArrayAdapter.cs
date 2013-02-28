using System.Collections.Generic;
using System;
using System.Collections;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures.ArrayLike;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Utilities
{

    public class NodeArrayAdapter : ArrayAdapterBase<ISpatialEntry>
    {
        /**
         * Static adapter.
         */
        public static NodeArrayAdapter STATIC = new NodeArrayAdapter();

        /**
         * Constructor.
         */
        protected NodeArrayAdapter() :
            base()
        {
            // TODO Auto-generated constructor stub
        }


        public override int Size(IEnumerable<ISpatialEntry> array)
        {
            return array.Count();
        }


        public override ISpatialEntry Get(IEnumerable<ISpatialEntry> array, int off)
        {
            return array.ElementAt(off);
        }
    }
}
