using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.Outliers;

namespace Socona.Expor.Utilities.Scaling
{

    public interface IOutlierScalingFunction : IScalingFunction
    {
        /**
         * Prepare is called once for each data set, before getScaled() will be
         * called. This function can be used to extract global parameters such as
         * means, minimums or maximums from the Database, Result or Annotation.
         * 
         * @param or Outlier result to use
         */
         void Prepare(OutlierResult or);
    }
}
