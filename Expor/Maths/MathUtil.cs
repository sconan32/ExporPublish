using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Maths.LinearAlgebra;

namespace Socona.Expor.Maths
{
    public sealed class MathUtil
    {
        /**
         * Two times Pi.
         */
        public static double TWOPI = 2 * Math.PI;

        /**
         * Square root of two times Pi.
         */
        public static double SQRTTWOPI = Math.Sqrt(TWOPI);

        /**
         * Square root of 2.
         */
        public static double SQRT2 = Math.Sqrt(2);

        /**
         * Square root of 5
         */
        public static double SQRT5 = Math.Sqrt(5);

        /**
         * Square root of 0.5 == 1 / Sqrt(2)
         */
        public static double SQRTHALF = Math.Sqrt(.5);

        /**
         * Precomputed value of 1 / Sqrt(pi)
         */
        public static double ONE_BY_SQRTPI = 1 / Math.Sqrt(Math.PI);

        /**
         * Logarithm of 2 to the basis e, for logarithm conversion.
         */
        public static double LOG2 = Math.Log(2);

        /**
         * Natural logarithm of 10
         */
        public static double LOG10 = Math.Log(10);

        /**
         * Math.log(Math.PI)
         */
        public static double LOGPI = Math.Log(Math.PI);

        /**
         * Math.log(Math.PI) / 2
         */
        public static double LOGPIHALF = LOGPI / 2.0;

        /**
         * Math.log(Math.Sqrt(2*Math.PI))
         */
        public static double LOGSQRTTWOPI = Math.Log(SQRTTWOPI);

        /**
         * Fake constructor for static class.
         */
        private MathUtil()
        {
            // Static methods only - do not instantiate!
        }

        /**
         * Computes the square root of the sum of the squared arguments without under
         * or overflow.
         * 
         * Note: this code is <em>not</em> redundant to {@link Math#hypot}, since the
         * latter is significantly slower (but maybe has a higher precision).
         * 
         * @param a first cathetus
         * @param b second cathetus
         * @return {@code Sqrt(a<sup>2</sup> + b<sup>2</sup>)}
         */
        public static double FastHypot(double a, double b)
        {
            if (a < 0)
            {
                a = -a;
            }
            if (b < 0)
            {
                b = -b;
            }
            if (a > b)
            {
                double r = b / a;
                return a * Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                double r = a / b;
                return b * Math.Sqrt(1 + r * r);
            }
            else
            {
                return 0.0;
            }
        }

        /**
         * Computes the square root of the sum of the squared arguments without under
         * or overflow.
         * 
         * Note: this code is <em>not</em> redundant to {@link Math#hypot}, since the
         * latter is significantly slower (but has a higher precision).
         * 
         * @param a first cathetus
         * @param b second cathetus
         * @param c second cathetus
         * @return {@code Sqrt(a<sup>2</sup> + b<sup>2</sup> + c<sup>2</sup>)}
         */
        public static double FastHypot3(double a, double b, double c)
        {
            if (a < 0)
            {
                a = -a;
            }
            if (b < 0)
            {
                b = -b;
            }
            if (c < 0)
            {
                c = -c;
            }
            double m = (a > b) ? ((a > c) ? a : c) : ((b > c) ? b : c);
            if (m <= 0)
            {
                return 0.0;
            }
            a = a / m;
            b = b / m;
            c = c / m;
            return m * Math.Sqrt(a * a + b * b + c * c);
        }

        /**
         * Compute the Mahalanobis distance using the given weight matrix
         * 
         * @param weightMatrix Weight Matrix
         * @param o1_minus_o2 Delta vector
         * @return Mahalanobis distance
         */
        public static double MahalanobisDistance(Matrix weightMatrix, Vector o1_minus_o2)
        {
            double sqrDist = o1_minus_o2.TransposeTimesTimes(weightMatrix, o1_minus_o2);

            if (sqrDist < 0 && Math.Abs(sqrDist) < 0.000000001)
            {
                sqrDist = Math.Abs(sqrDist);
            }
            return Math.Sqrt(sqrDist);
        }

