using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Maths.Statistics.Distributions
{
    public class Gamma
    {
        static Logging logger = Logging.GetLogger(typeof(Gamma));
        /**
        * Euler鈥揗ascheroni constant
        */
        public static double EULERS_CONST = 0.5772156649015328606065120900824024;

        /**
         * LANCZOS-Coefficients for Gamma approximation.
         * 
         * These are said to have higher precision than those in "Numerical Recipes".
         * They probably come from
         * 
         * Paul Godfrey: http://my.fit.edu/~gabdo/gamma.txt
         */
        static double[] LANCZOS = { 0.99999999999999709182, 57.156235665862923517, -59.597960355475491248, 14.136097974741747174, -0.49191381609762019978, .33994649984811888699e-4, .46523628927048575665e-4, -.98374475304879564677e-4, .15808870322491248884e-3, -.21026444172410488319e-3, .21743961811521264320e-3, -.16431810653676389022e-3, .84418223983852743293e-4, -.26190838401581408670e-4, .36899182659531622704e-5, };

        /**
         * Numerical precision to use (data type dependent!)
         * 
         * If you change this, make sure to test exhaustively!
         */
        static double NUM_PRECISION = 1E-15;

        /**
         * Maximum number of iterations for regularizedGammaP. To prevent degeneration
         * for extreme values.
         * 
         * FIXME: is this too high, too low? Can we improve behavior for extreme
         * cases?
         */
        static int MAX_ITERATIONS = 1000;
        /**
         * Compute logGamma.
         * 
         * Based loosely on "Numerical Recpies" and the work of Paul Godfrey at
         * http://my.fit.edu/~gabdo/gamma.txt
         * 
         * TODO: find out which approximation really is the best...
         * 
         * @param x Parameter x
         * @return log(&#915;(x))
         */
        public static double LogGamma(double x)
        {
            if (Double.IsNaN(x) || (x <= 0.0))
            {
                return Double.NaN;
            }
            double g = 607.0 / 128.0;
            double tmp = x + g + .5;
            tmp = (x + 0.5) * Math.Log(tmp) - tmp;
            double ser = LANCZOS[0];
            for (int i = LANCZOS.Length - 1; i > 0; --i)
            {
                ser += LANCZOS[i] / (x + i);
            }
            return tmp + Math.Log(MathUtil.SQRTTWOPI * ser / x);
        }


        /**
         * Compute probit (inverse cdf) for Gamma distributions.
         * 
         * Based on algorithm AS 91:
         * 
         * Reference:
         * <p>
         * Algorithm AS 91: The percentage points of the $\chi^2$ distribution<br />
         * D.J. Best, D. E. Roberts<br />
         * Journal of the Royal Statistical Society. Series C (Applied Statistics)
         * </p>
         * 
         * @param p Probability
         * @param k k, alpha aka. "shape" parameter
         * @param theta Theta = 1.0/Beta aka. "scaling" parameter
         * @return Probit for Gamma distribution
         */
        [Reference(Title = "Algorithm AS 91: The percentage points of the $\\chi^2$ distribution",
            Authors = "D.J. Best, D. E. Roberts",
            BookTitle = "Journal of the Royal Statistical Society. Series C (Applied Statistics)")]
        public static double Quantile(double p, double k, double theta)
        {
            double EPS2 = 5e-7; // final precision of AS 91
            int MAXIT = 1000;

            // Avoid degenerates
            if (Double.IsNaN(p) || Double.IsNaN(k) || Double.IsNaN(theta))
            {
                return Double.NaN;
            }
            // Range check
            if (p <= 0)
            {
                return 0;
            }
            if (p >= 1)
            {
                return Double.PositiveInfinity;
            }
            // Shape parameter check
            if (k < 0 || theta <= 0)
            {
                return Double.NaN;
            }
            // Corner case - all at 0
            if (k == 0)
            {
                return 0.0;
            }

            int max_newton_iterations = 1;
            // For small values, ensure some refinement iterations
            if (k < 1e-10)
            {
                max_newton_iterations = 7;
            }

            double g = LogGamma(k); // == logGamma(v/2)

            // Phase I, an initial rough approximation
            // First half of AS 91
            double ch = ChisquaredProbitApproximation(p, 2 * k, g);
        // Second hald of AS 91 follows:
        // Refine ChiSquared approximation
      //  chisq:
            {
                if (Double.IsInfinity(ch))
                {
                    // Cannot refine infinity
                    max_newton_iterations = 0;
                    goto outchisq;
                }
                if (ch < EPS2)
                {
                    // Do not iterate, but refine with newton method
                    max_newton_iterations = 20;
                    goto outchisq;
                }
                if (p > 1 - 1e-14 || p < 1e-100)
                {
                    // Not in appropriate value range for AS 91
                    max_newton_iterations = 20;
                    goto outchisq;
                }

                // Phase II: Iteration
                double c = k - 1;
                double ch0 = ch; // backup initial approximation
                for (int i = 1; i <= MAXIT; i++)
                {
                    double q = ch; // previous approximation
                    double p1 = 0.5 * ch;
                    double p2 = p - RegularizedGammaP(k, p1);
                    if (Double.IsInfinity(p2) || ch <= 0)
                    {
                        ch = ch0;
                        max_newton_iterations = 27;
                        goto outchisq;
                    }
                    { // Taylor series of AS 91: iteration via "goto 4"
                        double t = p2 * Math.Exp(k * MathUtil.LOG2 + g + p1 - c * Math.Log(ch));
                        double b = t / ch;
                        double a = 0.5 * t - b * c;
                        double s1 = (210.0 + a * (140.0 + a * (105.0 + a * (84.0 + a * (70.0 + 60.0 * a))))) / 420.0;
                        double s2 = (420.0 + a * (735.0 + a * (966.0 + a * (1141.0 + 1278 * a)))) / 2520.0;
                        double s3 = (210.0 + a * (462.0 + a * (707.0 + 932.0 * a))) / 2520.0;
                        double s4 = (252.0 + a * (672.0 + 1182.0 * a) + c * (294.0 + a * (889.0 + 1740.0 * a))) / 5040.0;
                        double s5 = (84.0 + 2264.0 * a + c * (1175.0 + 606.0 * a)) / 2520.0;
                        double s6 = (120.0 + c * (346.0 + 127.0 * c)) / 5040.0;
                        ch += t * (1 + 0.5 * t * s1 - b * c * (s1 - b * (s2 - b * (s3 - b * (s4 - b * (s5 - b * s6))))));
                    }
                    if (Math.Abs(q - ch) < EPS2 * ch)
                    {
                        goto outchisq;
                    }
                    // Divergence treatment, from GNU R
                    if (Math.Abs(q - ch) > 0.1 * Math.Abs(ch))
                    {
                        ch = ((ch < q) ? 0.9 : 1.1) * q;
                    }
                }
                logger.Warning("No convergence in AS 91 Gamma probit.");
                // no convergence in MAXIT iterations -- but we add Newton now...
            }
        outchisq:
            double x = 0.5 * ch / theta;
            if (max_newton_iterations > 0)
            {
                // Refine result using final Newton steps.
                // TODO: add unit tests that show an improvement! Maybe in logscale only?
                x = GammaQuantileNewtonRefinement(Math.Log(p), k, theta, max_newton_iterations, x);
            }
            return x;
        }

        /**
         * Refinement of ChiSquared probit using Newton iterations.
         * 
         * A trick used by GNU R to improve precision.
         * 
         * @param logpt Target value of log p
         * @param k Alpha
         * @param theta Theta = 1 / Beta
         * @param maxit Maximum number of iterations to do
         * @param x Initial estimate
         * @return Refined value
         */
        protected static double GammaQuantileNewtonRefinement(double logpt, double k, double theta, int maxit, double x)
        {
            double EPS_N = 1e-15; // Precision threshold
            // 0 is not possible, try MIN_NORMAL instead
            if (x <= 0)
            {
                x = Double.Epsilon;
            }
            // Current estimation
            double logpc = Logcdf(x, k, theta);
            if (x == Double.Epsilon && logpc > logpt * (1.0 + 1e-7))
            {
                return 0.0;
            }
            if (logpc == Double.NegativeInfinity)
            {
                return 0.0;
            }
            // Refine by newton iterations
            for (int i = 0; i < maxit; i++)
            {
                // Error of current approximation
                double logpe = logpc - logpt;
                if (Math.Abs(logpe) < Math.Abs(EPS_N * logpt))
                {
                    break;
                }
                // Step size is controlled by PDF:
                double g = Logpdf(x, k, theta);
                if (g == Double.NegativeInfinity)
                {
                    break;
                }
                double newx = x - logpe * Math.Exp(logpc - g);
                // New estimate:
                logpc = Logcdf(newx, k, theta);
                if (Math.Abs(logpc - logpt) > Math.Abs(logpe) || (i > 0 && Math.Abs(logpc - logpt) == Math.Abs(logpe)))
                {
                    // no further improvement
                    break;
                }
                x = newx;
            }
            return x;
        }
        /**
          * Approximate probit for chi squared distribution
          * 
          * Based on first half of algorithm AS 91
          * 
          * Reference:
          * <p>
          * Algorithm AS 91: The percentage points of the $\chi^2$ distribution<br />
          * D.J. Best, D. E. Roberts<br />
          * Journal of the Royal Statistical Society. Series C (Applied Statistics)
          * </p>
          * 
          * @param p Probit value
          * @param nu Shape parameter for Chi, nu = 2 * k
          * @param g log(nu)
          * @return Probit for chi squared
          */
        [Reference(Title = "Algorithm AS 91: The percentage points of the $\\chi^2$ distribution", Authors = "D.J. Best, D. E. Roberts",
            BookTitle = "Journal of the Royal Statistical Society. Series C (Applied Statistics)")]
        protected static double ChisquaredProbitApproximation(double p, double nu, double g)
        {
            double EPS1 = 1e-2; // Approximation quality
            // Sanity checks
            if (Double.IsNaN(p) || Double.IsNaN(nu))
            {
                return Double.NaN;
            }
            // Range check
            if (p <= 0)
            {
                return 0;
            }
            if (p >= 1)
            {
                return Double.PositiveInfinity;
            }
            // Invalid parameters
            if (nu <= 0)
            {
                return Double.NaN;
            }
            // Shape of gamma distribution, "XX" in AS 91
            double k = 0.5 * nu;

            // For small chi squared values - AS 91
            double logp = Math.Log(p);
            if (nu < -1.24 * logp)
            {
                // FIXME: implement and use logGammap1 instead - more stable?
                //
                // final double lgam1pa = (alpha < 0.5) ? logGammap1(alpha) :
                // (Math.log(alpha) + g);
                // return Math.exp((lgam1pa + logp) / alpha + MathUtil.LOG2);
                // This is literal AS 91, above is the GNU R variant.
                return Math.Pow(p * k * Math.Exp(g + k * MathUtil.LOG2), 1.0 / k);
            }
            else if (nu > 0.32)
            {
                // Wilson and Hilferty estimate: - AS 91 at 3
                double x = Normal.Quantile(p, 0, 1);
                double p1 = 2.0 / (9.0 * nu);
                double a = x * Math.Sqrt(p1) + 1 - p1;
                double ch = nu * a * a * a;

                // Better approximation for p tending to 1:
                if (ch > 2.2 * nu + 6)
                {
                    ch = -2 * (Math.Log(1 - p) - (k - 1) * Math.Log(0.5 * ch) + g);
                }
                return ch;
            }
            else
            {
                // nu <= 0.32, AS 91 at 1
                double C7 = 4.67, C8 = 6.66, C9 = 6.73, C10 = 13.32;
                double ag = Math.Log(1 - p) + g + (k - 1) * MathUtil.LOG2;
                double ch = 0.4;
                while (true)
                {
                    double p1 = 1 + ch * (C7 + ch);
                    double p2 = ch * (C9 + ch * (C8 + ch));
                    double t = -0.5 + (C7 + 2 * ch) / p1 - (C9 + ch * (C10 + 3 * ch)) / p2;
                    double delta = (1 - Math.Exp(ag + 0.5 * ch) * p2 / p1) / t;
                    ch -= delta;
                    if (Math.Abs(delta) > EPS1 * Math.Abs(ch))
                    {
                        return ch;
                    }
                }
            }
        }

        /**
         * Returns the regularized gamma function P(a, x).
         * 
         * Includes the quadrature way of computing.
         * 
         * TODO: find "the" most accurate version of this. We seem to agree with
         * others for the first 10+ digits, but diverge a bit later than that.
         * 
         * @param a Parameter a
         * @param x Parameter x
         * @return Gamma value
         */
        public static double RegularizedGammaP(double a, double x)
        {
            // Special cases
            if (Double.IsInfinity(a) || Double.IsInfinity(x) || !(a > 0.0) || !(x >= 0.0))
            {
                return Double.NaN;
            }
            if (x == 0.0)
            {
                return 0.0;
            }
            if (x >= a + 1)
            {
                // Expected to converge faster
                return 1.0 - RegularizedGammaQ(a, x);
            }
            // Loosely following "Numerical Recipes"
            double term = 1.0 / a;
            double sum = term;
            for (int n = 1; n < MAX_ITERATIONS; n++)
            {
                // compute next element in the series
                term = x / (a + n) * term;
                sum = sum + term;
                if (sum == Double.PositiveInfinity)
                {
                    return 1.0;
                }
                if (Math.Abs(term / sum) < NUM_PRECISION)
                {
                    break;
                }
            }
            return Math.Exp(-x + (a * Math.Log(x)) - LogGamma(a)) * sum;
        }

        /**
         * Returns the regularized gamma function Q(a, x) = 1 - P(a, x).
         * 
         * Includes the continued fraction way of computing, based loosely on the book
         * "Numerical Recipes"; but probably not with the exactly same precision,
         * since we reimplemented this in our coding style, not literally.
         * 
         * TODO: find "the" most accurate version of this. We seem to agree with
         * others for the first 10+ digits, but diverge a bit later than that.
         * 
         * @param a parameter a
         * @param x parameter x
         * @return Result
         */
        public static double RegularizedGammaQ(double a, double x)
        {
            if (Double.IsNaN(a) || Double.IsNaN(x) || (a <= 0.0) || (x < 0.0))
            {
                return Double.NaN;
            }
            if (x == 0.0)
            {
                return 1.0;
            }
            if (x < a + 1.0)
            {
                // Expected to converge faster
                return 1.0 - RegularizedGammaP(a, x);
            }
            // Compute using continued fraction approach.
            double FPMIN = Double.MinValue / NUM_PRECISION;
            double b = x + 1 - a;
            double c = 1.0 / FPMIN;
            double d = 1.0 / b;
            double fac = d;
            for (int i = 1; i < MAX_ITERATIONS; i++)
            {
                double an = i * (a - i);
                b += 2;
                d = an * d + b;
                if (Math.Abs(d) < FPMIN)
                {
                    d = FPMIN;
                }
                c = b + an / c;
                if (Math.Abs(c) < FPMIN)
                {
                    c = FPMIN;
                }
                d = 1 / d;
                double del = d * c;
                fac *= del;
                if (Math.Abs(del - 1.0) <= NUM_PRECISION)
                {
                    break;
                }
            }
            return fac * Math.Exp(-x + a * Math.Log(x) - LogGamma(a));
        }
        /**
 * The log CDF, static version.
 * 
 * @param val Value
 * @param k Shape k
 * @param theta Theta = 1.0/Beta aka. "scaling" parameter
 * @return cdf value
 */
        public static double Logcdf(double val, double k, double theta)
        {
            MathNet.Numerics.Distributions.Gamma gamma = new MathNet.Numerics.Distributions.Gamma(k, theta);
            return Math.Log(gamma.CumulativeDistribution(val));
            //if (val < 0)
            //{
            //    return Double.NegativeInfinity;
            //}
            //return logregularizedGammaP(k, val * theta);
        }
        /**
         * Gamma distribution PDF (with 0.0 for x &lt; 0)
         * 
         * @param x query value
         * @param k Alpha
         * @param theta Theta = 1 / Beta
         * @return probability density
         */
        public static double Logpdf(double x, double k, double theta)
        {
            if (x < 0)
            {
                return Double.NegativeInfinity;
            }
            if (x == 0)
            {
                if (k == 1.0)
                {
                    return Math.Log(theta);
                }
                else
                {
                    return Double.NegativeInfinity;
                }
            }
            if (k == 1.0)
            {
                return Math.Log(theta) - x * theta;
            }

            return Math.Log(theta) + (k - 1.0) * Math.Log(x * theta) - x * theta - LogGamma(k);
        }
    }
}
