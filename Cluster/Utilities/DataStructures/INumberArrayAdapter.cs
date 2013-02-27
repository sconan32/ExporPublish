using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.DataStructures
{

    /**
     * Adapter for arrays of numbers, to avoid boxing.
     * 
     * @author Erich Schubert
     *
     * @param <N> Number type
     * @param <A> Array type
     */
    public interface INumberArrayAdapter<N, A> : IArrayAdapter<N, A>
    {

        public override int size(A array);


        public override N get(A array, int off);

        /**
         * Get the off'th item from the array as double.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public double getDouble(A array, int off);

        /**
         * Get the off'th item from the array as float.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public float getFloat(A array, int off);

        /**
         * Get the off'th item from the array as integer.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public int getInteger(A array, int off);

        /**
         * Get the off'th item from the array as short.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public short getShort(A array, int off);

        /**
         * Get the off'th item from the array as long.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public long getLong(A array, int off);

        /**
         * Get the off'th item from the array as byte.
         * 
         * @param array Array to get from
         * @param off Offset
         * @return Item at offset off
         * @throws IndexOutOfBoundsException for an invalid index.
         */
        public byte getByte(A array, int off);
    }
}
