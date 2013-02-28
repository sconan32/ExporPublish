using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed.LocalPca
{

    [Title("Knn Query Based Local PCA Preprocessor")]
    [Description("Materializes the local PCA and the locally weighted matrix of objects of a database. The PCA is based on k nearest neighbor queries.")]
    public class KNNQueryFilteredPCAIndex : AbstractFilteredPCAIndex<INumberVector>
    {
        /**
         * Logger.
         */
        private static Logging logger = Logging.GetLogger(typeof(KNNQueryFilteredPCAIndex));

        /**
         * The kNN query instance we use
         */
        private IKNNQuery knnQuery;

        /**
         * Query k
         */
        private int k;

        /**
         * Constructor.
         * 
         * @param database Database to use
         * @param pca PCA Runner to use
         * @param knnQuery KNN Query to use
         * @param k k value
         */
        public KNNQueryFilteredPCAIndex(IRelation database,
            PCAFilteredRunner<INumberVector> pca, IKNNQuery knnQuery, int k) :
            base(database, pca)
        {
            this.knnQuery = knnQuery;
            this.k = k;
        }


        protected override IEnumerable<IDistanceDbIdPair> ObjectsForPCA(IDbId id)
        {
            return knnQuery.GetKNNForDbId(id, k);
        }


        public override String LongName
        {
            get { return "kNN-based local filtered PCA"; }
        }


        public override String ShortName
        {
            get { return "kNNFilteredPCA"; }
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.landmark
         * @apiviz.uses KNNQueryFilteredPCAIndex oneway - - 芦create禄
         */
        public new class Factory : AbstractFilteredPCAIndex<INumberVector>.Factory
        {
            /**
             * Optional parameter to specify the number of nearest neighbors considered
             * in the PCA, must be an integer greater than 0. If this parameter is not
             * set, k is set to three times of the dimensionality of the database
             * objects.
             * <p>
             * Key: {@code -localpca.k}
             * </p>
             * <p>
             * Default value: three times of the dimensionality of the database objects
             * </p>
             */
            public static OptionDescription K_ID = OptionDescription.GetOrCreate(
                "localpca.k", "The number of nearest neighbors considered in the PCA. " +
                "If this parameter is not set, k ist set to three " +
                "times of the dimensionality of the database objects.");

            /**
             * Holds the value of {@link #K_ID}.
             */
            private int k = 0;

            /**
             * Constructor.
             * 
             * @param pcaDistanceFunction distance
             * @param pca PCA class
             * @param k k
             */
            public Factory(IDistanceFunction pcaDistanceFunction, PCAFilteredRunner<INumberVector> pca, int k) :
                base(pcaDistanceFunction, pca)
            {
                this.k = k;
            }


            public override IIndex Instantiate(IRelation relation)
            {
                // TODO: set bulk flag, once the parent class supports bulk.
                IKNNQuery knnquery = QueryUtil.GetKNNQuery(relation, pcaDistanceFunction, k);
                return new KNNQueryFilteredPCAIndex(relation, pca, knnquery, k);
            }

            /**
             * Parameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public new class Parameterizer : AbstractFilteredPCAIndex<INumberVector>.Factory.Parameterizer
            {
                protected int k = 0;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    IntParameter kP = new IntParameter(K_ID, new GreaterConstraint<int>(0), true);
                    if (config.Grab(kP))
                    {
                        k = kP.GetValue();
                    }
                }


                protected override object MakeInstance()
                {
                    return new Factory(pcaDistanceFunction, pca, k);
                }
            }
        }
    }
}
