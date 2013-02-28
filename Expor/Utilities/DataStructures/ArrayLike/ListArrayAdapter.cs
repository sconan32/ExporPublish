using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Static adapter class to use a {@link java.util.List} in an array API.
     * 
     * Use the static instance from {@link ArrayLikeUtil}!
     * 
     * @author Erich Schubert
     * 
     * @param <T> Data object type.
     */
    public class ListArrayAdapter<T> : ArrayAdapterBase<T>
    {
        /**
         * Constructor.
         *
         * Use the static instance from {@link ArrayLikeUtil}!
         */
        internal ListArrayAdapter()
            : base()
        {

        }


        public override int Size(IEnumerable<T> array)
        {
            return array.Count();
        }


        public override T Get(IEnumerable<T> array, int off)
        {
            return array.ElementAt(off) ;
        }
    }
}
