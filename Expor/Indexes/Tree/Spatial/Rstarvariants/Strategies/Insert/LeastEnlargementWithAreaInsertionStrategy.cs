using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert
{

    /**
     * A slight modification of the default R-Tree insertion strategy: find
     * rectangle with least volume enlargement, but choose least area on ties.
     * 
     * Proposed for non-leaf entries in:
     * <p>
     * N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger:<br />
     * The R*-tree: an efficient and robust access method for points and rectangles<br />
     * in: Proceedings of the 1990 ACM SIGMOD International Conference on Management
     * of Data, Atlantic City, NJ, May 23-25, 1990
     * </p>
     * 
     * @author Erich Schubert
     */
    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class LeastEnlargementWithAreaInsertionStrategy : IInsertionStrategy
    {
        /**
         * Static instance.
         */
        public static LeastEnlargementWithAreaInsertionStrategy STATIC =
            new LeastEnlargementWithAreaInsertionStrategy();

        /**
         * Constructor.
         */
        public LeastEnlargementWithAreaInsertionStrategy()
        {

        }


        public int Choose(IEnumerable<ISpatialEntry> options, IArrayAdapter getter,
            ISpatialComparable obj, int height, int depth)
        {
            int size = getter.Size(options);
            Debug.Assert(size > 0, "Choose from empty set?");
            // As in R-Tree, with a slight modification for ties
            double leastEnlargement = Double.PositiveInfinity;
            double minArea = -1;
            int best = -1;
            for (int i = 0; i < size; i++)
            {
                ISpatialComparable entry = (ISpatialComparable)getter.Get(options, i);
                double enlargement = SpatialUtil.Enlargement(entry, obj);
                if (enlargement < leastEnlargement)
                {
                    leastEnlargement = enlargement;
                    best = i;
                    minArea = SpatialUtil.Volume(entry);
                }
                else if (enlargement == leastEnlargement)
                {
                    double area = SpatialUtil.Volume(entry);
                    if (area < minArea)
                    {
                        // Tie handling proposed by R*:
                        best = i;
                        minArea = area;
                    }
                }
            }
            Debug.Assert(best > -1);
            return best;
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

            protected override  object  MakeInstance()
            {
                return LeastEnlargementWithAreaInsertionStrategy.STATIC;
            }
        }
    }
}
