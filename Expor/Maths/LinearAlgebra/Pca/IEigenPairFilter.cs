using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public interface IEigenPairFilter : IParameterizable
    {
        /**
         * Filters the specified eigenpairs into strong and weak eigenpairs,
         * where strong eigenpairs having high variances
         * and weak eigenpairs having small variances.
         *
         * @param eigenPairs the eigenPairs (i.e. the eigenvectors and
         * @return the filtered eigenpairs
         */
        FilteredEigenPairs Filter(SortedEigenPairs eigenPairs);
    }

}
