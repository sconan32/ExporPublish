using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Use a double array as, well, double array in the ArrayAdapter API.
     * 
     * @author Erich Schubert
     *
     * @apiviz.exclude
     */
    class DoubleArrayAdapter : ArrayAdapterBase<double>
    {
        /**
         * Constructor.
         * 
         * Use the static instance from {@link ArrayLikeUtil}!
         */
        internal DoubleArrayAdapter()
            : base()
        {
           
        }


        public override int Size(IEnumerable<double> array)
        {
            return array.Count();
        }


        public override Double Get(IEnumerable<double> array, int off)
        {
            return array.ElementAt(off);
        }

        //@Override
        //public double getDouble(double[] array, int off) throws IndexOutOfBoundsException {
        //  return array[off];
        //}

        //@Override
        //public float getFloat(double[] array, int off) throws IndexOutOfBoundsException {
        //  return (float) array[off];
        //}

        //@Override
        //public int getInteger(double[] array, int off) throws IndexOutOfBoundsException {
        //  return (int) array[off];
        //}

        //@Override
        //public short getShort(double[] array, int off) throws IndexOutOfBoundsException {
        //  return (short) array[off];
        //}

        //@Override
        //public long getLong(double[] array, int off) throws IndexOutOfBoundsException {
        //  return (long) array[off];
        //}

        //@Override
        //public byte getByte(double[] array, int off) throws IndexOutOfBoundsException {
        //  return (byte) array[off];
        //}    

       
    }
}
