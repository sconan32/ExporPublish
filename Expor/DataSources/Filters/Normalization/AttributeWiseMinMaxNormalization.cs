using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.DataSources.Filters.Normalization
{

    /**
     * Class to perform and undo a normalization on real vectors with respect to
     * given minimum and maximum in each dimension.
     * 
     * @author Elke Achtert
     * @param <V> vector type
     * 
     * @apiviz.uses NumberVector
     */
    // TODO: extract superclass AbstractAttributeWiseNormalization
    public class AttributeWiseMinMaxNormalization: AbstractNormalization<INumberVector>
   
    {
        /**
         * Class logger.
         */
        private static Logging LOG = Logging.GetLogger(typeof(AttributeWiseMinMaxNormalization));

        /**
         * Parameter for minimum.
         */
        public static OptionDescription MINIMA_ID = new OptionDescription("normalize.min",
            "a comma separated concatenation of the minimum values in each dimension that are mapped to 0. If no value is specified, the minimum value of the attribute range in this dimension will be taken.");

        /**
         * Parameter for maximum.
         */
        public static OptionDescription MAXIMA_ID = new OptionDescription("normalize.max",
            "a comma separated concatenation of the maximum values in each dimension that are mapped to 1. If no value is specified, the maximum value of the attribute range in this dimension will be taken.");

        /**
         * Stores the maximum in each dimension.
         */
        private double[] maxima = new double[0];

        /**
         * Stores the minimum in each dimension.
         */
        private double[] minima = new double[0];

        /**
         * Constructor.
         * 
         * @param minima Minimum values
         * @param maxima Maximum values
         */
        public AttributeWiseMinMaxNormalization(double[] minima, double[] maxima)
        {

            this.minima = minima;
            this.maxima = maxima;
        }


        protected override bool PrepareStart(SimpleTypeInformation tin)
        {
            return (minima.Length == 0 || maxima.Length == 0);
        }


        protected override void PrepareProcessInstance(INumberVector featureVector)
        {
            // First object? Then initialize.
            if (minima.Length == 0 || maxima.Length == 0)
            {
                int dimensionality = featureVector.Count;
                minima = new double[dimensionality];
                maxima = new double[dimensionality];
                for (int i = 0; i < dimensionality; i++)
                {
                    maxima[i] = -Double.MaxValue;
                    minima[i] = Double.MaxValue;
                }
            }
            if (minima.Length != featureVector.Count)
            {
                throw new ArgumentException("FeatureVectors differ in length.");
            }
            for (int d = 0; d < featureVector.Count; d++)
            {
                double val = featureVector[(d)];
                if (val > maxima[d])
                {
                    maxima[d] = val;
                }
                if (val < minima[d])
                {
                    minima[d] = val;
                }
            }
        }


        protected override INumberVector FilterSingleObject(INumberVector featureVector)
        {
            double[] values = new double[featureVector.Count];
            if (minima.Length != featureVector.Count)
            {
                throw new ArgumentException("FeatureVectors and given Minima/Maxima differ in length.");
            }
            for (int d = 0; d < featureVector.Count; d++)
            {
                values[d] = (featureVector[(d)] - minima[d]) / Factor(d);
            }
            return factory.NewNumberVector(values);
        }


        public override INumberVector Restore(INumberVector featureVector)
        {
            if (featureVector.Count == maxima.Length && featureVector.Count == minima.Length)
            {
                double[] values = new double[featureVector.Count];
                for (int d = 0; d < featureVector.Count; d++)
                {
                    values[d] = (featureVector[(d)] * (Factor(d)) + minima[d]);
                }
                return factory.NewNumberVector(values);
            }
            else
            {
                throw new NonNumericFeaturesException("Attributes cannot be resized: current dimensionality: " +
                    featureVector.Count + " former dimensionality: " + maxima.Length);
            }
        }

        /**
         * Returns a factor for normalization in a certain dimension.
         * <p/>
         * The provided factor is the maximum-minimum in the specified dimension, if
         * these two values differ, otherwise it is the maximum if this value differs
         * from 0, otherwise it is 1.
         * 
         * @param dimension the dimension to get a factor for normalization
         * @return a factor for normalization in a certain dimension
         */
        private double Factor(int dimension)
        {
            return maxima[dimension] > minima[dimension] ? maxima[dimension] - minima[dimension] : maxima[dimension] > 0 ? maxima[dimension] : 1;
        }


        public override LinearEquationSystem Transform(LinearEquationSystem linearEquationSystem)
        {
            double[,] coeff = linearEquationSystem.GetCoefficents();
            double[] rhs = linearEquationSystem.GetRHS();
            int[] row = linearEquationSystem.GetRowPermutations();
            int[] col = linearEquationSystem.GetColumnPermutations();

            for (int i = 0; i < coeff.Length; i++)
            {
                for (int r = 0; r < coeff.Length; r++)
                {
                    double sum = 0.0;
                    for (int c = 0; c < coeff.GetLength(1); c++)
                    {
                        sum += minima[c] * coeff[row[r], col[c]] / Factor(c);
                        coeff[row[r], col[c]] = coeff[row[r], col[c]] / Factor(c);
                    }
                    rhs[row[r]] = rhs[row[r]] + sum;
                }
            }

            LinearEquationSystem lq = new LinearEquationSystem(coeff, rhs, row, col);
            return lq;
        }


        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("normalization class: ").Append(GetType().Name);
            result.Append('\n');
            result.Append("normalization minima: ").Append(FormatUtil.Format(minima));
            result.Append('\n');
            result.Append("normalization maxima: ").Append(FormatUtil.Format(maxima));
            return result.ToString();
        }


        protected override SimpleTypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.NUMBER_VECTOR_FIELD;
        }


        protected override Logging GetLogger()
        {
            return LOG;
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
             * Stores the maximum in each dimension.
             */
            private double[] maxima = new double[0];

            /**
             * Stores the minimum in each dimension.
             */
            private double[] minima = new double[0];


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleListParameter minimaP = new DoubleListParameter(MINIMA_ID, true);
                if (config.Grab(minimaP))
                {
                    minima = minimaP.GetValue().ToArray();
                }
                DoubleListParameter maximaP = new DoubleListParameter(MAXIMA_ID, true);
                if (config.Grab(maximaP))
                {
                    maxima = maximaP.GetValue().ToArray();
                }

                config.CheckConstraint(new AllOrNoneMustBeSetGlobalConstraint<IList<double>>(minimaP, maximaP));
                config.CheckConstraint(new EqualSizeGlobalConstraint<double>(minimaP, maximaP));
            }


            protected override object MakeInstance()
            {
                return new AttributeWiseMinMaxNormalization(minima, maxima);
            }
        }

        public override Bundles.MultipleObjectsBundle NormalizeObjects(Bundles.MultipleObjectsBundle objects)
        {
            throw new NotImplementedException();
        }
    }

}
