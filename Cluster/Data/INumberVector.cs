using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Data.Spatial;
using Socona.Clustering.Utilities.Options;
using Socona.Clustering.Math.LinearAlgebra;
using Socona.Clustering.Utilities.DataStructures;

namespace Socona.Clustering.Data
{

    /**
     * Interface NumberVector defines the methods that should be implemented by any
     * Object that is element of a real vector space of type N.
     * 
     * @author Arthur Zimek
     * 
     * @param <V> the type of NumberVector implemented by a subclass
     * @param <N> the type of the attribute values
     * 
     * @apiviz.landmark
     * @apiviz.has Vector
     */
    public interface INumberVector<N> : IFeatureVector<N>, ISpatialComparable, IParameterizable
    {
        /**
         * Returns the value in the specified dimension as double.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).doubleValue()}, but usually this is much more
         * efficient due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        double doubleValue(int dimension);

        /**
         * Returns the value in the specified dimension as float.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).floatValue()}, but usually this is much more efficient
         * due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        float floatValue(int dimension);

        /**
         * Returns the value in the specified dimension as int.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).intValue()}, but usually this is much more efficient
         * due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        int intValue(int dimension);

        /**
         * Returns the value in the specified dimension as long.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).longValue()}, but usually this is much more efficient
         * due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        long longValue(int dimension);

        /**
         * Returns the value in the specified dimension as short.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).shortValue()}, but usually this is much more efficient
         * due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        short shortValue(int dimension);

        /**
         * Returns the value in the specified dimension as byte.
         * 
         * Note: this might seem redundant with respect to
         * {@code getValue(dim).byteValue()}, but usually this is much more efficient
         * due to boxing/unboxing cost.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        byte byteValue(int dimension);

        /**
         * Returns a Vector representing in one column and
         * <code>getDimensionality()</code> rows the values of this NumberVector of V.
         * 
         * @return a Matrix representing in one column and
         *         <code>getDimensionality()</code> rows the values of this
         *         NumberVector of V
         */
        Vector getColumnVector();

        /**
         * Returns a new NumberVector of N for the given values.
         * 
         * @param values the values of the NumberVector
         * @return a new NumberVector of N for the given values
         */
        INumberVector<N> newNumberVector(double[] values);

        /**
         * Instantiate from any number-array like object.
         * 
         * @param <A> Array type
         * @param array Array
         * @param adapter Adapter
         * @return a new NumberVector of N for the given values.
         */
        INumberVector<N> newNumberVector<A>(A array, INumberArrayAdapter<N, A> adapter);
    }
}
