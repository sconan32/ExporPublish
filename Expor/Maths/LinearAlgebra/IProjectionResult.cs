using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra
{

    // TODO: cleanup
    public interface IProjectionResult
    {
        /**
         * Get the number of "strong" dimensions
         * 
         * @return number of strong (correlated) dimensions
         */
         int GetCorrelationDimension();

        /**
         * Projection matrix
         * 
         * @return projection matrix
         */
         Matrix SimilarityMatrix();
    }

}
