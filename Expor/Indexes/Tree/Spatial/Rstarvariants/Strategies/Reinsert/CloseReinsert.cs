using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Reinsert
{

    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class CloseReinsert : AbstractPartialReinsert
    {
        /**
         * Constructor.
         *
         * @param reinsertAmount Amount of objects to reinsert
         * @param distanceFunction Distance function to use for reinsertion
         */
        public CloseReinsert(double reinsertAmount, ISpatialPrimitiveDoubleDistanceFunction distanceFunction) :
            base(reinsertAmount, distanceFunction)
        {
        }


        public override int[] ComputeReinserts(IEnumerable<ISpatialEntry> entries,
            IArrayAdapter getter, ISpatialComparable page)
        {
            DoubleIntPair[] order = new DoubleIntPair[getter.Size(entries)];
            DoubleVector centroid = new DoubleVector(SpatialUtil.Centroid(page));
            for (int i = 0; i < order.Length; i++)
            {
                double distance = distanceFunction.MinDoubleDistance(new DoubleVector(
                    SpatialUtil.Centroid((getter as ArrayAdapterBase<ISpatialEntry>) .Get(entries, i))), centroid);
                order[i] = new DoubleIntPair(distance, i);
            }
            Array.Sort(order, new Comparison<DoubleIntPair>((o1, o2) => { return -o1.CompareTo(o2); }));

            int num = (int)(reinsertAmount * order.Length);
            int[] re = new int[num];
            for (int i = 0; i < num; i++)
            {
                re[i] = order[num - 1 - i].second;
            }
            return re;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPartialReinsert.Parameterizer
        {

            protected override Object MakeInstance()
            {
                return new CloseReinsert(reinsertAmount, distanceFunction);
            }
        }
    }
}
