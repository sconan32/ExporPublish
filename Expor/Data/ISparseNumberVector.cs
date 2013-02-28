using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data
{

    public interface ISparseNumberVector : INumberVector, ISparseFeatureVector<double>
    {
        /**
         * Returns a new NumberVector of N for the given values.
         * 
         * @param values the values of the NumberVector
         * @param maxdim Maximum dimensionality.
         * @return a new NumberVector of N for the given values
         */
        ISparseNumberVector NewNumberVector(IDictionary<int,object> values, int maxdim);

        /**
         * Update the vector space dimensionality.
         * 
         * @param maxdim New dimensionality
         */
        void SetDimensionality(int maxdim);
    }

}
