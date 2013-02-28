using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Algorithms.Outliers
{
    /**
   * Generic super interface for outlier detection algorithms.
   * 
   * @author Erich Schubert
   *
   * @apiviz.landmark
   * @apiviz.excludeSubtypes
   * @apiviz.has OutlierResult
   */
    public interface IOutlierAlgorithm : IAlgorithm
    {
        // Note: usually you won't override this method directly, but instead
        // Use the magic in AbstractAlgorithm and just implement a run method for your input data

        //IResult Run(Database database);
    }

}
