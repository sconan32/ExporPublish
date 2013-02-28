using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public class PCAFilteredRunner<V> : PCARunner<V> where V : INumberVector
    {
        /**
         * Parameter to specify the filter for determination of the strong and weak
         * eigenvectors, must be a subclass of {@link EigenPairFilter}.
         * <p/>
         * Default value: {@link PercentageEigenPairFilter}
         * </p>
         * <p/>
         * Key: {@code -pca.filter}
         * </p>
         */
        public static OptionDescription PCA_EIGENPAIR_FILTER = OptionDescription.GetOrCreate(
            "pca.filter", "Filter class to determine the strong and weak eigenvectors.");

        /**
         * Parameter to specify a constant big value to reset high eigenvalues, must
         * be a double greater than 0.
         * <p>
         * Default value: {@code 1.0}
         * </p>
         * <p>
         * Key: {@code -pca.big}
         * </p>
         */
        public static OptionDescription BIG_ID = OptionDescription.GetOrCreate(
            "pca.big", "A constant big value to reset high eigenvalues.");

        /**
         * Parameter to specify a constant small value to reset low eigenvalues, must
         * be a double greater than 0.
         * <p>
         * Default value: {@code 0.0}
         * </p>
         * <p>
         * Key: {@code -pca.small}
         * </p>
         */
        public static OptionDescription SMALL_ID = OptionDescription.GetOrCreate(
            "pca.small", "A constant small value to reset low eigenvalues.");

        /**
         * Holds the instance of the EigenPairFilter specified by
         * {@link #PCA_EIGENPAIR_FILTER}.
         */
        private IEigenPairFilter eigenPairFilter;

        /**
         * Holds the value of {@link #BIG_ID}.
         */
        private double big;

        /**
         * Holds the value of {@link #SMALL_ID}.
         */
        private double small;

        /**
         * Constructor.
         * 
         * @param covarianceMatrixBuilder
         * @param eigenPairFilter
         * @param big
         * @param small
         */
        public PCAFilteredRunner(ICovarianceMatrixBuilder<V> covarianceMatrixBuilder,
            IEigenPairFilter eigenPairFilter, double big, double small) :
            base(covarianceMatrixBuilder)
        {
            this.eigenPairFilter = eigenPairFilter;
            this.big = big;
            this.small = small;
        }

        /**
         * Run PCA on a collection of database IDs
         * 
         * @param ids a collection of ids
         * @param database the database used
         * @return PCA result
         */

        public override PCAResult ProcessIds(IDbIds ids, IRelation database)
        {
            return ProcessCovarMatrix(covarianceMatrixBuilder.ProcessIds(ids, database));
        }

        /**
         * Run PCA on a QueryResult Collection
         * 
         * @param results a collection of QueryResults
         * @param database the database used
         * @return PCA result
         */

        public override PCAResult ProcessQueryResult(ICollection<IDistanceDbIdPair> results, IRelation database)
        {
            return ProcessCovarMatrix(covarianceMatrixBuilder.ProcessQueryResults(results, database));
        }

        /**
         * Process an existing Covariance Matrix
         * 
         * @param covarMatrix the matrix used for performing PCA
         */

        public override PCAResult ProcessCovarMatrix(Matrix covarMatrix)
        {
            // TODO: Add support for a different implementation to do EVD?
            EigenvalueDecomposition evd = new EigenvalueDecomposition(covarMatrix);
            return ProcessEVD(evd);
        }

        /**
         * Process an existing eigenvalue decomposition
         * 
         * @param evd eigenvalue decomposition to use
         */

        public override PCAResult ProcessEVD(EigenvalueDecomposition evd)
        {
            SortedEigenPairs eigenPairs = new SortedEigenPairs(evd, false);
            FilteredEigenPairs filteredEigenPairs = eigenPairFilter.Filter(eigenPairs);
            return new PCAFilteredResult(eigenPairs, filteredEigenPairs, big, small);
        }

        /**
         * Retrieve the {@link EigenPairFilter} to be used. For derived PCA Runners
         * 
         * @return eigenpair filter configured.
         */
        protected IEigenPairFilter GetEigenPairFilter()
        {
            return eigenPairFilter;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : PCARunner<V>.Parameterizer
        {
            /**
             * Holds the instance of the EigenPairFilter specified by
             * {@link #PCA_EIGENPAIR_FILTER}.
             */
            protected IEigenPairFilter eigenPairFilter;

            /**
             * Holds the value of {@link #BIG_ID}.
             */
            protected double big;

            /**
             * Holds the value of {@link #SMALL_ID}.
             */
            protected double small;

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IEigenPairFilter> EIGENPAIR_FILTER_PARAM = new ObjectParameter<IEigenPairFilter>(
                    PCA_EIGENPAIR_FILTER, typeof(IEigenPairFilter), typeof(PercentageEigenPairFilter));
                if (config.Grab(EIGENPAIR_FILTER_PARAM))
                {
                    eigenPairFilter = EIGENPAIR_FILTER_PARAM.InstantiateClass(config);
                }

                DoubleParameter BIG_PARAM = new DoubleParameter(BIG_ID, new GreaterConstraint<double>(0), 1.0);
                if (config.Grab(BIG_PARAM))
                {
                    big = BIG_PARAM.GetValue();

                }

                DoubleParameter SMALL_PARAM = new DoubleParameter(SMALL_ID, new GreaterEqualConstraint<double>(0), 0.0);
                if (config.Grab(SMALL_PARAM))
                {
                    small = SMALL_PARAM.GetValue();
                }

                // global constraint small <--> big
                config.CheckConstraint(new LessGlobalConstraint<Double>(SMALL_PARAM, BIG_PARAM));
            }

            protected override object MakeInstance()
            {
                return new PCAFilteredRunner<V>(covarianceMatrixBuilder, eigenPairFilter, big, small);
            }
        }
    }
}
