using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.Statistics.Distributions
{
    public class Normal
    {
        /**
   * Coefficients for erf approximation.
   * 
   * Loosely based on http://www.netlib.org/specfun/erf
   */
        static double[] ERFAPP_A = { 1.85777706184603153e-1, 3.16112374387056560e+0, 1.13864154151050156E+2, 3.77485237685302021e+2, 3.20937758913846947e+3 };

        /**
         * Coefficients for erf approximation.
         * 
         * Loosely based on http://www.netlib.org/specfun/erf
         */
        static double[] ERFAPP_B = { 1.00000000000000000e00, 2.36012909523441209e01, 2.44024637934444173e02, 1.28261652607737228e03, 2.84423683343917062e03 };

        /**
         * Coefficients for erf approximation.
         * 
         * Loosely based on http://www.netlib.org/specfun/erf
         */
        static double[] ERFAPP_C = { 2.15311535474403846e-8, 5.64188496988670089e-1, 8.88314979438837594e00, 6.61191906371416295e01, 2.98635138197400131e02, 8.81952221241769090e02, 1.71204761263407058e03, 2.05107837782607147e03, 1.23033935479799725E03 };

        /**
         * Coefficients for erf approximation.
         * 
         * Loosely based on http://www.netlib.org/specfun/erf
         */
        static double[] ERFAPP_D = { 1.00000000000000000e00, 1.57449261107098347e01, 1.17693950891312499e02, 5.37181101862009858e02, 1.62138957456669019e03, 3.29079923573345963e03, 4.36261909014324716e03, 3.43936767414372164e03, 1.23033935480374942e03 };

        /**
         * Coefficients for erf approximation.
         * 
         * Loosely based on http://www.netlib.org/specfun/erf
         */
        static double[] ERFAPP_P = { 1.63153871373020978e-2, 3.05326634961232344e-1, 3.60344899949804439e-1, 1.25781726111229246e-1, 1.60837851487422766e-2, 6.58749161529837803e-4 };

        /**
         * Coefficients for erf approximation.
         * 
         * Loosely based on http://www.netlib.org/specfun/erf
         */
        static double[] ERFAPP_Q = { 1.00000000000000000e00, 2.56852019228982242e00, 1.87295284992346047e00, 5.27905102951428412e-1, 6.05183413124413191e-2, 2.33520497626869185e-3 };

        /**
         * Treshold for switching nethods for erfinv approximation
         */
        static double P_LOW = 0.02425D;

        /**
         * Treshold for switching nethods for erfinv approximation
         */
        static double P_HIGH = 1.0D - P_LOW;

        /**
         * Coefficients for erfinv approximation, rational version
         */
        static double[] ERFINV_A = { -3.969683028665376e+01, 2.209460984245205e+02, -2.759285104469687e+02, 1.383577518672690e+02, -3.066479806614716e+01, 2.506628277459239e+00 };

        /**
         * Coefficients for erfinv approximation, rational version
         */
        static double[] ERFINV_B = { -5.447609879822406e+01, 1.615858368580409e+02, -1.556989798598866e+02, 6.680131188771972e+01, -1.328068155288572e+01 };

        /**
         * Coefficients for erfinv approximation, rational version
         */
        static double[] ERFINV_C = { -7.784894002430293e-03, -3.223964580411365e-01, -2.400758277161838e+00, -2.549732539343734e+00, 4.374664141464968e+00, 2.938163982698783e+00 };

        /**
         * Coefficients for erfinv approximation, rational version
         */
        static double[] ERFINV_D = { 7.784695709041462e-03, 3.224671290700398e-01, 2.445134137142996e+00, 3.754408661907416e+00 };

        /**
         * 1 / CDFINV(0.75)
         */
        public static double ONEBYPHIINV075 = 1.48260221850560186054;
        /**
  * Inverse cumulative probability density function (probit) of a normal
  * distribution.
  * 
  * @param x value to evaluate probit function at
  * @param mu Mean value
  * @param sigma Standard deviation.
  * @return The probit of the given normal distribution at x.
  */
        public static double Quantile(double x, double mu, double sigma)
        {
            return mu + sigma * StandardNormalQuantile(x);
        }

        /**
         * Approximate the inverse error function for normal distributions.
         * 
         * Largely based on:
         * <p>
         * http://www.math.uio.no/~jacklam/notes/invnorm/index.html <br>
         * by Peter John Acklam
         * </p>
         * 
         * FIXME: precision of this seems to be rather low, compared to our other
         * functions. Only about 8-9 digits agree with SciPy/GNU R.
         * 
         * @param d Quantile. Must be in [0:1], obviously.
         * @return Inverse erf.
         */
        public static double StandardNormalQuantile(double d)
        {
            if (d == 0)
            {
                return Double.NegativeInfinity;
            }
            else if (d == 1)
            {
                return Double.PositiveInfinity;
            }
            else if (Double.IsNaN(d) || d < 0 || d > 1)
            {
                return Double.NaN;
            }
            else if (d < P_LOW)
            {
                // Rational approximation for lower region:
                double q = Math.Sqrt(-2 * Math.Log(d));
                return (((((ERFINV_C[0] * q + ERFINV_C[1]) * q + ERFINV_C[2]) * q + ERFINV_C[3]) * q + ERFINV_C[4]) * q + ERFINV_C[5])
                    / ((((ERFINV_D[0] * q + ERFINV_D[1]) * q + ERFINV_D[2]) * q + ERFINV_D[3]) * q + 1);
            }
            else if (P_HIGH < d)
            {
                // Rational approximation for upper region:
                double q = Math.Sqrt(-2 * Math.Log(1 - d));
                return -(((((ERFINV_C[0] * q + ERFINV_C[1]) * q + ERFINV_C[2]) * q + ERFINV_C[3]) * q + ERFINV_C[4]) * q + ERFINV_C[5]) /
                    ((((ERFINV_D[0] * q + ERFINV_D[1]) * q + ERFINV_D[2]) * q + ERFINV_D[3]) * q + 1);
            }
            else
            {
                // Rational approximation for central region:
                double q = d - 0.5D;
                double r = q * q;
                return (((((ERFINV_A[0] * r + ERFINV_A[1]) * r + ERFINV_A[2]) * r + ERFINV_A[3]) * r + ERFINV_A[4]) * r + ERFINV_A[5]) * q /
                    (((((ERFINV_B[0] * r + ERFINV_B[1]) * r + ERFINV_B[2]) * r + ERFINV_B[3]) * r + ERFINV_B[4]) * r + 1);
            }
        }
    }
}
