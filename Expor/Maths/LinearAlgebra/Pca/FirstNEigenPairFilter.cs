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

    // todo parameter comments
    [Title("First n Eigenpair filter")]
    [Description("Sorts the eigenpairs in decending order of their eigenvalues and marks the first n eigenpairs as strong eigenpairs.")]
    public class FirstNEigenPairFilter : IEigenPairFilter
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(FirstNEigenPairFilter));

        /**
         * Paremeter n
         */
        public static OptionDescription EIGENPAIR_FILTER_N = OptionDescription.GetOrCreate("pca.filter.n", "The number of strong eigenvectors: n eigenvectors with the n highest" + "eigenvalues are marked as strong eigenvectors.");

        /**
         * The threshold for strong eigenvectors: n eigenvectors with the n highest
         * eigenvalues are marked as strong eigenvectors.
         */
        private int n;

        /**
         * Constructor.
         * 
         * @param n
         */
        public FirstNEigenPairFilter(int n) :
            base()
        {
            this.n = n;
        }


        public FilteredEigenPairs Filter(SortedEigenPairs eigenPairs)
        {
            StringBuilder msg = new StringBuilder();
            if (logger.IsDebugging)
            {
                msg.Append("sortedEigenPairs ").Append(eigenPairs.ToString());
                msg.Append("\nn = ").Append(n);
            }

            // init strong and weak eigenpairs
            List<EigenPair> strongEigenPairs = new List<EigenPair>();
            List<EigenPair> weakEigenPairs = new List<EigenPair>();

            // determine strong and weak eigenpairs
            for (int i = 0; i < eigenPairs.Count; i++)
            {
                EigenPair eigenPair = eigenPairs.GetEigenPair(i);
                if (i < n)
                {
                    strongEigenPairs.Add(eigenPair);
                }
                else
                {
                    weakEigenPairs.Add(eigenPair);
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
         * IParameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {
            /**
             * The number of eigenpairs to keep.
             */
            protected int n = 0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter nP = new IntParameter(EIGENPAIR_FILTER_N, new GreaterEqualConstraint<int>(0));
                if (config.Grab(nP))
                {
                    n = nP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new FirstNEigenPairFilter(n);
            }
        }
    }
}
