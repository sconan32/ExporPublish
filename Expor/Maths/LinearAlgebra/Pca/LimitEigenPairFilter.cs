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

    [Title("Limit-based Eigenpair Filter")]
    [Description("Filters all eigenpairs, which are lower than a given value.")]
    public class LimitEigenPairFilter : IEigenPairFilter
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(LimitEigenPairFilter));

        /**
         * "absolute" Flag
         */
        public static OptionDescription EIGENPAIR_FILTER_ABSOLUTE = OptionDescription.GetOrCreate(
            "pca.filter.absolute", "Flag to mark delta as an absolute value.");

        /**
         * Parameter delta
         */
        public static OptionDescription EIGENPAIR_FILTER_DELTA = OptionDescription.GetOrCreate(
            "pca.filter.delta", "The threshold for strong Eigenvalues. If not otherwise specified, delta " +
            "is a relative value w.r.t. the (absolute) highest Eigenvalues and has to be " +
            "a double between 0 and 1. To mark delta as an absolute value, use " + "the option -" +
            EIGENPAIR_FILTER_ABSOLUTE.Name + ".");

        /**
         * The default value for delta.
         */
        public static double DEFAULT_DELTA = 0.01;

        /**
         * Threshold for strong eigenpairs, can be absolute or relative.
         */
        private double delta;

        /**
         * Indicates whether delta is an absolute or a relative value.
         */
        private bool absolute;

        /**
         * Constructor.
         * 
         * @param delta
         * @param absolute
         */
        public LimitEigenPairFilter(double delta, bool absolute) :
            base()
        {
            this.delta = delta;
            this.absolute = absolute;
        }


        public FilteredEigenPairs Filter(SortedEigenPairs eigenPairs)
        {
            StringBuilder msg = new StringBuilder();
            if (logger.IsDebugging)
            {
                msg.Append("delta = ").Append(delta);
            }

            // determine limit
            double limit;
            if (absolute)
            {
                limit = delta;
            }
            else
            {
                double max = Double.NegativeInfinity;
                for (int i = 0; i < eigenPairs.Count; i++)
                {
                    EigenPair eigenPair = eigenPairs.GetEigenPair(i);
                    double eigenValue = Math.Abs(eigenPair.Eigenvalue);
                    if (max < eigenValue)
                    {
                        max = eigenValue;
                    }
                }
                limit = max * delta;
            }
            if (logger.IsDebugging)
            {
                msg.Append("\nlimit = ").Append(limit);
            }

            // init strong and weak eigenpairs
            List<EigenPair> strongEigenPairs = new List<EigenPair>();
            List<EigenPair> weakEigenPairs = new List<EigenPair>();

            // determine strong and weak eigenpairs
            for (int i = 0; i < eigenPairs.Count; i++)
            {
                EigenPair eigenPair = eigenPairs.GetEigenPair(i);
                double eigenValue = Math.Abs(eigenPair.Eigenvalue);
                if (eigenValue >= limit)
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
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public  class Parameterizer : AbstractParameterizer
        {
            /**
             * Threshold for strong eigenpairs, can be absolute or relative.
             */
            private double delta;

            /**
             * Indicates whether delta is an absolute or a relative value.
             */
            private bool absolute;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                BoolParameter absoluteF = new BoolParameter(EIGENPAIR_FILTER_ABSOLUTE);
                if (config.Grab(absoluteF))
                {
                    absolute = absoluteF.GetValue();
                }

                DoubleParameter deltaP = new DoubleParameter(EIGENPAIR_FILTER_DELTA,
                    new GreaterEqualConstraint<double>(0), DEFAULT_DELTA);
                if (config.Grab(deltaP))
                {
                    delta = deltaP.GetValue();
                    // TODO: make this a global constraint?
                    if (absolute && deltaP.TookDefaultValue())
                    {
                        config.ReportError(new WrongParameterValueException("Illegal parameter setting: " + "Flag " + absoluteF.GetName() + " is set, " + "but no value for " + deltaP.GetName() + " is specified."));
                    }
                }

                // Conditional Constraint:
                // delta must be >= 0 and <= 1 if it's a relative value
                // Since relative or absolute is dependent on the absolute flag this is a
                // global constraint!
                List<IParameterConstraint> cons = new List<IParameterConstraint>();
                // TODO: Keep the constraint here - applies to non-conditional case as
                // well,
                // and is set above.
                IParameterConstraint aboveNull = new GreaterEqualConstraint<double>(0);
                cons.Add(aboveNull);
                IParameterConstraint underOne = new LessEqualConstraint<double>(1);
                cons.Add(underOne);

                IGlobalParameterConstraint gpc = new ParameterFlagGlobalConstraint<Double>(deltaP, cons, absoluteF, false);
                config.CheckConstraint(gpc);
            }


            protected override object MakeInstance()
            {
                return new LimitEigenPairFilter(delta, absolute);
            }
        }
    }
}
