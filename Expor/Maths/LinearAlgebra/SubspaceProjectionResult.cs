using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra
{

    public class SubspaceProjectionResult : IProjectionResult
    {
        /**
         * The correlation dimensionality
         */
        private int correlationDimensionality;

        /**
         * The similarity matrix
         */
        private Matrix similarityMat;

        /**
         * Constructor.
         * 
         * @param correlationDimensionality dimensionality
         * @param similarityMat projection matrix
         */
        public SubspaceProjectionResult(int correlationDimensionality, Matrix similarityMat) :
            base()
        {
            this.correlationDimensionality = correlationDimensionality;
            this.similarityMat = similarityMat;
        }


        public int GetCorrelationDimension()
        {
            return correlationDimensionality;
        }


        public Matrix SimilarityMatrix()
        {
            return similarityMat;
        }
    }

}
