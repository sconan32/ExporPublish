using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Utilities.Scaling
{

    public interface IScalingFunction : IParameterizable
    {
        /**
         * Transform a given value using the scaling function.
         * 
         * @param value Original value
         * @return Scaled value
         */
         double GetScaled(double value);

        /**
         * Get minimum resulting value. May be {@link Double#NaN} or
         * {@link Double#NEGATIVE_INFINITY}.
         * 
         * @return Minimum resulting value.
         */
         double GetMin();

        /**
         * Get maximum resulting value. May be {@link Double#NaN} or
         * {@link Double#POSITIVE_INFINITY}.
         * 
         * @return Maximum resulting value.
         */
         double GetMax();
    }
}
