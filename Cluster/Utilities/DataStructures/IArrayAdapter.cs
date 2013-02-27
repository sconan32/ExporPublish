using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.DataStructures
{

    /**
     * Adapter for array-like things. For example, arrays and lists.
     * 
     * @author Erich Schubert
     * 
     * @param <T> Item type
     * @param <A> Array object type
     */
    public interface IArrayAdapter<T, A>
    {
        /**
         * Get the size of the array.
         * 
         * @param array Array-like thing
         * @return Size
         */
        public int size(A array);

        /**
         * Get the off'th item from the array.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public T get(A array, int off);
    }
}
