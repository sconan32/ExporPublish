using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Adapter for array-like things. For example, arrays and lists.
     * 
     * @author Erich Schubert
     * 
     */
    public interface IArrayAdapter
    {
        /**
         * Get the size of the array.
         * 
         * @param array Array-like thing
         * @return Size
         */
        int Size(IEnumerable array);

        /**
         * Get the off'th item from the array.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        object Get(IEnumerable array, int off);
    }
}