        /**
         * <p>
         * Provides the Pearson product-moment correlation coefficient for two
         * FeatureVectors.
         * </p>
         * 
         * @param x first FeatureVector
         * @param y second FeatureVector
         * @return the Pearson product-moment correlation coefficient for x and y
         */
        public static double PearsonCorrelationCoefficient(INumberVector x, INumberVector y)
        {
            int xdim = x.Count;
            int ydim = y.Count;
            if (xdim != ydim)
            {
                throw new ArgumentException("Invalid arguments: feature vectors differ in dimensionality.");
            }
            if (xdim <= 0)
            {
                throw new ArgumentException("Invalid arguments: dimensionality not positive.");
            }
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < xdim; i++)
            {
                pc.put(x[i + 1], y[i + 1], 1.0);
            }
            return pc.GetCorrelation();
        }

        /**
         * <p>
         * Provides the Pearson product-moment correlation coefficient for two
         * FeatureVectors.
         * </p>
         * 
         * @param x first FeatureVector
         * @param y second FeatureVector
         * @return the Pearson product-moment correlation coefficient for x and y
         */
        public static double WeightedPearsonCorrelationCoefficient(INumberVector x, INumberVector y, double[] weights)
        {
            int xdim = x.Count;
            int ydim = y.Count;
            if (xdim != ydim)
            {
                throw new ArgumentException("Invalid arguments: feature vectors differ in dimensionality.");
            }
            if (xdim != weights.Length)
            {
                throw new ArgumentException("Dimensionality doesn't agree to weights.");
            }
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < xdim; i++)
            {
                pc.put(x[i + 1], y[i + 1], weights[i]);
            }
            return pc.GetCorrelation();
        }

        /**
         * <p>
         * Provides the Pearson product-moment correlation coefficient for two
         * FeatureVectors.
         * </p>
         * 
         * @param x first FeatureVector
         * @param y second FeatureVector
         * @return the Pearson product-moment correlation coefficient for x and y
         */
        public static double WeightedPearsonCorrelationCoefficient(INumberVector x, INumberVector y, INumberVector weights)
        {
            int xdim = x.Count;
            int ydim = y.Count;
            if (xdim != ydim)
            {
                throw new ArgumentException("Invalid arguments: feature vectors differ in dimensionality.");
            }
            if (xdim != weights.Count)
            {
                throw new ArgumentException("Dimensionality doesn't agree to weights.");
            }
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < xdim; i++)
            {
                pc.put(x[i + 1], y[i + 1], weights[i + 1]);
            }
            return pc.GetCorrelation();
        }

        /**
         * <p>
         * Provides the Pearson product-moment correlation coefficient for two
         * FeatureVectors.
         * </p>
         * 
         * @param x first FeatureVector
         * @param y second FeatureVector
         * @return the Pearson product-moment correlation coefficient for x and y
         */
        public static double PearsonCorrelationCoefficient(double[] x, double[] y)
        {
            int xdim = x.Length;
            int ydim = y.Length;
            if (xdim != ydim)
            {
                throw new ArgumentException("Invalid arguments: feature vectors differ in dimensionality.");
            }
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < xdim; i++)
            {
                pc.put(x[i], y[i], 1.0);
            }
            return pc.GetCorrelation();
        }

        /**
         * <p>
         * Provides the Pearson product-moment correlation coefficient for two
         * FeatureVectors.
         * </p>
         * 
         * @param x first FeatureVector
         * @param y second FeatureVector
         * @return the Pearson product-moment correlation coefficient for x and y
         */
        public static double weightedPearsonCorrelationCoefficient(double[] x, double[] y, double[] weights)
        {
            int xdim = x.Length;
            int ydim = y.Length;
            if (xdim != ydim)
            {
                throw new ArgumentException("Invalid arguments: feature vectors differ in dimensionality.");
            }
            if (xdim != weights.Length)
            {
                throw new ArgumentException("Dimensionality doesn't agree to weights.");
            }
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < xdim; i++)
            {
                pc.put(x[i], y[i], weights[i]);
            }
            return pc.GetCorrelation();
        }

        /**
         * Compute the Factorial of n, often written as <code>c!</code> in
         * mathematics.</p>
         * <p>
         * Use this method if for large values of <code>n</code>.
         * </p>
         * 
         * @param n Note: n &gt;= 0. This {@link BigInteger} <code>n</code> will be 0
         *        after this method finishes.
         * @return n * (n-1) * (n-2) * ... * 1
         */
        public static BigInteger Factorial(BigInteger n)
        {
            BigInteger nFac = BigInteger.One;
            while (n > BigInteger.One)
            {
                nFac = nFac * n;
                n = n - BigInteger.One;
            }
            return nFac;
        }

        /**
         * Compute the Factorial of n, often written as <code>c!</code> in
         * mathematics.
         * 
         * @param n Note: n &gt;= 0
         * @return n * (n-1) * (n-2) * ... * 1
         */
        public static long Factorial(int n)
        {
            long nFac = 1;
            for (long i = n; i > 0; i--)
            {
                nFac *= i;
            }
            return nFac;
        }

        /**
         * <p>
         * Binomial coefficient, also known as "n choose k".
         * </p>
         * 
         * @param n Total number of samples. n &gt; 0
         * @param k Number of elements to choose. <code>n &gt;= k</code>,
         *        <code>k &gt;= 0</code>
         * @return n! / (k! * (n-k)!)
         */
        public static long BinomialCoefficient(long n, long k)
        {
            long m = Math.Max(k, n - k);
            double temp = 1;
            for (long i = n, j = 1; i > m; i--, j++)
            {
                temp = temp * i / j;
            }
            return (long)temp;
        }

        /**
         * Compute the Factorial of n, often written as <code>c!</code> in
         * mathematics.
         * 
         * @param n Note: n &gt;= 0
         * @return n * (n-1) * (n-2) * ... * 1
         */
        public static double ApproximateFactorial(int n)
        {
            double nFac = 1.0;
            for (int i = n; i > 0; i--)
            {
                nFac *= i;
            }
            return nFac;
        }

        /**
         * <p>
         * Binomial coefficent, also known as "n choose k")
         * </p>
         * 
         * @param n Total number of samples. n &gt; 0
         * @param k Number of elements to choose. <code>n &gt;= k</code>,
         *        <code>k &gt;= 0</code>
         * @return n! / (k! * (n-k)!)
         */
        public static double ApproximateBinomialCoefficient(int n, int k)
        {
            int m = Math.Max(k, n - k);
            long temp = 1;
            for (int i = n, j = 1; i > m; i--, j++)
            {
                temp = temp * i / j;
            }
            return temp;
        }

        /**
         * Compute the sum of the i first integers.
         * 
         * @param i maximum summand
         * @return Sum
         */
        public static long SumFirstIntegers(long i)
        {
            return ((i - 1L) * i) / 2;
        }

        /**
         * Produce an array of random numbers in [0:1]
         * 
         * @param len Length
         * @return Array
         */
        public static double[] RandomDoubleArray(int len)
        {
            return RandomDoubleArray(len, new Random());
        }

        /**
         * Produce an array of random numbers in [0:1]
         * 
         * @param len Length
         * @param r Random generator
         * @return Array
         */
        public static double[] RandomDoubleArray(int len, Random r)
        {
            double[] ret = new double[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = r.NextDouble();
            }
            return ret;
        }

        /**
         * Convert Degree to Radians
         * 
         * @param deg Degree value
         * @return Radian value
         */
        public static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /**
         * Radians to Degree
         * 
         * @param rad Radians value
         * @return Degree value
         */
        public static double Rad2Deg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /**
         * Compute the approximate on-earth-surface distance of two points.
         * 
         * @param lat1 Latitude of first point in degree
         * @param lon1 Longitude of first point in degree
         * @param lat2 Latitude of second point in degree
         * @param lon2 Longitude of second point in degree
         * @return Distance in km (approximately)
         */
        public static double LatLngDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double EARTH_RADIUS = 6371; // km.
            // Work in radians
            lat1 = MathUtil.Deg2Rad(lat1);
            lat2 = MathUtil.Deg2Rad(lat2);
            lon1 = MathUtil.Deg2Rad(lon1);
            lon2 = MathUtil.Deg2Rad(lon2);
            // Delta
            double dlat = lat1 - lat2;
            double dlon = lon1 - lon2;

            // Spherical Law of Cosines
            // NOTE: there seems to be a signedness issue in this code!
            // double dist = Math.Sin(lat1) * Math.Sin(lat2) + Math.cos(lat1) *
            // Math.cos(lat2) * Math.cos(dlon);
            // return EARTH_RADIUS * Math.atan(dist);

            // Alternative: Havestine formula, higher precision at < 1 meters:
            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) + Math.Sin(dlon / 2) * Math.Sin(dlon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EARTH_RADIUS * c;
        }

        /**
         * Compute the angle between two vectors.
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @return Angle
         */
        public static double Angle(Vector v1, Vector v2)
        {
            return Angle(v1.GetArrayRef(), v2.GetArrayRef());
        }

        /**
         * Compute the angle between two vectors.
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @return Angle
         */
        public static double Angle(double[] v1, double[] v2)
        {
            // Essentially, we want to compute this:
            // v1.transposeTimes(v2) / (v1.euclideanLength() * v2.euclideanLength());
            // We can just compute all three in parallel.
            double s = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < v1.Length; k++)
            {
                double r1 = v1[k];
                double r2 = v2[k];
                s += r1 * r2;
                e1 += r1 * r1;
                e2 += r2 * r2;
            }
            return Math.Sqrt((s / e1) * (s / e2));
        }

        /**
         * Compute the angle between two vectors.
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @param o Origin
         * @return Angle
         */
        public static double Angle(Vector v1, Vector v2, Vector o)
        {
            return Angle(v1.GetArrayRef(), v2.GetArrayRef(), o.GetArrayRef());
        }

        /**
         * Compute the angle between two vectors.
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @param o Origin
         * @return Angle
         */
        public static double Angle(double[] v1, double[] v2, double[] o)
        {
            // Essentially, we want to compute this:
            // v1' = v1 - o, v2' = v2 - o
            // v1'.transposeTimes(v2') / (v1'.euclideanLength()*v2'.euclideanLength());
            // We can just compute all three in parallel.
            double s = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < v1.Length; k++)
            {
                double r1 = v1[k] - o[k];
                double r2 = v2[k] - o[k];
                s += r1 * r2;
                e1 += r1 * r1;
                e2 += r2 * r2;
            }
            return Math.Sqrt((s / e1) * (s / e2));
        }

        /**
         * Find the next power of 2.
         * 
         * Classic bit operation, for signed 32-bit. Valid for positive integers only
         * (0 otherwise).
         * 
         * @param x original integer
         * @return Next power of 2
         */
        public static int NextPow2Int(int x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return ++x;
        }

        /**
         * Find the next power of 2.
         * 
         * Classic bit operation, for signed 64-bit. Valid for positive integers only
         * (0 otherwise).
         * 
         * @param x original long integer
         * @return Next power of 2
         */
        public static long NextPow2Long(long x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 16;
            x |= x >> 32;
            return ++x;
        }

        /**
         * Find the next larger number with all ones.
         * 
         * Classic bit operation, for signed 32-bit. Valid for positive integers only
         * (-1 otherwise).
         * 
         * @param x original integer
         * @return Next number with all bits set
         */
        public static int NextAllOnesInt(int x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x;
        }

        /**
         * Find the next larger number with all ones.
         * 
         * Classic bit operation, for signed 64-bit. Valid for positive integers only
         * (-1 otherwise).
         * 
         * @param x original long integer
         * @return Next number with all bits set
         */
        public static long NextAllOnesLong(long x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 16;
            x |= x >> 32;
            return x;
        }

        ///**
        // * Return the largest double that rounds down to this float.
        // * 
        // * Note: Probably not always correct - subnormal values are quite tricky. So
        // * some of the bounds might not be tight.
        // * 
        // * @param f Float value
        // * @return Double value
        // */
        //public static double FloatToDoubleUpper(float f) {
        //  if(float.IsNaN(f)) {
        //    return Double.NaN;
        //  }
        //  if(float.IsInfinity(f)) {
        //    if(f > 0) {
        //      return Double.PositiveInfinity;
        //    }
        //    else {
        //      return Double.NegativeInfinity;
        //    }
        //  }
        //  long bits = Double.doubleToRawLongBits((double) f);
        //  if((bits & 0x8000000000000000l) == 0) { // Positive
        //    if(bits == 0l) {
        //      return Double.longBitsToDouble(0x3690000000000000l);
        //    }
        //    if(f == Float.MIN_VALUE) {
        //      // bits += 0x7_ffff_ffff_ffffl;
        //      return Double.longBitsToDouble(0x36a7ffffffffffffl);
        //    }
        //    if(Float.MIN_NORMAL > f && f >= Double.MIN_NORMAL) {
        //      // The most tricky case:
        //      // a denormalized float, but a normalized double
        //       long bits2 = Double.doubleToRawLongBits((double) Math.nextUp(f));
        //      bits = (bits >>> 1) + (bits2 >>> 1) - 1l;
        //    }
        //    else {
        //      bits += 0xfffffffl; // 28 extra bits
        //    }
        //    return Double.longBitsToDouble(bits);
        //  }
        //  else {
        //    if(bits == 0x8000000000000000l) {
        //      return -0.0d;
        //    }
        //    if(f == -Float.MIN_VALUE) {
        //      // bits -= 0xf_ffff_ffff_ffffl;
        //      return Double.longBitsToDouble(0xb690000000000001l);
        //    }
        //    if(-Float.MIN_NORMAL < f && f <= -Double.MIN_NORMAL) {
        //      // The most tricky case:
        //      // a denormalized float, but a normalized double
        //       long bits2 = Double.doubleToRawLongBits((double) Math.nextUp(f));
        //      bits = (bits >>> 1) + (bits2 >>> 1) + 1l;
        //    }
        //    else {
        //      bits -= 0xfffffffl; // 28 extra bits
        //    }
        //    return Double.longBitsToDouble(bits);
        //  }
        //}

        ///**
        // * Return the largest double that rounds up to this float.
        // * 
        // * Note: Probably not always correct - subnormal values are quite tricky. So
        // * some of the bounds might not be tight.
        // * 
        // * @param f Float value
        // * @return Double value
        // */
        //public static double FloatToDoubleLower(float f) {
        //  if(Float.isNaN(f)) {
        //    return Double.NaN;
        //  }
        //  if(Float.isInfinite(f)) {
        //    if(f < 0) {
        //      return Double.NEGATIVE_INFINITY;
        //    }
        //    else {
        //      return Double.longBitsToDouble(0x47efffffffffffffl);
        //    }
        //  }
        //  long bits = Double.doubleToRawLongBits((double) f);
        //  if((bits & 0x8000000000000000l) == 0) { // Positive
        //    if(bits == 0l) {
        //      return +0.0d;
        //    }
        //    if(f == Float.MIN_VALUE) {
        //      // bits -= 0xf_ffff_ffff_ffffl;
        //      return Double.longBitsToDouble(0x3690000000000001l);
        //    }
        //    if(Float.MIN_NORMAL > f /* && f >= Double.MIN_NORMAL */) {
        //      // The most tricky case:
        //      // a denormalized float, but a normalized double
        //       long bits2 = Double.doubleToRawLongBits((double) -Math.nextUp(-f));
        //      bits = (bits >>> 1) + (bits2 >>> 1) + 1l; // + (0xfff_ffffl << 18);
        //    }
        //    else {
        //      bits -= 0xfffffffl; // 28 extra bits
        //    }
        //    return Double.longBitsToDouble(bits);
        //  }
        //  else {
        //    if(bits == 0x8000000000000000l) {
        //      return Double.longBitsToDouble(0xb690000000000000l);
        //    }
        //    if(f == -Float.MIN_VALUE) {
        //      // bits += 0x7_ffff_ffff_ffffl;
        //      return Double.longBitsToDouble(0xb6a7ffffffffffffl);
        //    }
        //    if(-Float.MIN_NORMAL < f /* && f <= -Double.MIN_NORMAL */) {
        //      // The most tricky case:
        //      // a denormalized float, but a normalized double
        //       long bits2 = Double.doubleToRawLongBits((double) -Math.nextUp(-f));
        //      bits = (bits >>> 1) + (bits2 >>> 1) - 1l;
        //    }
        //    else {
        //      bits += 0xfffffffl; // 28 extra bits
        //    }
        //    return Double.longBitsToDouble(bits);
        //  }
        /**
  * Fast loop for computing {@code Math.pow(x, p)} for p >= 0 integer.
  * 
  * @param x Base
  * @param p Exponent
  * @return {@code Math.pow(x, p)}
  */
        public static double Powi(double x, int p)
        {
            if (p < 0)
            { // Fallback for negative integers.
                return Math.Pow(x, p);
            }
            double ret = 1.0;
            for (; p > 0; p >>= 1)
            {
                if ((p & 1) == 1)
                {
                    ret *= x;
                }
                x *= x;
            }
            return ret;
        }
    }
}
