using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Use a Float array as, well, Float array in the ArrayAdapter API.
     * 
     * @author Erich Schubert
     *
     * @apiviz.exclude
     */
    class FloatArrayAdapter : ArrayAdapterBase<float>
    {
        /**
         * Constructor.
         * 
         * Use the static instance from {@link ArrayLikeUtil}!
         */
        internal FloatArrayAdapter()
            : base()
        {

        }


        public  int Size(float[] array)
        {
            return array.Length;
        }


        public  float Get(float[] array, int off)
        {
            return array[off];
        }

        //@Override
        //public Float getFloat(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return array[off];
        //}

        //@Override
        //public float getFloat(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return (float) array[off];
        //}

        //@Override
        //public int getInteger(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return (int) array[off];
        //}

        //@Override
        //public short getShort(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return (short) array[off];
        //}

        //@Override
        //public long getLong(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return (long) array[off];
        //}

        //@Override
        //public byte getByte(Float[] array, int off) throws IndexOutOfBoundsException {
        //  return (byte) array[off];
        //}    


        public override int Size(IEnumerable<float> array)
        {
            return array.Count();
        }

        public override float Get(IEnumerable<float> array, int off)
        {
            return array.ElementAt(off);
        }
    }

}
