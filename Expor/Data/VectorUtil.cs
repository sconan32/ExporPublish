using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.DataStructures;
namespace Socona.Expor.Data
{

    public class VectorUtil
    {
        /**
         * Return the range across all dimensions. Useful in particular for time
         * series.
         * 
         * @param vec Vector to process.
         * @return [min, max]
         */
        public static DoubleMinMax GetRangeDouble(INumberVector vec)
        {
            DoubleMinMax minmax = new DoubleMinMax();

            for (int i = 0; i < vec.Count; i++)
            {
                minmax.Put(vec[i + 1]);
            }

            return minmax;
        }

        /**
         * Produce a new vector based on random numbers in [0:1] of the same type and
         * dimensionality as the given vector.
         * 
         * @param template existing instance of wanted dimensionality.
         * @param r Random generator
         * @return new instance
         */
        public static V RandomVector<V>(V template, Random r) where V : INumberVector
        {
            return (V)template.NewNumberVector(MathUtil.RandomDoubleArray(template.Count, r));
        }

        /**
         * Produce a new vector based on random numbers in [0:1] of the same type and
         * dimensionality as the given vector.
         * 
         * @param template existing instance of wanted dimensionality.
         * @return new instance
         */
        public static V RandomVector<V>(V template) where V : INumberVector
        {
            return RandomVector(template, new Random());
        }

        /**
         * Compute the angle for sparse vectors.
         * 
         * @param v1 First vector
         * @param v2 Second vector
         * @return angle
         */
        public static double AngleSparse(ISparseNumberVector v1, ISparseNumberVector v2)
        {
            BitArray b1 = v1.GetNotNullMask();
            BitArray b2 = v2.GetNotNullMask();
            BitArray both = (BitArray)b1.Clone();
            both.And(b2);

            // Length of first vector
            double l1 = 0.0;
            for (int i = b1.NextSetBitIndex(0); i >= 0; i = b1.NextSetBitIndex(i + 1))
            {
                double val = v1[i];
                l1 += val * val;
            }
            l1 = Math.Sqrt(l1);

            // Length of second vector
            double l2 = 0.0;
            for (int i = b2.NextSetBitIndex(0); i >= 0; i = b2.NextSetBitIndex(i + 1))
            {
                double val = v2[i];
                l2 += val * val;
            }
            l2 = Math.Sqrt(l2);

            // Cross product
            double cross = 0.0;
            for (int i = both.NextSetBitIndex(0); i >= 0; i = both.NextSetBitIndex(i + 1))
            {
                cross += v1[i] * v2[i];
            }
            return cross / (l1 * l2);
        }

        /**
         * Compute the angle between two vectors.
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @param o Origin
         * @return Angle
         */
        public static double Angle(INumberVector v1, INumberVector v2, Vector o)
        {
            // Essentially, we want to compute this:
            // v1' = v1 - o, v2' = v2 - o
            // v1'.transposeTimes(v2') / (v1'.euclideanLength()*v2'.euclideanLength());
            // We can just compute all three in parallel.
            double[] oe = o.GetArrayRef();
            int dim = v1.Count;
            double s = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < dim; k++)
            {
                double r1 = v1[k + 1] - oe[k];
                double r2 = v2[k + 1] - oe[k];
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
        public static double Angle(INumberVector v1, INumberVector v2, INumberVector o)
        {
            // Essentially, we want to compute this:
            // v1' = v1 - o, v2' = v2 - o
            // v1'.transposeTimes(v2') / (v1'.euclideanLength()*v2'.euclideanLength());
            // We can just compute all three in parallel.
            int dim = v1.Count;
            double s = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < dim; k++)
            {
                double r1 = v1[k + 1] - o[k + 1];
                double r2 = v2[k + 1] - o[k + 1];
                s += r1 * r2;
                e1 += r1 * r1;
                e2 += r2 * r2;
            }
            return Math.Sqrt((s / e1) * (s / e2));
        }

        /**
         * Compute the absolute cosine of the angle between two vectors.
         * 
         * To convert it to radians, use <code>Math.acos(angle)</code>!
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @return Angle
         */
        public static double CosAngle(INumberVector v1, INumberVector v2)
        {
            if (v1 is ISparseNumberVector && v2 is ISparseNumberVector)
            {
                return AngleSparse((ISparseNumberVector)v1, (ISparseNumberVector)v2);
            }
            // Essentially, we want to compute this:
            // v1.transposeTimes(v2) / (v1.euclideanLength() * v2.euclideanLength());
            // We can just compute all three in parallel.
            int d1 = v1.Count;
            int d2 = v2.Count;
            int dim = Math.Min(d1, d2);
            double s = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < dim; k++)
            {
                double r1 = v1[k + 1];
                double r2 = v2[k + 1];
                s += r1 * r2;
                e1 += r1 * r1;
                e2 += r2 * r2;
            }
            for (int k = dim; k < d1; k++)
            {
                double r1 = v1[k + 1];
                e1 += r1 * r1;
            }
            for (int k = dim; k < d2; k++)
            {
                double r2 = v2[k + 1];
                e2 += r2 * r2;
            }
            return Math.Min(Math.Sqrt((s / e1) * (s / e2)), 1);
        }

