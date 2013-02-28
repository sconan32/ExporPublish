using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert
{

    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class LeastOverlapInsertionStrategy : IInsertionStrategy
    {
        /**
         * Static instance.
         */
        public static LeastOverlapInsertionStrategy STATIC = new LeastOverlapInsertionStrategy();

        /**
         * Constructor.
         */
        public LeastOverlapInsertionStrategy() :
            base()
        {
        }


        public int Choose(IEnumerable<ISpatialEntry> options, IArrayAdapter getter, ISpatialComparable obj, int height, int depth)
        {
            int size = getter.Size(options);
            Debug.Assert(size > 0, "Choose from empty set?");
            // R*-Tree: overlap increase for leaves.
            int best = -1;
            double least_overlap = Double.PositiveInfinity;
            double least_areainc = Double.PositiveInfinity;
            double least_area = Double.PositiveInfinity;
            // least overlap increase, on reduced candidate set:
            for (int i = 0; i < size; i++)
            {
                // Existing object and extended rectangle:
                ISpatialComparable entry = (ISpatialComparable) getter.Get(options, i);
                HyperBoundingBox mbr = SpatialUtil.Union(entry, obj);
                // Compute relative overlap increase.
                double overlap_wout = 0.0;
                double overlap_with = 0.0;
                for (int k = 0; k < size; k++)
                {
                    if (i != k)
                    {
                        ISpatialComparable other = (ISpatialComparable)getter.Get(options, k);
                        overlap_wout += SpatialUtil.RelativeOverlap(entry, other);
                        overlap_with += SpatialUtil.RelativeOverlap(mbr, other);
                    }
                }
                double inc_overlap = overlap_with - overlap_wout;
                if (inc_overlap < least_overlap)
                {
                    double area = SpatialUtil.Volume(entry);
                    double inc_area = SpatialUtil.Volume(mbr) - area;
                    // Volume increase and overlap increase:
                    least_overlap = inc_overlap;
                    least_areainc = inc_area;
                    least_area = area;
                    best = i;
                }
                else if (inc_overlap == least_overlap)
                {
                    double area = SpatialUtil.Volume(entry);
                    double inc_area = SpatialUtil.Volume(mbr) - area;
                    if (inc_area < least_areainc || (inc_area == least_areainc && area < least_area))
                    {
                        least_overlap = inc_overlap;
                        least_areainc = inc_area;
                        least_area = area;
                        best = i;
                    }
                }
            }
            Debug.Assert(best > -1, "No split found? Volume outside of double precision?");
            return best;
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

            protected override object MakeInstance()
            {
                return LeastOverlapInsertionStrategy.STATIC;
            }
        }
    }
}
