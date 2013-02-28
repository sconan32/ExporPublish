using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    [Title("Percentage based Eigenpair filter")]
    [Description("Sorts the eigenpairs in decending order of their eigenvalues and returns the first eigenpairs, whose sum of eigenvalues is higher than the given percentage of the sum of all eigenvalues.")]
    public class PercentageEigenPairFilter : IEigenPairFilter
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(PercentageEigenPairFilter));

        /**
         * The threshold for 'strong' eigenvectorsin the 'strong' eigenvectors explain
         * a portion of at least alpha of the total variance.
         * <p>
         * Default valuein {@link #DEFAULT_ALPHA}
         * </p>
         * <p>
         * Keyin {@code -pca.filter.alpha}
         * </p>
         */
        public static OptionDescription ALPHA_ID = OptionDescription.GetOrCreate(
            "pca.filter.alpha", "The share (0.0 to 1.0) of variance that needs to be explained by the 'strong' eigenvectors." +
            "The filter class will choose the number of strong eigenvectors by this share.");

        /**
         * The default value for alpha.
         */
        public static double DEFAULT_ALPHA = 0.85;

        /**
         * The threshold for strong eigenvectorsin the strong eigenvectors explain a
         * portion of at least alpha of the total variance.
         */
        private double alpha;

        /**
         * Constructor.
         * 
         * @param alpha
         */
        public PercentageEigenPairFilter(double alpha)
            : base()
        {

            this.alpha = alpha;
        }

        public FilteredEigenPairs Filter(SortedEigenPairs eigenPairs)
        {
            StringBuilder msg = new StringBuilder();
            if (logger.IsDebugging)
            {
                msg.Append("alpha = ").Append(alpha);
                msg.Append("\nsortedEigenPairs = ").Append(eigenPairs);
            }

            // init strong and weak eigenpairs
            List<EigenPair> strongEigenPairs = new List<EigenPair>();
            List<EigenPair> weakEigenPairs = new List<EigenPair>();

            // determine sum of eigenvalues
            double totalSum = 0;
            for (int i = 0; i < eigenPairs.Count; i++)
            {
                EigenPair eigenPair = eigenPairs.GetEigenPair(i);
                totalSum += eigenPair.Eigenvalue;
            }
            if (logger.IsDebugging)
            {
                msg.Append("\ntotalSum = ").Append(totalSum);
            }

            // determine strong and weak eigenpairs
            double currSum = 0;
            bool found = false;
            for (int i = 0; i < eigenPairs.Count; i++)
            {
                EigenPair eigenPair = eigenPairs.GetEigenPair(i);
                currSum += eigenPair.Eigenvalue;
                if (currSum / totalSum >= alpha)
                {
                    if (!found)
                    {
                        found = true;
                        strongEigenPairs.Add(eigenPair);
                    }
                    else
                    {
                        weakEigenPairs.Add(eigenPair);
                    }
                }
                else
                {
                    strongEigenPairs.Add(eigenPair);
                }
            }
            if (logger.IsDebugging)
            {
                msg.Append("\nstrong EigenPairs = ").Append(strongEigenPairs);
                msg.Append("\nweak EigenPairs = ").Append(weakEigenPairs);
                logger.Debug(msg.ToString());
            }

            return new FilteredEigenPairs(weakEigenPairs, strongEigenPairs);
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
            /**
             * The threshold for strong eigenvectorsin the strong eigenvectors explain a
             * portion of at least alpha of the total variance.
             */
            private double alpha;

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleParameter alphaP = new DoubleParameter(ALPHA_ID, new IntervalConstraint<double>(
                    0.0, IntervalConstraint<double>.IntervalBoundary.OPEN, 1.0,
                    IntervalConstraint<double>.IntervalBoundary.OPEN),
                    DEFAULT_ALPHA);
                if (config.Grab(alphaP))
                {
                    alpha = alphaP.GetValue();
                }
            }

            protected override object MakeInstance()
            {
                return new PercentageEigenPairFilter(alpha);
            }
        }
    }
}
