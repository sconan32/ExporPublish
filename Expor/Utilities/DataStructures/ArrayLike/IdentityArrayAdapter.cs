using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Single-item subset adapter
     * 
     * Use the static instance from {@link ArrayLikeUtil}!
     * 
     * @author Erich Schubert
     * 
     * @param <T> Item type
     */
    public class IdentityArrayAdapter<T> : ArrayAdapterBase<T>
    {
        /**
         * Constructor.
         * 
         * Use the static instance from {@link ArrayLikeUtil}!
         */
        internal IdentityArrayAdapter()
            : base()
        {

        }


        public override int Size(IEnumerable<T> array)
        {
            return 1;
        }


        public override T Get(IEnumerable<T> array, int off)
        {
            Debug.Assert(off == 0, "Invalid get()");
            return array.ElementAt(off);
        }


    }
}
