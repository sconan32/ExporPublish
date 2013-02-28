using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.DataStructures;


namespace Socona.Expor.Data
{
    public interface INumberVector : IDataVector, ISpatialComparable, IParameterizable
    {


        double this[int dimension] { get; set; }
        /**
         * Returns a Vector representing in one column and
         * <code>getDimensionality()</code> rows the values of this NumberVector of V.
         * 
         * @return a Matrix representing in one column and
         *         <code>getDimensionality()</code> rows the values of this
         *         NumberVector of V
         */
        Vector GetColumnVector();

        /**
         * Returns a new NumberVector of N for the given values.
         * 
         * @param values the values of the NumberVector
         * @return a new NumberVector of N for the given values
         */
        INumberVector NewNumberVector(double[] values);

        /**
         * Instantiate from any number-array like object.
         * 
         * @param <A> Array type
         * @param array Array
         * @param aduapter Adapter
         * @return a new NumberVector of N for the given values.
         */
        // INumberVector NewNumberVector<A>(A array, INumberArrayAdapter<T, A> adapter);
    }
}
