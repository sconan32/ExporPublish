using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results.Outliers
{

    public interface IOutlierScoreMeta : IResult
    {
        /**
         * Get the actual minimum of the value range.
         * 
         * @return minimum or {@link Double#NaN}
         */
         double GetActualMinimum();

        /**
         * Get the actual maximum of the value range.
         * 
         * @return maximum or {@link Double#NaN}
         */
         double GetActualMaximum();

        /**
         * Get the theoretical minimum of the value range.
         * 
         * @return theoretical minimum or {@link Double#NaN}
         */
         double GetTheoreticalMinimum();

        /**
         * Get the theoretical maximum of the value range.
         * 
         * This value may be {@link Double#NEGATIVE_INFINITY} or {@link Double#NaN}.
         * 
         * @return theoretical maximum or {@link Double#NaN}
         */
         double GetTheoreticalMaximum();

        /**
         * Get the theoretical baseline of the value range.
         * 
         * It will be common to see {@link Double#POSITIVE_INFINITY} here.
         * 
         * @return theoretical baseline or {@link Double#NaN}
         */
         double GetTheoreticalBaseline();

        /**
         * Return a normalized value of the outlier score.
         * 
         * @param value outlier score
         * @return Normalized value (in 0.0-1.0)
         */
         double NormalizeScore(double value);
    }

}