        // TODO: add more precise but slower O(n^2) angle computation according to:
        // Computing the Angle between Vectors, P. Schatte
        // Journal of Computing, Volume 63, Number 1 (1999)

        /**
         * Compute the minimum angle between two rectangles.
         * 
         * @param v1 first rectangle
         * @param v2 second rectangle
         * @return Angle
         */
        public static double MinCosAngle(ISpatialComparable v1, ISpatialComparable v2)
        {
            if (v1 is INumberVector && v2 is INumberVector)
            {
                return CosAngle((INumberVector)v1, (INumberVector)v2);
            }
            // Essentially, we want to compute this:
            // absmax(v1.transposeTimes(v2))/(min(v1.euclideanLength())*min(v2.euclideanLength()));
            // We can just compute all three in parallel.
            int dim = v1.Count;
            double s1 = 0, s2 = 0, e1 = 0, e2 = 0;
            for (int k = 0; k < dim; k++)
            {
                double min1 = v1.GetMin(k + 1), max1 = v1.GetMax(k + 1);
                double min2 = v2.GetMin(k + 1), max2 = v2.GetMax(k + 1);
                double p1 = min1 * min2, p2 = min1 * max2;
                double p3 = max1 * min2, p4 = max1 * max2;
                s1 += Math.Max(Math.Max(p1, p2), Math.Max(p3, p4));
                s2 += Math.Min(Math.Min(p1, p2), Math.Min(p3, p4));
                if (max1 < 0)
                {
                    e1 += max1 * max1;
                }
                else if (min1 > 0)
                {
                    e1 += min1 * min1;
                } // else: 0
                if (max2 < 0)
                {
                    e2 += max2 * max2;
                }
                else if (min2 > 0)
                {
                    e2 += min2 * min2;
                } // else: 0
            }
            double s = Math.Max(s1, Math.Abs(s2));
            return Math.Min(Math.Sqrt((s / e1) * (s / e2)), 1.0);
        }

        /**
         * Provides the scalar product (inner product) of this and the given
         * DoubleVector.
         * 
         * @param d1 the first vector to compute the scalar product for
         * @param d2 the second vector to compute the scalar product for
         * @return the scalar product (inner product) of this and the given
         *         DoubleVector
         */
        public static double ScalarProduct(INumberVector d1, INumberVector d2)
        {
            int dim = d1.Count;
            double result = 0.0;
            for (int i = 1; i <= dim; i++)
            {
                result += d1[i] * d2[i];
            }
            return result;
        }

        /**
         * Compute medoid for a given subset.
         * 
         * @param relation Relation to process
         * @param sample Sample set
         * @return Medoid vector
         */
        public static Vector ComputeMedoid(IRelation relation, IDbIds sample)
        {
            int dim = DatabaseUtil.Dimensionality(relation);
            IArrayModifiableDbIds mids = DbIdUtil.NewArray(sample);
            SortDbIdsBySingleDimension s = new SortDbIdsBySingleDimension(relation,0);
            Vector medoid = new Vector(dim);
            for (int d = 0; d < dim; d++)
            {
                s.Dimension = (d + 1);
                medoid[d] = ((INumberVector)relation[QuickSelectUtil.Median(mids, s)])[d + 1];
            }
            return medoid;
        }

        /**
         * Compare number vectors by a single dimension
         * 
         * @author Erich Schubert
         */
        public class SortDbIdsBySingleDimension : IComparer<IDbId>
        {
            /**
             * Dimension to sort with
             */
            public int d;

            /**
             * The relation to sort.
             */
            private IRelation data;

            /**
             * Constructor.
             * 
             * @param data Vector data source
             */
            public SortDbIdsBySingleDimension(IRelation data, int d) :
                base()
            {
                this.d = d;
                this.data = data;
            }
            public int Dimension
            {
                get { return d; }
                set { d = value; }
            }

            public int Compare(IDbId id1, IDbId id2)
            {
                return (((IDataVector)data[id1]).Get(d) as IComparable).CompareTo(((IDataVector)data[id2]).Get(d));
            }
        }

        /**
         * Compare number vectors by a single dimension
         * 
         * @author Erich Schubert
         */
        public class SortVectorsBySingleDimension : IComparer<INumberVector>
        {
            /**
             * Dimension to sort with
             */
            public int d;

            /**
             * Constructor.
             */
            public SortVectorsBySingleDimension() :
                base()
            {
            }

            /**
             * Get the dimension to sort by
             * 
             * @return Dimension to sort with
             */
            public int getDimension()
            {
                return this.d;
            }

            /**
             * Set the dimension to sort by
             * 
             * @param d Dimension to sort with
             */
            public void setDimension(int d)
            {
                this.d = d;
            }


            public int Compare(INumberVector o1, INumberVector o2)
            {
                return o1[d].CompareTo(o2[d]);
            }
        }

    }
}
