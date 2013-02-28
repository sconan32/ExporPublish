using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.DataSources.Filters
{
    public interface INormalization<O> : IObjectFilter, IParameterizable
    {
        /**
         * Performs a normalization on a database object bundle.
         *
         * @param objects the database objects package
         * @return modified object bundle
         * @throws NonNumericFeaturesException if feature vectors differ in length or values are not
         *                                     suitable to normalization
         */
        MultipleObjectsBundle NormalizeObjects(MultipleObjectsBundle objects);

        /**
         * Transforms a feature vector to the original attribute ranges.
         *
         * @param featureVector a feature vector to be transformed into original space
         * @return a feature vector transformed into original space corresponding to
         *         the given feature vector
         * @throws NonNumericFeaturesException feature vector is not compatible with values initialized
         *                                     during normalization
         */
        O Restore(O featureVector);

        /**
         * Transforms a linear equation system describing linear dependencies
         * derived on the normalized space into a linear equation system describing
         * linear dependencies quantitatively adapted to the original space.
         *
         * @param linearEquationSystem the linear equation system to be transformed
         * @return a linear equation system describing linear dependencies
         *         derived on the normalized space transformed into a linear equation system
         *         describing linear dependencies quantitatively adapted to the original space
         * @throws NonNumericFeaturesException if specified linear equation system is not compatible
         *                                     with values initialized during normalization
         */
        LinearEquationSystem Transform(LinearEquationSystem linearEquationSystem);
    }
}
