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
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public class PCARunner<V> : IParameterizable
    where V : INumberVector
    {
        /**
         * Parameter to specify the class to compute the covariance matrix, must be a
         * subclass of {@link CovarianceMatrixBuilder}.
         * <p>
         * Default value: {@link CovarianceMatrixBuilder}
         * </p>
         * <p>
         * Key: {@code -pca.covariance}
         * </p>
         */
        public static OptionDescription PCA_COVARIANCE_MATRIX = OptionDescription.GetOrCreate("pca.covariance", "Class used to compute the covariance matrix.");

        /**
         * The covariance computation class.
         */
        protected ICovarianceMatrixBuilder<V> covarianceMatrixBuilder;

        /**
         * Constructor.
         * 
         * @param covarianceMatrixBuilder Class for computing the covariance matrix
         */
        public PCARunner(ICovarianceMatrixBuilder<V> covarianceMatrixBuilder) :
            base()
        {
            this.covarianceMatrixBuilder = covarianceMatrixBuilder;
        }

        /**
         * Run PCA on the complete database
         * 
         * @param database the database used
         * @return PCA result
         */
        public virtual PCAResult ProcessDatabase(IRelation database)
        {
            return ProcessCovarMatrix(covarianceMatrixBuilder.ProcessDatabase(database));
        }

        /**
         * Run PCA on a collection of database IDs
         * 
         * @param ids a collection of ids
         * @param database the database used
         * @return PCA result
         */
        public virtual PCAResult ProcessIds(IDbIds ids, IRelation database)
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
        public virtual PCAResult ProcessQueryResult(ICollection<IDistanceDbIdPair> results, IRelation database)
        {
            return ProcessCovarMatrix(covarianceMatrixBuilder.ProcessQueryResults(results, database));
        }

        /**
         * Process an existing covariance Matrix
         * 
         * @param covarMatrix the matrix used for performing pca
         * @return PCA result
         */
        public virtual PCAResult ProcessCovarMatrix(Matrix covarMatrix)
        {
            // TODO: Add support for a different implementation to do EVD?
            EigenvalueDecomposition evd = new EigenvalueDecomposition(covarMatrix);
            return ProcessEVD(evd);
        }

        /**
         * Process an existing eigenvalue decomposition
         * 
         * @param evd eigenvalue decomposition to use
         * @return PCA result
         */
        public virtual PCAResult ProcessEVD(EigenvalueDecomposition evd)
        {
            SortedEigenPairs eigenPairs = new SortedEigenPairs(evd, false);
            return new PCAResult(eigenPairs);
        }


        public ICovarianceMatrixBuilder<V> CovarianceMatrixBuilder
        {
            get { return covarianceMatrixBuilder; }
            set { covarianceMatrixBuilder = value; }
        }
        /**
         * Get covariance matrix builder
         * 
         * @return covariance matrix builder in use
         */
        public ICovarianceMatrixBuilder<V> GetCovarianceMatrixBuilder()
        {
            return covarianceMatrixBuilder;
        }

        /**
         * Set covariance matrix builder.
         * 
         * @param covarianceBuilder New covariance matrix builder.
         */
        public void SetCovarianceMatrixBuilder(ICovarianceMatrixBuilder<V> covarianceBuilder)
        {
            this.covarianceMatrixBuilder = covarianceBuilder;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public  class Parameterizer : AbstractParameterizer
        {
            /**
             * The covariance computation class.
             */
            protected ICovarianceMatrixBuilder<V> covarianceMatrixBuilder;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<ICovarianceMatrixBuilder<V>> covarianceP =
                    new ObjectParameter<ICovarianceMatrixBuilder<V>>(PCA_COVARIANCE_MATRIX,
                        typeof(ICovarianceMatrixBuilder<V>), typeof(StandardCovarianceMatrixBuilder));
                if (config.Grab(covarianceP))
                {
                    covarianceMatrixBuilder = covarianceP.InstantiateClass(config);
                }
            }


            protected override object MakeInstance()
            {
                return new PCARunner<V>(covarianceMatrixBuilder);
            }

           
        }
    }
}
