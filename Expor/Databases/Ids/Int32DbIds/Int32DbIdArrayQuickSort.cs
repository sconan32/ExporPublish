using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class to sort an integer DBID array, using a modified quicksort.
     * 
     * Two array iterators will be used to seek to the elements to compare, while
     * the backing storage is a plain integer array.
     * 
     * The implementation is closely based on:
     * <p>
     * Dual-Pivot Quicksort<br />
     * Vladimir Yaroslavskiy
     * </p>
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses IntegerArrayDBIDs
     */
    [Reference(Authors = "Vladimir Yaroslavskiy",
        Title = "Dual-Pivot Quicksort",
        BookTitle = "http://iaroslavski.narod.ru/quicksort/",
        Url = "http://iaroslavski.narod.ru/quicksort/")]
    public class Int32DbIdArrayQuickSort
    {
        /**
         * Threshold for using insertion sort. Value taken from Javas QuickSort,
         * assuming that it will be similar for DBIDs.
         */
        private static int INSERTION_THRESHOLD = 47;

        /**
         * Sort the full array using the given comparator.
         * 
         * @param data Data to sort
         * @param comp Comparator
         */
        public static void Sort(int[] data, Comparison<IDbIdRef> comp)
        {
            Sort(data, 0, data.Length, comp);
        }

        /**
         * Sort the array using the given comparator.
         * 
         * @param data Data to sort
         * @param start First index
         * @param end Last index (exclusive)
         * @param comp Comparator
         */
        public static void Sort(int[] data, int start, int end, Comparison<IDbIdRef> comp)
        {
            QuickSort(data, start, end - 1, comp, new Int32DbIdVar(), new Int32DbIdVar(), new Int32DbIdVar());
        }

        /**
         * Actual recursive sort function.
         * 
         * @param data Data to sort
         * @param start First index
         * @param end Last index (inclusive!)
         * @param comp Comparator
         * @param vl First seeking iterator
         * @param vk Second seeking iterator
         * @param vr Third seeking iterator
         */
        private static void QuickSort(int[] data, int start, int end, Comparison<IDbIdRef> comp,
            Int32DbIdVar vl, Int32DbIdVar vk, Int32DbIdVar vr)
        {
            int len = end - start;
            if (len < INSERTION_THRESHOLD)
            {
                // Classic insertion sort.
                for (int i = start + 1; i <= end; i++)
                {
                    for (int j = i; j > start; j--)
                    {
                        vl.InternalSetIndex(data[j]);
                        vr.InternalSetIndex(data[j - 1]);
                        if (comp(vl, vr) < 0)
                        {
                            int tmp = data[j - 1];
                            data[j - 1] = data[j];
                            data[j] = tmp;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return;
            }

            // Choose pivots by looking at five candidates.
            int seventh = (len >> 3) + (len >> 6) + 1;
            int m3 = (start + end) >> 1; // middle
            int m2 = m3 - seventh;
            int m1 = m2 - seventh;
            int m4 = m3 + seventh;
            int m5 = m4 + seventh;

            // Explicit (and optimal) sorting network for 5 elements
            // See Knuth for details.
            if (Compare(vl, data[m1], vk, data[m2], comp) > 0)
            {
                int tmp = data[m2];
                data[m2] = data[m1];
                data[m1] = tmp;
            }
            if (Compare(vl, data[m1], vk, data[m3], comp) > 0)
            {
                int tmp = data[m3];
                data[m3] = data[m1];
                data[m1] = tmp;
            }
            if (Compare(vl, data[m2], vk, data[m3], comp) > 0)
            {
                int tmp = data[m3];
                data[m3] = data[m2];
                data[m2] = tmp;
            }
            if (Compare(vl, data[m4], vk, data[m5], comp) > 0)
            {
                int tmp = data[m5];
                data[m5] = data[m4];
                data[m4] = tmp;
            }
            if (Compare(vl, data[m1], vk, data[m4], comp) > 0)
            {
                int tmp = data[m4];
                data[m4] = data[m1];
                data[m1] = tmp;
            }
            if (Compare(vl, data[m3], vk, data[m4], comp) > 0)
            {
                int tmp = data[m4];
                data[m4] = data[m3];
                data[m3] = tmp;
            }
            if (Compare(vl, data[m2], vk, data[m5], comp) > 0)
            {
                int tmp = data[m5];
                data[m5] = data[m2];
                data[m2] = tmp;
            }
            if (Compare(vl, data[m2], vk, data[m3], comp) > 0)
            {
                int tmp = data[m3];
                data[m3] = data[m2];
                data[m2] = tmp;
            }
            if (Compare(vl, data[m4], vk, data[m5], comp) > 0)
            {
                int tmp = data[m5];
                data[m5] = data[m4];
                data[m4] = tmp;
            }

            // Choose the 2 and 4th as pivots, as we want to get three parts
            // Copy to variables v1 and v3, replace them with the start and end
            // Note: do not modify v1 or v3 until we put them back!
            vl.InternalSetIndex(data[m2]);
            vr.InternalSetIndex(data[m4]);
            data[m2] = data[start];
            data[m4] = data[end];

            // A tie is when the two chosen pivots are the same
            bool tied = comp(vl, vr) == 0;

            // Insertion points for pivot areas.
            int left = start + 1;
            int right = end - 1;

            // Note: we merged the ties and no ties cases.
            // This likely is marginally slower, but not at a macro level
            // And you never know with hotspot.
            for (int k = left; k <= right; k++)
            {
                int tmp = data[k];
                vk.InternalSetIndex(tmp);
                int c = comp(vk, vl);
                if (c == 0)
                {
                    continue;
                }
                else if (c < 0)
                {
                    // Traditional quicksort
                    data[k] = data[left];
                    data[left] = tmp;
                    left++;
                }
                else if (tied || comp(vk, vr) > 0)
                {
                    // Now look at the right. First skip correct entries there, too
                    while (true)
                    {
                        vk.InternalSetIndex(data[right]);
                        if (comp(vk, vr) > 0 && k < right)
                        {
                            right--;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // Now move tmp from k to the right.
                    data[k] = data[right];
                    data[right] = tmp;
                    right--;
                    // Test the element we just inserted: left or center?
                    vk.InternalSetIndex(data[k]);
                    if (comp(vk, vl) < 0)
                    {
                        tmp = data[k];
                        data[k] = data[left];
                        data[left] = tmp;
                        left++;
                    } // else: center. cannot be on right.
                }
            }
            // Put the pivot elements back in.
            // Remember: we must not modify v1 and v3 above.
            data[start] = data[left - 1];
            data[left - 1] = vl.InternalGetIndex();
            data[end] = data[right + 1];
            data[right + 1] = vr.InternalGetIndex();
            // v1 and v3 are now safe to modify again. Perform recursion:
            QuickSort(data, start, left - 2, comp, vl, vk, vr);
            // Handle the middle part - if necessary:
            if (!tied)
            {
                // TODO: the original publication had a special tie handling here.
                // It shouldn't affect correctness, but probably improves situations
                // with a lot of tied elements.
                QuickSort(data, left, right, comp, vl, vk, vr);
            }
            QuickSort(data, right + 2, end, comp, vl, vk, vr);
        }

        /**
         * Compare two elements.
         * 
         * @param i1 First scratch variable
         * @param p1 Value for first
         * @param i2 Second scratch variable
         * @param p2 Value for second
         * @param comp Comparator
         * @return Comparison result
         */
        private static int Compare(Int32DbIdVar i1, int p1, Int32DbIdVar i2, int p2, Comparison<IDbIdRef> comp)
        {
            i1.InternalSetIndex(p1);
            i2.InternalSetIndex(p2);
            return comp(i1, i2);
        }
    }

}
