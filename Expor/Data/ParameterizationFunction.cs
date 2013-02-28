using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Data
{

    public class ParameterizationFunction : DoubleVector, ITextWriteable
    {
        /**
         * Available types for the global extremum.
         * 
         * @apiviz.exclude
         */
        public enum ExtremumType
        {
            /**
             * Minimum
             */
            MINIMUM,
            /**
             * Maximum
             */
            MAXIMUM,
            /**
             * Constant
             */
            CONSTANT
        }

        /**
         * Static factory
         */
        public new static ParameterizationFunction STATIC = new ParameterizationFunction(new double[] { 0.0 });

        /**
         * A small number to handle numbers near 0 as 0.
         */
        public static double DELTA = 1E-10;

        /**
         * Holds the alpha values of the global extremum.
         */
        private double[] alphaExtremum;

        /**
         * Holds the type of the global extremum.
         */
        private ExtremumType extremumType;

        /**
         * Provides a new parameterization function describing all lines in a
         * d-dimensional feature space intersecting in one point p.
         * 
         * @param values the values of the point p
         */
        public ParameterizationFunction(double[] values) :
            base(values)
        {
            determineGlobalExtremum();
        }



        /**
         * Provides a new parameterization function describing all lines in a
         * d-dimensional feature space intersecting in one point p.
         * 
         * @param values the values of the point p
         */
        public ParameterizationFunction(List<Double> values) :
            base(values)
        {
            determineGlobalExtremum();
        }

        /**
         * Provides a new parameterization function describing all lines in a
         * d-dimensional feature space intersecting in one point p.
         * 
         * @param columnMatrix the values of the point p
         */
        public ParameterizationFunction(Vector columnMatrix) :
            base(columnMatrix)
        {
            determineGlobalExtremum();
        }

        /**
         * Computes the function value at <code>alpha</code>.
         * 
         * @param alpha the values of the d-1 angles
         * @return the function value at alpha
         */
        public double Function(double[] alpha)
        {
            int d = Count;
            if (alpha.Length != d - 1)
            {
                throw new ArgumentException("Parameter alpha must have a " + "dimensionality of " + (d - 1) + ", read: " + alpha.Length);
            }

            double result = 0;
            for (int i = 0; i < d; i++)
            {
                double alpha_i = i == d - 1 ? 0 : alpha[i];
                result += this[(i + 1)] * sinusProduct(0, i, alpha) * Math.Cos(alpha_i);
            }
            return result;
        }

        /**
         * Determines the alpha values where this function has a minumum and maximum
         * value in the given interval.
         * 
         * @param interval the hyper bounding box defining the interval
         * @return he alpha values where this function has a minumum and maximum value
         *         in the given interval
         */
        public HyperBoundingBox DetermineAlphaMinMax(HyperBoundingBox interval)
        {
            int dim = Count;
            if (interval.Count != dim - 1)
            {
                throw new ArgumentException("Interval needs to have dimensionality d=" + (dim - 1) + ", read: " + interval.Count);
            }

            if (extremumType.Equals(ExtremumType.CONSTANT))
            {
                double[] Centroid = SpatialUtil.Centroid(interval);
                return new HyperBoundingBox(Centroid, Centroid);
            }

            double[] alpha_min = new double[dim - 1];
            double[] alpha_max = new double[dim - 1];

            if (SpatialUtil.Contains(interval, alphaExtremum))
            {
                if (extremumType.Equals(ExtremumType.MINIMUM))
                {
                    alpha_min = alphaExtremum;
                    for (int d = dim - 2; d >= 0; d--)
                    {
                        alpha_max[d] = DetermineAlphaMax(d, alpha_max, interval);
                    }
                }
                else
                {
                    alpha_max = alphaExtremum;
                    for (int d = dim - 2; d >= 0; d--)
                    {
                        alpha_min[d] = DetermineAlphaMin(d, alpha_min, interval);
                    }
                }
            }
            else
            {
                for (int d = dim - 2; d >= 0; d--)
                {
                    alpha_min[d] = DetermineAlphaMin(d, alpha_min, interval);
                    alpha_max[d] = DetermineAlphaMax(d, alpha_max, interval);
                }
            }

            return new HyperBoundingBox(alpha_min, alpha_max);
        }

        /**
         * Returns the type of the extremum at the specified alpha values.
         * 
         * @param n the index until the alpha values are computed
         * @param alpha_extreme the already computed alpha values
         * @param interval the hyper bounding box defining the interval in which the
         *        extremum occurs
         * @return the type of the extremum at the specified alpha_values
         */
        private ExtremumType GetExtremumType(int n, double[] alpha_extreme, HyperBoundingBox interval)
        {
            Random random = new Random();
            // return the type of the global extremum
            if (n == alpha_extreme.Length - 1)
            {
                return extremumType;
            }

            // create random alpha values
            double[] alpha_extreme_l = new double[alpha_extreme.Length];
            double[] alpha_extreme_r = new double[alpha_extreme.Length];
            double[] alpha_extreme_c = new double[alpha_extreme.Length];

            Array.Copy(alpha_extreme, 0, alpha_extreme_l, 0, alpha_extreme.Length);
            Array.Copy(alpha_extreme, 0, alpha_extreme_r, 0, alpha_extreme.Length);
            Array.Copy(alpha_extreme, 0, alpha_extreme_c, 0, alpha_extreme.Length);

            double[] Centroid = SpatialUtil.Centroid(interval);
            for (int i = 0; i < n; i++)
            {
                alpha_extreme_l[i] = Centroid[i];
                alpha_extreme_r[i] = Centroid[i];
                alpha_extreme_c[i] = Centroid[i];
            }

            double intervalLength = interval.GetMax(n + 1) - interval.GetMin(n + 1);
            alpha_extreme_l[n] = random.NextDouble() * intervalLength + interval.GetMin(n + 1);
            alpha_extreme_r[n] = random.NextDouble() * intervalLength + interval.GetMin(n + 1);

            double f_c = Function(alpha_extreme_c);
            double f_l = Function(alpha_extreme_l);
            double f_r = Function(alpha_extreme_r);

            if (f_l < f_c)
            {
                if (f_r < f_c || Math.Abs(f_r - f_c) < DELTA)
                {
                    return ExtremumType.MAXIMUM;
                }
            }
            if (f_r < f_c)
            {
                if (f_l < f_c || Math.Abs(f_l - f_c) < DELTA)
                {
                    return ExtremumType.MAXIMUM;
                }
            }

            if (f_l > f_c)
            {
                if (f_r > f_c || Math.Abs(f_r - f_c) < DELTA)
                {
                    return ExtremumType.MINIMUM;
                }
            }
            if (f_r > f_c)
            {
                if (f_l > f_c || Math.Abs(f_l - f_c) < DELTA)
                {
                    return ExtremumType.MINIMUM;
                }
            }

            if (Math.Abs(f_l - f_c) < DELTA && Math.Abs(f_r - f_c) < DELTA)
            {
                return ExtremumType.CONSTANT;
            }

            throw new ArgumentException("Houston, we have a problem!\n" + this + "\n" + "f_l " + f_l + "\n" + "f_c " + f_c + "\n" + "f_r " + f_r + "\n" + "p " + GetColumnVector() + "\n" + "alpha   " + FormatUtil.Format(alpha_extreme_c) + "\n" + "alpha_l " + FormatUtil.Format(alpha_extreme_l) + "\n" + "alpha_r " + FormatUtil.Format(alpha_extreme_r) + "\n" + "n " + n);
            // + "box min " + FormatUtil.Format(interval.GetMin()) + "\n"
            // + "box max " + FormatUtil.Format(interval.GetMax()) + "\n"
        }

        /**
         * Determines the n-th alpha value where this function has a minimum in the
         * specified interval.
         * 
         * @param n the index of the alpha value to be determined
         * @param alpha_min the already computed alpha values
         * @param interval the hyper bounding box defining the interval
         * @return the n-th alpha value where this function has a minimum in the
         *         specified interval
         */
        private double DetermineAlphaMin(int n, double[] alpha_min, HyperBoundingBox interval)
        {
            double alpha_n = extremum_alpha_n(n, alpha_min);
            double lower = interval.GetMin(n + 1);
            double upper = interval.GetMax(n + 1);

            double[] alpha_extreme = new double[alpha_min.Length];
            Array.Copy(alpha_min, n, alpha_extreme, n, alpha_extreme.Length - n);
            alpha_extreme[n] = alpha_n;

            ExtremumType type = 
                GetExtremumType(n, alpha_extreme, interval);
            if (type.Equals(ExtremumType.MINIMUM) || type.Equals(ExtremumType.CONSTANT))
            {
                // A) lower <= alpha_n <= upper
                if (lower <= alpha_n && alpha_n <= upper)
                {
                    return alpha_n;
                }
                // B) alpha_n < upper
                else if (alpha_n < lower)
                {
                    return lower;
                }
                // C) alpha_n > max
                else
                {
                    if (alpha_n <= upper)
                    {
                        throw new InvalidOperationException("Should never happen!");
                    }
                    return upper;
                }
            }
            // extremum is maximum
            else
            {
                if (lower <= alpha_n && alpha_n <= upper)
                {
                    // A1) min <= alpha_n <= max && alpha_n - min <= max - alpha_n
                    if (alpha_n - lower <= upper - alpha_n)
                    {
                        return upper;
                    }
                    // A2) min <= alpha_n <= max && alpha_n - min > max - alpha_n
                    else
                    {
                        return lower;
                    }
                }
                // B) alpha_n < min
                else if (alpha_n < lower)
                {
                    return upper;
                }
                // C) alpha_n > max
                else
                {
                    if (alpha_n <= upper)
                    {
                        throw new InvalidOperationException("Should never happen!");
                    }
                    return lower;
                }
            }
        }

        /**
         * Determines the n-th alpha value where this function has a maximum in the
         * specified interval.
         * 
         * @param n the index of the alpha value to be determined
         * @param alpha_max the already computed alpha values
         * @param interval the hyper bounding box defining the interval
         * @return the n-th alpha value where this function has a minimum in the
         *         specified interval
         */
        private double DetermineAlphaMax(int n, double[] alpha_max, HyperBoundingBox interval)
        {
            double alpha_n = extremum_alpha_n(n, alpha_max);
            double lower = interval.GetMin(n + 1);
            double upper = interval.GetMax(n + 1);

            double[] alpha_extreme = new double[alpha_max.Length];
            Array.Copy(alpha_max, n, alpha_extreme, n, alpha_extreme.Length - n);
            alpha_extreme[n] = alpha_n;

            ExtremumType type = GetExtremumType(n, alpha_extreme, interval);
            if (type.Equals(ExtremumType.MINIMUM) || type.Equals(ExtremumType.CONSTANT))
            {
                if (lower <= alpha_n && alpha_n <= upper)
                {
                    // A1) min <= alpha_n <= max && alpha_n - min <= max - alpha_n
                    if (alpha_n - lower <= upper - alpha_n)
                    {
                        return upper;
                    }
                    // A2) min <= alpha_n <= max && alpha_n - min > max - alpha_n
                    else
                    {
                        return lower;
                    }
                }
                // B) alpha_n < min
                else if (alpha_n < lower)
                {
                    return upper;
                }
                // C) alpha_n > max
                else
                {
                    if (alpha_n <= upper)
                    {
                        throw new InvalidOperationException("Should never happen!");
                    }
                    return lower;
                }
            }
            // extremum is maximum
            else
            {
                // A) min <= alpha_n <= max
                if (lower <= alpha_n && alpha_n <= upper)
                {
                    return alpha_n;
                }
                // B) alpha_n < min
                else if (alpha_n < lower)
                {
                    return lower;
                }
                // C) alpha_n > max
                else
                {
                    if (alpha_n <= upper)
                    {
                        throw new InvalidOperationException("Should never happen!");
                    }
                    return upper;
                }
            }
        }

        /**
         * Returns the alpha values of the extremum point in interval [(0,...,0),
         * (Pi,...,Pi)].
         * 
         * @return the alpha values of the extremum
         */
        public double[] GetGlobalAlphaExtremum()
        {
            return alphaExtremum;
        }

        /**
         * Returns the global extremum of this function in interval [0,...,Pi)^d-1.
         * 
         * @return the global extremum
         */
        public double GetGlobalExtremum()
        {
            return Function(alphaExtremum);
        }

        /**
         * Returns the type of the global extremum in interval [0,...,Pi)^d-1.
         * 
         * @return the type of the global extremum
         */
        public ExtremumType GetGlobalExtremumType()
        {
            return extremumType;
        }

        /**
         * Returns a string representation of the object.
         * 
         * @return a string representation of the object.
         */

        public override String ToString()
        {
            return ToString(0);
        }

        /**
         * Returns a string representation of the object with the specified offset.
         * 
         * @param offset the offset of the string representation
         * @return a string representation of the object.
         */
        public String ToString(int offset)
        {
            StringBuilder result = new StringBuilder();
            for (int d = 0; d < Count; d++)
            {
                if (d != 0)
                {
                    result.Append(" + \n").Append(FormatUtil.Whitespace(offset));
                }
                result.Append(FormatUtil.Format(this[(d + 1)]));
                for (int j = 0; j < d; j++)
                {
                    result.Append(" * sin(a_").Append(j + 1).Append(")");
                }
                if (d != Count - 1)
                {
                    result.Append(" * cos(a_").Append(d + 1).Append(")");
                }
            }
            return result.ToString();
        }

        /**
         * Computes the product of all sinus values of the specified angles from start
         * to end index.
         * 
         * @param start the index to start
         * @param end the index to end
         * @param alpha the array of angles
         * @return the product of all sinus values of the specified angles from start
         *         to end index
         */
        private double sinusProduct(int start, int end, double[] alpha)
        {
            double result = 1;
            for (int j = start; j < end; j++)
            {
                result *= Math.Sin(alpha[j]);
            }
            return result;
        }

        /**
         * Determines the global extremum of this parameterization function.
         */
        private void determineGlobalExtremum()
        {
            alphaExtremum = new double[Count - 1];
            for (int n = alphaExtremum.Length - 1; n >= 0; n--)
            {
                alphaExtremum[n] = extremum_alpha_n(n, alphaExtremum);
                if (Double.IsNaN(alphaExtremum[n]))
                {
                    throw new InvalidOperationException("Houston, we have a problem!" + "\n" + this + "\n" + this.GetColumnVector() + "\n" + FormatUtil.Format(alphaExtremum));
                }
            }

            determineGlobalExtremumType();
        }

        /**
         * Determines the type of the global extremum.
         */
        private void determineGlobalExtremumType()
        {
            double f = Function(alphaExtremum);
            Random random = new Random();
            // create random alpha values
            double[] alpha_1 = new double[alphaExtremum.Length];
            double[] alpha_2 = new double[alphaExtremum.Length];
            for (int i = 0; i < alphaExtremum.Length; i++)
            {
                alpha_1[i] = random.NextDouble() * Math.PI;
                alpha_2[i] =random.NextDouble() * Math.PI;
            }

            // look if f1 and f2 are less, greater or equal to f
            double f1 = Function(alpha_1);
            double f2 = Function(alpha_2);

            if (f1 < f && f2 < f)
            {
                extremumType = ExtremumType.MAXIMUM;
            }
            else if (f1 > f && f2 > f)
            {
                extremumType = ExtremumType.MINIMUM;
            }
            else if (Math.Abs(f1 - f) < DELTA && Math.Abs(f2 - f) < DELTA)
            {
                extremumType = ExtremumType.CONSTANT;
            }
            else
            {
                throw new InvalidOperationException("Houston, we have a problem:" + "\n" + this + "\nextremum at " + FormatUtil.Format(alphaExtremum) + "\nf  " + f + "\nf1 " + f1 + "\nf2 " + f2);
            }

        }

        /**
         * Determines the value for alpha_n where this function has a (local)
         * extremum.
         * 
         * @param n the index of the angle
         * @param alpha the already determined alpha_values for the extremum
         * @return the value for alpha_n where this function has a (local) extremum
         */
        private double extremum_alpha_n(int n, double[] alpha)
        {
            // arctan(infinity) = PI/2
            if (this[(n + 1)] == 0)
            {
                return 0.5 * Math.PI;
            }

            double tan = 0;
            for (int j = n + 1; j < Count; j++)
            {
                double alpha_j = j == Count - 1 ? 0 : alpha[j];
                tan += this[(j + 1)] * sinusProduct(n + 1, j, alpha) * Math.Cos(alpha_j);
            }
            tan /= this[(n + 1)];

            // if (debug) {
            // Debug("tan alpha_" + (n + 1) + " = " + tan);
            // }
            double alpha_n = Math.Atan(tan);
            if (alpha_n < 0)
            {
                alpha_n = Math.PI + alpha_n;
            }
            return alpha_n;
        }


        public void WriteToText(TextWriterStream sout, String label)
        {
            String pre = "";
            if (label != null)
            {
                pre = label + "=";
            }
            sout.InlinePrintNoQuotes(pre + base.ToString());
        }


        public ParameterizationFunction newNumberVector(double[] values)
        {
            return new ParameterizationFunction(values);
        }


        public ParameterizationFunction NewFeatureVector(IEnumerable<double> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            double[] values = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                values[i] =(double) adapter.Get(array, i);
            }
            return new ParameterizationFunction(values);
        }


        public ParameterizationFunction NewNumberVector(IEnumerable<double> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            double[] values = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                values[i] =(double) adapter.Get(array, i);
            }
            return new ParameterizationFunction(values);
        }

        /**
         * IParameterization class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public  new class Parameterizer : AbstractParameterizer
        {

            protected override object MakeInstance()
            {
                return STATIC;
            }
        }
    }
}
