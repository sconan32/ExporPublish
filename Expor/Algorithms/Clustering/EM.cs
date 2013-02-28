using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering.KMeans;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Maths;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Algorithms.Clustering
{

    [Title("EM-Clustering: Clustering by Expectation Maximization")]
    [Description("Provides k Gaussian mixtures maximizing the probability of the given data")]
    [Reference(Authors = "A. P. Dempster, N. M. Laird, D. B. Rubin",
        Title = "Maximum Likelihood from Incomplete Data via the EM algorithm",
        BookTitle = "Journal of the Royal Statistical Society, Series B, 39(1), 1977, pp. 1-31",
        Url = "http://www.jstor.org/stable/2984875")]

    public class EM : AbstractAlgorithm, IClusteringAlgorithm
    {

        private static Logging logger = Logging.GetLogger(typeof(EM));

        /**
         * Small value to increment diagonally of a matrix in order to avoid
         * singularity before building the inverse.
         */


        private static double SINGULARITY_CHEAT = 1E-9;

        /**
         * Parameter to specify the number of clusters to find, must be an integer
         * greater than 0.
         */
        public static OptionDescription K_ID =
            OptionDescription.GetOrCreate("em.k", "The number of clusters to find.");

        /**
         * Holds the value of {@link #K_ID}.
         */
        private int k;

        /**
         * Parameter to specify the termination criterion for maximization of E(M):
         * E(M) - E(M') < em.delta, must be a double equal to or greater than 0.
         */
        public static OptionDescription DELTA_ID =
            OptionDescription.GetOrCreate("em.delta", "The termination criterion for maximization of E(M): " + "E(M) - E(M') < em.delta");

        /**
         * Parameter to specify the initialization method
         */
        public static OptionDescription INIT_ID =
            OptionDescription.GetOrCreate("kmeans.initialization", "Method to choose the initial means.");

        private static double MIN_LOGLIKELIHOOD = -100000;

        /**
         * Holds the value of {@link #DELTA_ID}.
         */
        private double delta;

        /**
         * Store the individual probabilities, for use by EMOutlierDetection etc.
         */
        private IWritableDataStore<double[]> probClusterIGivenX;

        /**
         * Class to choose the initial means
         */
        private IKMeansInitialization<INumberVector> initializer;

        /**
         * Constructor.
         * 
         * @param k k parameter
         * @param delta delta parameter
         * @param initializer Class to choose the initial means
         */
        public EM(int k, double delta, IKMeansInitialization<INumberVector> initializer)
            : base()
        {

            this.k = k;
            this.delta = delta;
            this.initializer = initializer;
        }

        /**
         * Performs the EM clustering algorithm on the given database.
         * <p/>
         * Finally a hard clustering is provided where each clusters gets assigned the
         * points exhibiting the highest probability to belong to this cluster. But
         * still, the database objects hold associated the complete probability-vector
         * for all models.
         * 
         * @param database Database
         * @param relation Relation
         * @return Result
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            if (relation.Count == 0)
            {
                throw new ArgumentException("database empty: must contain elements");
            }
            // initial models
            if (logger.IsVerbose)
            {
                logger.Verbose("initializing " + k + " models");
            }
            IList<Vector> means = new List<Vector>();
            foreach (INumberVector nv in
                initializer.ChooseInitialMeans(relation, k, EuclideanDistanceFunction.STATIC))
            {
                means.Add(nv.GetColumnVector());
            }
            IList<Matrix> covarianceMatrices = new List<Matrix>(k);
            double[] normDistrFactor = new double[k];
            IList<Matrix> invCovMatr = new List<Matrix>(k);
            double[] clusterWeights = new double[k];
            probClusterIGivenX = DataStoreUtil.MakeStorage<double[]>(relation.GetDbIds(), (DataStoreHints.Hot | DataStoreHints.Sorted), typeof(double[]));

            int dimensionality = means[0].Count;
            for (int i = 0; i < k; i++)
            {
                Matrix m = Matrix.Identity(dimensionality, dimensionality);
                covarianceMatrices.Add(m);
                normDistrFactor[i] = 1.0 / Math.Sqrt(Math.Pow(MathUtil.TWOPI, dimensionality) * m.Determinant());
                invCovMatr.Add(m.Inverse());
                clusterWeights[i] = 1.0 / k;
                if (logger.IsDebugging)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append(" model ").Append(i).Append(":\n");
                    msg.Append(" mean:    ").Append(means[i]).Append("\n");
                    msg.Append(" m:\n").Append(FormatUtil.Format(m, "        ")).Append("\n");
                    msg.Append(" m.det(): ").Append(m.Determinant()).Append("\n");
                    msg.Append(" cluster weight: ").Append(clusterWeights[i]).Append("\n");
                    msg.Append(" normDistFact:   ").Append(normDistrFactor[i]).Append("\n");
                    logger.Debug(msg.ToString());
                }
            }
            double emNew = AssignProbabilitiesToInstances(relation, normDistrFactor,
                means, invCovMatr, clusterWeights, probClusterIGivenX);

            // iteration unless no change
            if (logger.IsVerbose)
            {
                logger.Verbose("iterating EM");
            }

            double em;
            int it = 0;
            do
            {
                it++;
                if (logger.IsVerbose)
                {
                    logger.Verbose("iteration " + it + " - expectation value: " + emNew);
                }
                em = emNew;

                // recompute models
                IList<Vector> meanSums = new List<Vector>(k);
                double[] sumOfClusterProbabilities = new double[k];

                for (int i = 0; i < k; i++)
                {
                    clusterWeights[i] = 0.0;
                    meanSums.Add(new Vector(dimensionality));
                    covarianceMatrices[i] = new Matrix(dimensionality);
                }

                // weights and means
                foreach (IDbId id in relation.GetDbIds())
                {

                    double[] clusterProbabilities = (double[])probClusterIGivenX[id];

                    for (int i = 0; i < k; i++)
                    {
                        sumOfClusterProbabilities[i] += clusterProbabilities[i];
                        Vector summand = (relation[id] as INumberVector).GetColumnVector() * clusterProbabilities[i];
                        meanSums[i] = meanSums[i] + summand;
                    }
                }
                int n = relation.Count;
                for (int i = 0; i < k; i++)
                {
                    clusterWeights[i] = sumOfClusterProbabilities[i] / n;
                    meanSums[i] *= (1 / sumOfClusterProbabilities[i]);
                    Vector newMean = meanSums[i];
                    means[i] = newMean;
                }
                // covariance matrices
                foreach (IDbId id in relation.GetDbIds())
                {
                    //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                    double[] clusterProbabilities = (double[])probClusterIGivenX[(id)];
                    Vector instance = (relation[id] as INumberVector).GetColumnVector();
                    for (int i = 0; i < k; i++)
                    {
                        Vector difference = instance - means[i];
                        Matrix tmp = difference.TimesTranspose(difference) * clusterProbabilities[i];
                        covarianceMatrices[i] += tmp;
                    }
                }
                for (int i = 0; i < k; i++)
                {
                    covarianceMatrices[i] = (covarianceMatrices[i] * (1 / sumOfClusterProbabilities[i])).CheatToAvoidSingularity(SINGULARITY_CHEAT);
                }
                for (int i = 0; i < k; i++)
                {
                    normDistrFactor[i] = 1.0 / Math.Sqrt(Math.Pow(MathUtil.TWOPI, dimensionality) * covarianceMatrices[i].Determinant());
                    invCovMatr[i] = covarianceMatrices[i].Inverse();
                }
                // reassign probabilities
                emNew = AssignProbabilitiesToInstances(relation, normDistrFactor,
                    means, invCovMatr, clusterWeights, probClusterIGivenX);
            } while (Math.Abs(em - emNew) > delta);

            if (logger.IsVerbose)
            {
                logger.Verbose("assigning clusters");
            }

            // fill result with clusters and models
            IList<IModifiableDbIds> hardClusters = new List<IModifiableDbIds>(k);
            for (int i = 0; i < k; i++)
            {
                hardClusters.Add(DbIdUtil.NewHashSet());
            }

            // provide a hard clustering
            foreach (IDbId id in relation.GetDbIds())
            {
                // for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                double[] clusterProbabilities = (double[])probClusterIGivenX[id];
                int maxIndex = 0;
                double currentMax = 0.0;
                for (int i = 0; i < k; i++)
                {
                    if (clusterProbabilities[i] > currentMax)
                    {
                        maxIndex = i;
                        currentMax = clusterProbabilities[i];
                    }
                }
                hardClusters[maxIndex].Add(id);
            }
            INumberVector factory = DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory();
            ClusterList result = new ClusterList("EM Clustering", "em-clustering");
            // provide models within the result
            for (int i = 0; i < k; i++)
            {
                // TODO: re-do labeling.
                // SimpleClassLabel label = new SimpleClassLabel();
                // label.init(result.canonicalClusterLabel(i));
                Cluster model = new Cluster(hardClusters[i], new EMModel<INumberVector>(factory.NewNumberVector(means[i].GetArrayRef()), covarianceMatrices[i]));
                result.AddCluster(model);
            }
            return result;
        }


        /// <summary>
        /// Assigns the current probability values to the instances in the database and
        ///compute the expectation value of the current mixture of distributions.
        ///  
        /// Computed as the sum of the logarithms of the prior probability of each
        /// instance.
        /// </summary>
        /// <param name="database"> the database used for assignment to instances</param>
        /// <param name="normDistrFactor"> normDistrFactor normalization factor for density function, based on current covariance matrix</param>
        /// <param name="means">the current means</param>
        /// <param name="invCovMatr">the inverse covariance matrices</param>
        /// <param name="clusterWeights">the weights of the current clusters</param>
        /// <param name="probClusterIGivenX"></param>
        /// <returns>the expectation value of the current mixture of distributions</returns>
        protected double AssignProbabilitiesToInstances(
            IRelation database,
            double[] normDistrFactor,
            IList<Vector> means,
            IList<Matrix> invCovMatr,
            double[] clusterWeights,
            IWritableDataStore<double[]> probClusterIGivenX)
        {
            double emSum = 0.0;

            foreach (IDbId id in database.GetDbIds())
            //for (DbIdIter iditer = database.iterDbIds(); iditer.valid(); iditer.advance())
            {
                Vector x = (database[id] as INumberVector).GetColumnVector();
                double[] probabilities = new double[k];
                for (int i = 0; i < k; i++)
                {
                    Vector difference = x - (means[i]);
                    //计算方差（当invcovmar[i]是单位阵的时候。
                    double rowTimesCovTimesCol = difference.TransposeTimesTimes(invCovMatr[i], difference);
                    double power = rowTimesCovTimesCol / 2.0;
                    double prob = normDistrFactor[i] * Math.Exp(-power);
                    //if (logger.IsDebugging)
                    //{
                    //    logger.Debug("ID is "+id.ToString()+"\ndifference vector :=\n ( " + FormatUtil.Format(difference)+ " )\n" +
                    //        " difference:=\n" + FormatUtil.Format(difference, "    ") + "\n" +
                    //        " rowTimesCovTimesCol  := " + rowTimesCovTimesCol + "\n" +
                    //        " power\t:= " + power + "\n" + " prob\t:= " + prob + "\n" + " inv cov matrix: \n" +
                    //        FormatUtil.Format(invCovMatr[i], "     "));
                    //}
                    probabilities[i] = prob;
                }
                double priorProbability = 0.0;
                for (int i = 0; i < k; i++)
                {
                    priorProbability += probabilities[i] * clusterWeights[i];
                }
                double logP = Math.Max(Math.Log(priorProbability), MIN_LOGLIKELIHOOD);
                if (!Double.IsNaN(logP))
                {
                    emSum += logP;
                }

                double[] clusterProbabilities = new double[k];
                for (int i = 0; i < k; i++)
                {
                    Debug.Assert(priorProbability >= 0.0);
                    Debug.Assert(clusterWeights[i] >= 0.0);
                    // do not divide by zero!
                    if (priorProbability == 0.0)
                    {
                        clusterProbabilities[i] = 0.0;
                    }
                    else
                    {
                        clusterProbabilities[i] = probabilities[i] / priorProbability * clusterWeights[i];
                    }
                }
                probClusterIGivenX[id] = clusterProbabilities;
            }

            return emSum;
        }

        /**
         * Compute the inverse cluster matrices.
         * 
         * @param covarianceMatrices Input covariance matrices
         * @param invCovMatr Output array for inverse matrices
         * @param normDistrFactor Output array for norm distribution factors.
         * @param norm Normalization factor, usually (2pi)^d
         */
        public static void ComputeInverseMatrixes(Matrix[] covarianceMatrices, Matrix[] invCovMatr, double[] normDistrFactor, double norm)
        {
            int k = covarianceMatrices.Length;
            for (int i = 0; i < k; i++)
            {
                double det = covarianceMatrices[i].Determinant();
                if (det > 0.0)
                {
                    normDistrFactor[i] = 1.0 / Math.Sqrt(norm * det);
                }
                else
                {
                    logger.Warning("Encountered matrix with 0 determinant - degenerated.");
                    normDistrFactor[i] = 1.0; // Not really well defined
                }
                invCovMatr[i] = covarianceMatrices[i].Inverse();
            }
        }

        /**
         * Recompute the covariance matrixes.
         * 
         * @param relation Vector data
         * @param probClusterIGivenX Object probabilities
         * @param means Cluster means output
         * @param covarianceMatrices Output covariance matrixes
         * @param dimensionality Data set dimensionality
         */
        public static void RecomputeCovarianceMatrices(IRelation relation, IWritableDataStore<double[]> probClusterIGivenX,
            Vector[] means, Matrix[] covarianceMatrices, int dimensionality)
        {
            int k = means.Length;
            CovarianceMatrix[] cms = new CovarianceMatrix[k];
            for (int i = 0; i < k; i++)
            {
                cms[i] = new CovarianceMatrix(dimensionality);
            }
            //for(DBIDIter iditer = relation.iterDBIDs(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation)
            {
                double[] clusterProbabilities = probClusterIGivenX[(iditer)];
                Vector instance = relation.VectorAt(iditer).GetColumnVector();
                for (int i = 0; i < k; i++)
                {
                    if (clusterProbabilities[i] > 0.0)
                    {
                        cms[i].Put(instance, clusterProbabilities[i]);
                    }
                }
            }
            for (int i = 0; i < k; i++)
            {
                if (cms[i].GetWeight() <= 0.0)
                {
                    means[i] = new Vector(dimensionality);
                    covarianceMatrices[i] = Matrix.Identity(dimensionality, dimensionality);
                }
                else
                {
                    means[i] = cms[i].GetMeanVector();
                    covarianceMatrices[i] = cms[i].DestroyToNaiveMatrix().CheatToAvoidSingularity(SINGULARITY_CHEAT);
                }
            }
        }

        /**
         * Assigns the current probability values to the instances in the database and
         * compute the expectation value of the current mixture of distributions.
         * 
         * Computed as the sum of the logarithms of the prior probability of each
         * instance.
         * 
         * @param relation the database used for assignment to instances
         * @param normDistrFactor normalization factor for density function, based on
         *        current covariance matrix
         * @param means the current means
         * @param invCovMatr the inverse covariance matrices
         * @param clusterWeights the weights of the current clusters
         * @return the expectation value of the current mixture of distributions
         */
        public static double AssignProbabilitiesToInstances(IRelation relation, double[] normDistrFactor, Vector[] means, Matrix[] invCovMatr,
            double[] clusterWeights, IWritableDataStore<double[]> probClusterIGivenX)
        {
            int k = clusterWeights.Length;
            double emSum = 0.0;

            //for(DBIDIter iditer = relation.iterDBIDs(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation)
            {
                Vector x = relation.VectorAt(iditer).GetColumnVector();
                double[] probabilities = new double[k];
                for (int i = 0; i < k; i++)
                {
                    Vector difference = x - (means[i]);
                    double rowTimesCovTimesCol = difference.TransposeTimesTimes(invCovMatr[i], difference);
                    double power = rowTimesCovTimesCol / 2.0;
                    double prob = normDistrFactor[i] * Math.Exp(-power);
                    //if (logger.IsDebugging)
                    //{
                    //    logger.Debug(" difference vector= ( " + difference.ToString() + " )\n" + //
                    //    " difference:\n" + FormatUtil.Format(difference, "    ") + "\n" + //
                    //    " rowTimesCovTimesCol:\n" + rowTimesCovTimesCol + "\n" + //
                    //    " power= " + power + "\n" + " prob=" + prob + "\n" + //
                    //    " inv cov matrix: \n" + FormatUtil.Format(invCovMatr[i], "     "));
                    //}
                    if (!(prob >= 0.0))
                    {
                        logger.Warning("Invalid probability: " + prob + " power: " + power + " factor: " + normDistrFactor[i]);
                        prob = 0.0;
                    }
                    probabilities[i] = prob;
                }
                double priorProbability = 0.0;
                for (int i = 0; i < k; i++)
                {
                    priorProbability += probabilities[i] * clusterWeights[i];
                }
                double logP = Math.Max(Math.Log(priorProbability), MIN_LOGLIKELIHOOD);
                if (!Double.IsNaN(logP))
                {
                    emSum += logP;
                }

                double[] clusterProbabilities = new double[k];
                for (int i = 0; i < k; i++)
                {
                    Debug.Assert(clusterWeights[i] >= 0.0);
                    // do not divide by zero!
                    if (priorProbability > 0.0)
                    {
                        clusterProbabilities[i] = probabilities[i] / priorProbability * clusterWeights[i];
                    }
                    else
                    {
                        clusterProbabilities[i] = 0.0;
                    }
                }
                probClusterIGivenX[iditer] = clusterProbabilities;
            }

            return emSum / relation.Count;
        }
        /**
         * Get the probabilities for a given point.
         * 
         * @param index Point ID
         * @return Probabilities of given point
         */
        public double[] GetProbClusterIGivenX(IDbIdRef index)
        {
            return (double[])probClusterIGivenX[(index)];
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
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {
            protected int k;

            protected double delta;

            protected IKMeansInitialization<INumberVector> initializer;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter kP = new IntParameter(K_ID, new GreaterConstraint<int>(0));
                if (config.Grab(kP))
                {
                    k = (int)kP.GetValue();
                }

                ObjectParameter<IKMeansInitialization<INumberVector>> initialP =
                    new ObjectParameter<IKMeansInitialization<INumberVector>>(INIT_ID,
                        typeof(IKMeansInitialization<INumberVector>),
                        typeof(RandomlyGeneratedInitialMeans<INumberVector>));
                if (config.Grab(initialP))
                {
                    initializer = initialP.InstantiateClass(config);
                }

                DoubleParameter deltaP = new DoubleParameter(DELTA_ID, new GreaterEqualConstraint<double>(0.0), 0.0);
                if (config.Grab(deltaP))
                {
                    delta = (double)deltaP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new EM(k, delta, initializer);
            }


        }




    }
}
