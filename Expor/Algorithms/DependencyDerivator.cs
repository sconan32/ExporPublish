using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Algorithms
{

    [Title("Dependency Derivator: Deriving numerical inter-dependencies on data")]
    [Description("Derives an equality-system describing dependencies between attributes in a correlation-cluster")]
    [Reference(Authors = "E. Achtert, C. B枚hm, H.-P. Kriegel, P. Kr枚ger, A. Zimek",
        Title = "Deriving Quantitative Dependencies for Correlation Clusters",
        BookTitle = "Proc. 12th Int. Conf. on Knowledge Discovery and Data Mining (KDD '06), Philadelphia, PA 2006.",
        Url = "http://dx.doi.org/10.1145/1150402.1150408")]
    public class DependencyDerivator : AbstractPrimitiveDistanceBasedAlgorithm<INumberVector>
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(DependencyDerivator));

        /**
         * ParameterFlag to use random sample (use knn query around Centroid, if flag is not
         * set).
         */
        public static OptionDescription DEPENDENCY_DERIVATOR_RANDOM_SAMPLE = OptionDescription.GetOrCreate(
            "derivator.randomSample", "ParameterFlag to use random sample (use knn query around Centroid, if flag is not set).");

        /**
         * Parameter to specify the threshold for output accuracy fraction digits,
         * must be an integer equal to or greater than 0.
         */
        public static OptionDescription OUTPUT_ACCURACY_ID = OptionDescription.GetOrCreate(
            "derivator.accuracy", "Threshold for output accuracy fraction digits.");

        /**
         * Optional parameter to specify the threshold for the size of the random
         * sample to use, must be an integer greater than 0.
         * <p/>
         * Default value: the size of the complete dataset
         * </p>
         */
        public static OptionDescription SAMPLE_SIZE_ID = OptionDescription.GetOrCreate(
            "derivator.sampleSize", "Threshold for the size of the random sample to use. " +
            "Default value is size of the complete dataset.");

        /**
         * Holds the value of {@link #SAMPLE_SIZE_ID}.
         */
        private int sampleSize;

        /**
         * Holds the object performing the pca.
         */
        private PCAFilteredRunner<INumberVector> pca;

        /**
         * Number Format for output of solution.
         */
        public NumberFormatInfo NF;

        /**
         * ParameterFlag for random sampling vs. kNN
         */
        private bool randomsample;

        /**
         * Constructor.
         * 
         * @param distanceFunction distance function
         * @param nf Number Format
         * @param pca PCA runner
         * @param sampleSize sample size
         * @param randomsample flag for random sampling
         */
        public DependencyDerivator(IPrimitiveDistanceFunction<INumberVector> distanceFunction,
            NumberFormatInfo nf, PCAFilteredRunner<INumberVector> pca, int sampleSize, bool randomsample) :
            base(distanceFunction)
        {
            this.NF = nf;
            this.pca = pca;
            this.sampleSize = sampleSize;
            this.randomsample = randomsample;
        }

        /**
         * Computes quantitatively linear dependencies among the attributes of the
         * given database based on a linear correlation PCA.
         * 
         * @param database the database to run this DependencyDerivator on
         * @param relation the relation to use
         * @return the CorrelationAnalysisSolution computed by this
         *         DependencyDerivator
         */
        public CorrelationAnalysisSolution<INumberVector> Run(IDatabase database, IRelation relation)
        {
            if (logger.IsVerbose)
            {
                logger.Verbose("retrieving database objects...");
            }
            INumberVector centroidDV = DatabaseUtil.Centroid<INumberVector>(relation);
            IDbIds ids;
            if (this.sampleSize > 0)
            {
                if (randomsample)
                {
                    ids = DbIdUtil.RandomSample(relation.GetDbIds(), this.sampleSize, 1L);
                }
                else
                {
                    IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, GetDistanceFunction());
                    IKNNList queryResults = database.GetKNNQuery(distanceQuery, this.sampleSize).GetKNNForObject(centroidDV, this.sampleSize);
                    ids = DbIdUtil.NewHashSet(queryResults.ToDbIds());
                }
            }
            else
            {
                ids = relation.GetDbIds();
            }

            return generateModel(relation, ids, centroidDV);
        }

        /**
         * Runs the pca on the given set of IDs. The Centroid is computed from the
         * given ids.
         * 
         * @param db the database
         * @param ids the set of ids
         * @return a matrix of equations describing the dependencies
         */
        public CorrelationAnalysisSolution<INumberVector> generateModel(IRelation db, IDbIds ids)
        {
            INumberVector centroidDV = DatabaseUtil.Centroid<INumberVector>(db, ids);
            return generateModel(db, ids, centroidDV);
        }

        /**
         * Runs the pca on the given set of IDs and for the given Centroid.
         * 
         * @param db the database
         * @param ids the set of ids
         * @param centroidDV the Centroid
         * @return a matrix of equations describing the dependencies
         */
        public CorrelationAnalysisSolution<INumberVector> generateModel(IRelation db, IDbIds ids, INumberVector centroidDV)
        {
            CorrelationAnalysisSolution<INumberVector> sol;
            if (logger.IsDebugging)
            {
                logger.Debug("PCA...");
            }

            PCAFilteredResult pcares = (PCAFilteredResult)pca.ProcessIds(ids, db);
            // Matrix weakEigenvectors =
            // pca.Eigenvectors.times(pca.selectionMatrixOfWeakEigenvectors());
            Matrix weakEigenvectors = pcares.GetWeakEigenvectors();
            // Matrix strongEigenvectors =
            // pca.Eigenvectors.times(pca.selectionMatrixOfStrongEigenvectors());
            Matrix strongEigenvectors = pcares.GetStrongEigenvectors();
            Vector Centroid = centroidDV.GetColumnVector();

            // TODO: what if we don't have any weak eigenvectors?
            if (weakEigenvectors.ColumnCount == 0)
            {
                sol = new CorrelationAnalysisSolution<INumberVector>(null, db, strongEigenvectors,
                    weakEigenvectors, pcares.SimilarityMatrix(), Centroid);
            }
            else
            {
                Matrix transposedWeakEigenvectors = (Matrix)weakEigenvectors.Transpose();
                if (logger.IsDebugging)
                {
                    StringBuilder log = new StringBuilder();
                    log.Append("Strong Eigenvectors:\n");
                    log.Append(FormatUtil.Format(pcares.Eigenvectors * (pcares.selectionMatrixOfStrongEigenvectors()), NF)).Append('\n');
                    log.Append("Transposed weak Eigenvectors:\n");
                    log.Append(FormatUtil.Format(transposedWeakEigenvectors, NF)).Append('\n');
                    log.Append("Eigenvalues:\n");
                    log.Append(FormatUtil.Format(pcares.Eigenvalues, " , ", 2));
                    logger.Debug(log.ToString());
                }
                Vector B = (Vector)(transposedWeakEigenvectors * Centroid);
                if (logger.IsDebugging)
                {
                    StringBuilder log = new StringBuilder();
                    log.Append("Centroid:\n").Append(Centroid).Append('\n');
                    log.Append("tEV * Centroid\n");
                    log.Append(B);
                    logger.Debug(log.ToString());
                }

                // +1 == + B.ColumnCount
                Matrix gaussJordan = new Matrix(transposedWeakEigenvectors.RowCount, transposedWeakEigenvectors.ColumnCount + 1);
                gaussJordan.SetMatrix(0, transposedWeakEigenvectors.RowCount - 1, 0, transposedWeakEigenvectors.ColumnCount - 1, transposedWeakEigenvectors);
                gaussJordan.SetColumn(transposedWeakEigenvectors.ColumnCount, B);

                if (logger.IsDebugging)
                {
                    logger.Debug("Gauss-Jordan-Elimination of " + FormatUtil.Format(gaussJordan, NF));
                }

                double[,] a = new double[transposedWeakEigenvectors.RowCount, transposedWeakEigenvectors.ColumnCount];
                double[,] we = transposedWeakEigenvectors.GetArrayRef();
                double[] b = B.GetArrayRef();
                Array.Copy(we, 0, a, 0, transposedWeakEigenvectors.RowCount);

                LinearEquationSystem lq = new LinearEquationSystem(a, b);
                lq.SolveByTotalPivotSearch();

                sol = new CorrelationAnalysisSolution<INumberVector>(lq, db, strongEigenvectors, pcares.GetWeakEigenvectors(), pcares.SimilarityMatrix(), Centroid);

                if (logger.IsDebugging)
                {
                    StringBuilder log = new StringBuilder();
                    log.Append("Solution:\n");
                    log.Append("Standard deviation ").Append(sol.GetStandardDeviation());
                    log.Append(lq.EquationsToString(NF.NumberDecimalDigits));
                    logger.Debug(log.ToString());
                }
            }
            return sol;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.NUMBER_VECTOR_FIELD);
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * IParameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPrimitiveDistanceBasedAlgorithm<INumberVector>.Parameterizer
        {
            protected int outputAccuracy = 0;

            protected int sampleSize = 0;

            protected bool randomSample = false;

            protected PCAFilteredRunner<INumberVector> pca = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigAccuracy(config);
                ConfigSampleSize(config);
                ConfigRandomSampleFlag(config);
                Type cls = ClassGenericsUtil.UglyCastIntoSubclass(typeof(PCAFilteredRunner<INumberVector>));
                pca = config.TryInstantiate<PCAFilteredRunner<INumberVector>>(cls);
            }

            public void ConfigRandomSampleFlag(IParameterization config)
            {
                BoolParameter randomSampleF = new BoolParameter(DEPENDENCY_DERIVATOR_RANDOM_SAMPLE);
                if (config.Grab(randomSampleF))
                {
                    randomSample = randomSampleF.GetValue();
                }
            }

            public void ConfigSampleSize(IParameterization config)
            {
                IntParameter sampleSizeP = new IntParameter(SAMPLE_SIZE_ID, true);
                sampleSizeP.AddConstraint(new GreaterConstraint<int>(0));
                if (config.Grab(sampleSizeP))
                {
                    sampleSize = sampleSizeP.GetValue();
                }
            }

            public void ConfigAccuracy(IParameterization config)
            {
                IntParameter outputAccuracyP = new IntParameter(OUTPUT_ACCURACY_ID, 4);
                outputAccuracyP.AddConstraint(new GreaterEqualConstraint<int>(0));
                if (config.Grab(outputAccuracyP))
                {
                    outputAccuracy = outputAccuracyP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                NumberFormatInfo NF = new NumberFormatInfo();
                NF.NumberDecimalDigits = (outputAccuracy);


                return new DependencyDerivator(distanceFunction, NF, pca, sampleSize, randomSample);
            }
        }
    }
}
