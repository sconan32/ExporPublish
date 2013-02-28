using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures.ArrayLike;

namespace Socona.Expor.Data.Spatial
{

    public sealed class SpatialUtil
    {
        /**
         * Returns a clone of the minimum hyper point.
         * 
         * @return the minimum hyper point
         */
        public static double[] GetMin(ISpatialComparable box)
        {
            int dim = box.Count;
            double[] min = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                min[i] = box.GetMin(i + 1);
            }
            return min;
        }

        /**
         * Returns a clone of the maximum hyper point.
         * 
         * @return the maximum hyper point
         */
        public static double[] GetMax(ISpatialComparable box)
        {
            int dim = box.Count;
            double[] max = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                max[i] = box.GetMax(i + 1);
            }
            return max;
        }

        /**
         * Returns true if the two ISpatialComparables intersect, false otherwise.
         * 
         * @param box1 the first ISpatialComparable
         * @param box2 the first ISpatialComparable
         * @return true if the ISpatialComparables intersect, false otherwise
         */
        public static bool Intersects(ISpatialComparable box1, ISpatialComparable box2)
        {
            int dim = box1.Count;
            if (dim != box2.Count)
            {
                throw new ArgumentException("The spatial objects do not have the same dimensionality: " +
                    box1.Count + " " + box2.Count);
            }
            bool intersect = true;
            for (int i = 1; i <= dim; i++)
            {
                if (box1.GetMin(i) > box2.GetMax(i) || box1.GetMax(i) < box2.GetMin(i))
                {
                    intersect = false;
                    break;
                }
            }
            return intersect;
        }

        /**
         * Returns true if the first ISpatialComparable contains the second
         * ISpatialComparable, false otherwise.
         * 
         * @param box1 the outer ISpatialComparable
         * @param box2 the inner ISpatialComparable
         * @return true if the first ISpatialComparable contains the second
         *         ISpatialComparable, false otherwise
         */
        public static bool Contains(ISpatialComparable box1, ISpatialComparable box2)
        {
            int dim = box1.Count;
            if (dim != box2.Count)
            {
                throw new ArgumentException("The spatial objects do not have the same dimensionality!");
            }

            bool contains = true;
            for (int i = 1; i <= dim; i++)
            {
                if (box1.GetMin(i) > box2.GetMin(i) || box1.GetMax(i) < box2.GetMax(i))
                {
                    contains = false;
                    break;
                }
            }
            return contains;
        }

        /**
         * Returns true if this ISpatialComparable contains the given point, false
         * otherwise.
         * 
         * @param point the point to be tested for containment
         * @return true if this ISpatialComparable contains the given point, false
         *         otherwise
         */
        public static bool Contains(ISpatialComparable box, double[] point)
        {
            int dim = box.Count;
            if (dim != point.Length)
            {
                throw new ArgumentException("This HyperBoundingBox and the given point need same dimensionality");
            }

            bool contains = true;
            for (int i = 0; i < dim; i++)
            {
                if (box.GetMin(i + 1) > point[i] || box.GetMax(i + 1) < point[i])
                {
                    contains = false;
                    break;
                }
            }
            return contains;
        }

        /**
         * Computes the volume of this ISpatialComparable
         * 
         * @return the volume of this ISpatialComparable
         */
        public static double Volume(ISpatialComparable box)
        {
            double vol = 1;
            int dim = box.Count;
            for (int i = 1; i <= dim; i++)
            {
                double delta = box.GetMax(i) - box.GetMin(i);
                if (delta == 0.0)
                {
                    return 0.0;
                }
                vol *= delta;
            }
            return vol;
        }

        /**
         * Compute the volume (area) of the union of two MBRs
         * 
         * @param r1 First object
         * @param r2 Second object
         * @return Volume of union
         */
        public static double VolumeUnion(ISpatialComparable r1, ISpatialComparable r2)
        {
            int dim1 = r1.Count;
            int dim2 = r2.Count;
            Debug.Assert(dim1 == dim2, "Computing union with different dimensionality: " + dim1 + " vs. " + dim2);
            double volume = 1.0;
            for (int i = 1; i <= dim1; i++)
            {
                double min = Math.Min(r1.GetMin(i), r2.GetMin(i));
                double max = Math.Max(r1.GetMax(i), r2.GetMax(i));
                volume *= (max - min);
            }
            return volume;
        }

        /**
         * Computes the volume of this ISpatialComparable
         * 
         * @param scale Scaling factor
         * @return the volume of this ISpatialComparable
         */
        public static double VolumeScaled(ISpatialComparable box, double scale)
        {
            double vol = 1;
            int dim = box.Count;
            for (int i = 1; i <= dim; i++)
            {
                double delta = box.GetMax(i) - box.GetMin(i);
                if (delta == 0.0)
                {
                    return 0.0;
                }
                vol *= delta * scale;
            }
            return vol;
        }

        /**
         * Compute the volume (area) of the union of two MBRs
         * 
         * @param r1 First object
         * @param r2 Second object
         * @param scale Scaling factor
         * @return Volume of union
         */
        public static double volumeUnionScaled(ISpatialComparable r1, ISpatialComparable r2, double scale)
        {
            int dim1 = r1.Count;
            int dim2 = r2.Count;
            Debug.Assert(dim1 == dim2, "Computing union with different dimensionality: " + dim1 + " vs. " + dim2);
            double volume = 1.0;
            for (int i = 1; i <= dim1; i++)
            {
                double min = Math.Min(r1.GetMin(i), r2.GetMin(i));
                double max = Math.Max(r1.GetMax(i), r2.GetMax(i));
                volume *= (max - min) * scale;
            }
            return volume;
        }

        /**
         * Compute the enlargement obtained by adding an object to an existing object.
         * 
         * @param exist Existing rectangle
         * @param addit Additional rectangle
         * @return Enlargement factor
         */
        public static double Enlargement(ISpatialComparable exist, ISpatialComparable addit)
        {
            int dim1 = exist.Count;
            int dim2 = addit.Count;
            Debug.Assert(dim1 == dim2, "Computing union with different dimensionality: " + dim1 + " vs. " + dim2);
            double v1 = 1.0;
            double v2 = 1.0;
            for (int i = 1; i <= dim1; i++)
            {
                double emin = exist.GetMin(i);
                double emax = exist.GetMax(i);
                double amin = addit.GetMin(i);
                double amax = addit.GetMax(i);

                double min = Math.Min(emin, amin);
                double max = Math.Max(emax, amax);
                v1 *= (max - min);
                v2 *= (emax - emin);
            }
            return v2 - v1;
        }

        /**
         * Compute the enlargement obtained by adding an object to an existing object.
         * 
         * @param exist Existing rectangle
         * @param addit Additional rectangle
         * @param scale Scaling helper
         * @return Enlargement factor
         */
        public static double EnlargementScaled(ISpatialComparable exist, ISpatialComparable addit, double scale)
        {
            int dim1 = exist.Count;
            int dim2 = addit.Count;
            Debug.Assert(dim1 == dim2, "Computing union with different dimensionality: " + dim1 + " vs. " + dim2);
            double v1 = 1.0;
            double v2 = 1.0;
            for (int i = 1; i <= dim1; i++)
            {
                double emin = exist.GetMin(i);
                double emax = exist.GetMax(i);
                double amin = addit.GetMin(i);
                double amax = addit.GetMax(i);

                double min = Math.Min(emin, amin);
                double max = Math.Max(emax, amax);
                v1 *= (max - min) * scale;
                v2 *= (emax - emin) * scale;
            }
            return v2 - v1;
        }

        /**
         * Computes the perimeter of this ISpatialComparable.
         * 
         * @return the perimeter of this ISpatialComparable
         */
        public static double Perimeter(ISpatialComparable box)
        {
            int dim = box.Count;
            double perimeter = 0;
            for (int i = 1; i <= dim; i++)
            {
                perimeter += box.GetMax(i) - box.GetMin(i);
            }
            return perimeter;
        }

        /**
         * Computes the volume of the overlapping box between two ISpatialComparables.
         * 
         * @param box1 the first ISpatialComparable
         * @param box2 the second ISpatialComparable
         * @return the overlap volume.
         */
        public static double Overlap(ISpatialComparable box1, ISpatialComparable box2)
        {
            int dim = box1.Count;
            if (dim != box2.Count)
            {
                throw new ArgumentException("This HyperBoundingBox and the given HyperBoundingBox need same dimensionality");
            }

            // the maximal and minimal value of the overlap box.
            double omax, omin;

            // the overlap volume
            double overlap = 1.0;

            for (int i = 1; i <= dim; i++)
            {
                // The maximal value of that overlap box in the current
                // dimension is the minimum of the max values.
                omax = Math.Min(box1.GetMax(i), box2.GetMax(i));
                // The minimal value is the maximum of the min values.
                omin = Math.Max(box1.GetMin(i), box2.GetMin(i));

                // if omax <= omin in any dimension, the overlap box has a volume of zero
                if (omax <= omin)
                {
                    return 0.0;
                }

                overlap *= omax - omin;
            }

            return overlap;
        }

        /**
         * Computes the volume of the overlapping box between two ISpatialComparables
         * and return the relation between the volume of the overlapping box and the
         * volume of both ISpatialComparable.
         * 
         * @param box1 the first ISpatialComparable
         * @param box2 the second ISpatialComparable
         * @return the overlap volume in relation to the singular volumes.
         */
        public static double RelativeOverlap(ISpatialComparable box1, ISpatialComparable box2)
        {
            int dim = box1.Count;
            if (dim != box2.Count)
            {
                throw new ArgumentException("This HyperBoundingBox and the given HyperBoundingBox need same dimensionality");
            }

            // the overlap volume
            double overlap = 1.0;
            double vol1 = 1.0;
            double vol2 = 1.0;

            for (int i = 1; i <= dim; i++)
            {
                double box1min = box1.GetMin(i);
                double box1max = box1.GetMax(i);
                double box2min = box2.GetMin(i);
                double box2max = box2.GetMax(i);

                double omax = Math.Min(box1max, box2max);
                double omin = Math.Max(box1min, box2min);

                // if omax <= omin in any dimension, the overlap box has a volume of zero
                if (omax <= omin)
                {
                    return 0.0;
                }

                overlap *= omax - omin;
                vol1 *= box1max - box1min;
                vol2 *= box2max - box2min;
            }

            return overlap / (vol1 + vol2);
        }

        /**
         * Computes the union HyperBoundingBox of two ISpatialComparables.
         * 
         * @param box1 the first ISpatialComparable
         * @param box2 the second ISpatialComparable
         * @return the union HyperBoundingBox of this HyperBoundingBox and the given
         *         HyperBoundingBox
         */
        public static ModifiableHyperBoundingBox Union(ISpatialComparable box1, ISpatialComparable box2)
        {
            int dim = box1.Count;
            if (dim != box2.Count)
            {
                throw new ArgumentException("This HyperBoundingBox and the given HyperBoundingBox need same dimensionality");
            }

            double[] min = new double[dim];
            double[] max = new double[dim];

            for (int i = 1; i <= dim; i++)
            {
                min[i - 1] = Math.Min(box1.GetMin(i), box2.GetMin(i));
                max[i - 1] = Math.Max(box1.GetMax(i), box2.GetMax(i));
            }
            return new ModifiableHyperBoundingBox(min, max);
        }

        /**
         * Returns the union of the two specified MBRs. Tolerant of "null" values.
         * 
         * @param mbr1 the first MBR
         * @param mbr2 the second MBR
         * @return the union of the two specified MBRs
         */
        public static HyperBoundingBox UnionTolerant(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            if (mbr1 == null && mbr2 == null)
            {
                return null;
            }
            if (mbr1 == null)
            {
                // Clone - intentionally
                return new HyperBoundingBox(mbr2);
            }
            if (mbr2 == null)
            {
                // Clone - intentionally
                return new HyperBoundingBox(mbr1);
            }
            return Union(mbr1, mbr2);
        }

        /**
         * Compute the union of a number of objects as a flat MBR (low-level, for
         * index structures)
         * 
         * @param data Object
         * @param Getter Array adapter
         * @return Flat MBR
         */
        public static double[] UnionFlatMBR<E>(IList<E> data, IArrayAdapter getter) where E : ISpatialComparable
        {
            int num = getter.Size(data);
            Debug.Assert(num > 0, "Cannot compute MBR of empty set.");
            E first = (E)getter.Get(data, 0);
            int dim = first.Count;
            double[] mbr = new double[2 * dim];
            for (int d = 0; d < dim; d++)
            {
                mbr[d] = first.GetMin(d + 1);
                mbr[dim + d] = first.GetMax(d + 1);
            }
            for (int i = 1; i < num; i++)
            {
                E next =(E) getter.Get(data, i);
                for (int d = 0; d < dim; d++)
                {
                    mbr[d] = Math.Min(mbr[d], next.GetMin(d + 1));
                    mbr[dim + d] = Math.Max(mbr[dim + d], next.GetMax(d + 1));
                }
            }
            return mbr;
        }

        /**
         * Returns the centroid of this ISpatialComparable.
         * 
         * @param obj Spatial object to process
         * @return the centroid of this ISpatialComparable
         */
        public static double[] Centroid(ISpatialComparable obj)
        {
            int dim = obj.Count;
            double[] centroid = new double[dim];
            for (int d = 1; d <= dim; d++)
            {
                centroid[d - 1] = (obj.GetMax(d) + obj.GetMin(d)) / 2.0;
            }
            return centroid;
        }

        /**
         * Returns the centroid of the specified values of this ISpatialComparable.
         * 
         * @param obj Spatial object to process
         * @param start the start dimension to be considered
         * @param end the end dimension to be considered
         * @return the centroid of the specified values of this ISpatialComparable
         */
        public static double[] Centroid(ISpatialComparable obj, int start, int end)
        {
            double[] centroid = new double[end - start + 1];
            for (int d = start - 1; d < end; d++)
            {
                centroid[d - start + 1] = (obj.GetMax(d + 1) + obj.GetMin(d + 1)) / 2.0;
            }
            return centroid;
        }

        /**
         * Test two ISpatialComparables for equality.
         * 
         * @param box1 First bounding box
         * @param box2 Second bounding box
         * @return true when the boxes are equal
         */
        public static bool Equals(ISpatialComparable box1, ISpatialComparable box2)
        {
            if (box1.Count != box2.Count)
            {
                return false;
            }
            for (int i = 1; i <= box1.Count; i++)
            {
                if (box1.GetMin(i) != box2.GetMin(i))
                {
                    return false;
                }
                if (box1.GetMax(i) != box2.GetMax(i))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
