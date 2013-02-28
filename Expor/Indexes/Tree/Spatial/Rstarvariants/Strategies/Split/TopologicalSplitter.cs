using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Split
{

    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class TopologicalSplitter : ISplitStrategy
    {
        /**
         * Static instance.
         */
        public static TopologicalSplitter STATIC = new TopologicalSplitter();

        /**
         * constructor.
         */
        public TopologicalSplitter()
        {
            // Nothing to do.
        }


        public BitArray Split<E>(IEnumerable<E> entries, IArrayAdapter getter, int minEntries)
       where E:ISpatialComparable
        {
            Splitter<E> split = new Splitter<E>(entries, getter);
            split.ChooseSplitAxis(minEntries);
            split.ChooseSplitPoint(minEntries);

            Debug.Assert(split.splitPoint < split.size,
                "Invalid split produced. Size: " + getter.Size(entries) +
                " minEntries: " + minEntries + " split.size: " + split.size);
            BitArray assignment = new BitArray(split.size);
            for (int i = split.splitPoint; i < split.size; i++)
            {
                assignment.Set(split.bestSorting[i].second, true);
            }
            return assignment;
        }

        /**
         * Internal data for an actual split.
         * 
         * @author Erich Schubert
         * 
         * @param <E> Actual entry type
         */
        private class Splitter<E>
        where E : ISpatialComparable
        {
            /**
             * The index of the split point.
             */
            internal int splitPoint = -1;

            /**
             * Indicates whether the sorting according to maximal or to minimal value
             * has been used for choosing the split axis and split point.
             */
            internal DoubleIntPair[] bestSorting;

            /**
             * The entries sorted according to their max values of their MBRs.
             */
            internal DoubleIntPair[] maxSorting;

            /**
             * The entries sorted according to their min values of their MBRs.
             */
            internal DoubleIntPair[] minSorting;

            /**
             * The entries we process.
             */
            private IEnumerable<E> entries;

            /**
             * The getter class for the entries
             */
            private IArrayAdapter getter;

            /**
             * List size
             */
            internal int size;

            /**
             * Dimensionality
             */
            private int dimensionality;

            /**
             * Constructor.
             */
            public Splitter(IEnumerable<E> entries, IArrayAdapter getter)
            {
                this.entries = entries;
                this.getter = getter;
                this.size = getter.Size(entries);
                this.dimensionality = ((E)getter.Get(entries, 0)).Count;
                InitMinMaxArrays();
            }

            /**
             * Chooses a split axis.
             * 
             * @param minEntries number of minimum entries in the node to be split
             */
            internal void ChooseSplitAxis(int minEntries)
            {
                // best value for the surface
                double minSurface = Double.MaxValue;
                int splitAxis = -1;

                for (int d = 1; d <= dimensionality; d++)
                {
                    double sumOfAllMargins = 0;
                    FillAndSort(d);

                    // Note: this has a somewhat surprising evaluation order.
                    // We compute the sum as in the original paper:
                    // it says "sum of all margin-values".
                    // Except that we don't match them as you would do in a split, but
                    // Iterate over all possible splits from both sides (as well as min and
                    // max) in parallel, since union can be computed incrementally.
                    ModifiableHyperBoundingBox mbr_min_left = new ModifiableHyperBoundingBox(Get(minSorting[0]));
                    ModifiableHyperBoundingBox mbr_min_right = new ModifiableHyperBoundingBox(Get(minSorting[size - 1]));
                    ModifiableHyperBoundingBox mbr_max_left = new ModifiableHyperBoundingBox(Get(maxSorting[0]));
                    ModifiableHyperBoundingBox mbr_max_right = new ModifiableHyperBoundingBox(Get(maxSorting[size - 1]));

                    for (int k = 1; k < size - minEntries; k++)
                    {
                        mbr_min_left.extend(Get(minSorting[k]));
                        mbr_min_right.extend(Get(minSorting[size - 1 - k]));
                        mbr_max_left.extend(Get(maxSorting[k]));
                        mbr_max_right.extend(Get(maxSorting[size - 1 - k]));
                        if (k >= minEntries - 1)
                        {
                            // Yes, build the sum. This value is solely used for finding the
                            // preferred split axis!
                            // Note that mbr_min_left and mbr_max_left do not add up to a
                            // complete split, but when the sum is complete, it will also
                            // include their proper counterpart.
                            sumOfAllMargins += SpatialUtil.Perimeter(mbr_min_left) + SpatialUtil.Perimeter(mbr_min_right) +
                                SpatialUtil.Perimeter(mbr_max_left) + SpatialUtil.Perimeter(mbr_max_right);
                        }
                    }
                    if (sumOfAllMargins < minSurface)
                    {
                        splitAxis = d;
                        minSurface = sumOfAllMargins;
                    }
                }
                if (splitAxis != dimensionality)
                {
                    FillAndSort(splitAxis);
                }
            }

            /**
             * Init the arrays we use
             */
            protected void InitMinMaxArrays()
            {
                maxSorting = new DoubleIntPair[size];
                minSorting = new DoubleIntPair[size];
                // Prefill
                for (int j = 0; j < size; j++)
                {
                    minSorting[j] = new DoubleIntPair(0, -1);
                    maxSorting[j] = new DoubleIntPair(0, -1);
                }
            }

            /**
             * Fill the array with the dimension projection needed for sorting.
             * 
             * @param dim Relevant dimension.
             */
            protected void FillAndSort(int dim)
            {
                // sort the entries according to their minimal and according to their
                // maximal value in the current dimension.
                for (int j = 0; j < size; j++)
                {
                    E e = Get(j);
                    minSorting[j].first = e.GetMin(dim);
                    minSorting[j].second = j;
                    maxSorting[j].first = e.GetMax(dim);
                    maxSorting[j].second = j;
                }
                Array.Sort(minSorting);
                Array.Sort(maxSorting);
            }

            /**
             * Chooses a split axis.
             * 
             * @param minEntries number of minimum entries in the node to be split
             */
            internal void ChooseSplitPoint(int minEntries)
            {
                // the split point (first set to minimum entries in the node)
                splitPoint = size;
                // best value for the overlap
                double minOverlap = Double.PositiveInfinity;
                // the volume of mbr1 and mbr2
                double volume = Double.PositiveInfinity;
                // indicates whether the sorting according to maximal or to minimal value
                // is best for the split axis
                bestSorting = null;

                Debug.Assert(size - 2 * minEntries > 0, "Cannot split underfull nodes.");
                // test the sorting with respect to the minimal values
                {
                    ModifiableHyperBoundingBox mbr1 = Mbr(minSorting, 0, minEntries - 1);
                    for (int i = 0; i <= size - 2 * minEntries; i++)
                    {
                        mbr1.extend((ISpatialComparable)getter.Get(entries, minSorting[minEntries + i - 1].second));
                        HyperBoundingBox mbr2 = Mbr(minSorting, minEntries + i, size);
                        double currentOverlap = SpatialUtil.RelativeOverlap(mbr1, mbr2);
                        if (currentOverlap <= minOverlap)
                        {
                            double vol = SpatialUtil.Volume(mbr1) + SpatialUtil.Volume(mbr2);
                            if (currentOverlap < minOverlap || vol < volume)
                            {
                                minOverlap = currentOverlap;
                                volume = vol;
                                splitPoint = minEntries + i;
                                bestSorting = minSorting;
                            }
                        }
                    }
                }
                // test the sorting with respect to the maximal values
                {
                    ModifiableHyperBoundingBox mbr1 = Mbr(maxSorting, 0, minEntries - 1);
                    for (int i = 0; i <= size - 2 * minEntries; i++)
                    {
                        mbr1.extend((ISpatialComparable)getter.Get(entries, maxSorting[minEntries + i - 1].second));
                        HyperBoundingBox mbr2 = Mbr(maxSorting, minEntries + i, size);
                        double currentOverlap = SpatialUtil.RelativeOverlap(mbr1, mbr2);
                        if (currentOverlap <= minOverlap)
                        {
                            double vol = SpatialUtil.Volume(mbr1) + SpatialUtil.Volume(mbr2);
                            if (currentOverlap < minOverlap || vol < volume)
                            {
                                minOverlap = currentOverlap;
                                volume = vol;
                                splitPoint = minEntries + i;
                                bestSorting = maxSorting;
                            }
                        }
                    }
                }
                Debug.Assert(splitPoint < size, "No split found? Volume outside of double precision?");
            }

            private E Get(int off)
            {
                return (E)getter.Get(entries, off);
            }

            private E Get(DoubleIntPair pair)
            {
                return (E)getter.Get(entries, pair.second);
            }

            /**
             * Computes and returns the mbr of the specified nodes, only the nodes
             * between from and to index are considered.
             * 
             * @param sorting the array of nodes
             * @param from the start index
             * @param to the end index
             * @return the mbr of the specified nodes
             */
            private ModifiableHyperBoundingBox Mbr(DoubleIntPair[] sorting, int from, int to)
            {
                ModifiableHyperBoundingBox mbr = new ModifiableHyperBoundingBox(Get(sorting[from]));
                for (int i = from + 1; i < to; i++)
                {
                    mbr.extend(Get(sorting[i]));
                }
                return mbr;
            }
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
                return TopologicalSplitter.STATIC;
            }
        }
    }
}