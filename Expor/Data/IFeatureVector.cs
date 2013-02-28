using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.DataStructures.ArrayLike;

namespace Socona.Expor.Data
{

    /**
     * Generic DataVector class that can contain any type of data (i.e. numerical
     * or categorical attributes). See {@link INumberVector} for vectors that
     * actually store numerical features.
     * 
     * @author Erich Schubert
     * 
     */
    public interface IDataVector : IHasDimensionality
    {


        /**
         * Returns the value in the specified dimension.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         */
        object Get(int dimesion);


        /**
         * Returns a new FeatureVector of V for the given values.
         * 
         * @param array the values of the featureVector
         * @param adapter adapter class
         * @param <A> Array type
         * @return a new FeatureVector of V for the given values
         */
        IDataVector NewFeatureVector(IList<object> array, IArrayAdapter adapter);
    }
}
