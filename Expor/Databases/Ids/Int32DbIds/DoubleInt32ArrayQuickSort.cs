using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class to sort a double and an integer DBID array, using a quicksort with a
     * best of 5 heuristic.
     * 
     * @author Erich Schubert
     */
    public class DoubleInt32ArrayQuickSort
    {
        /**
         * Threshold for using insertion sort.
         */
        private static int INSERTION_THRESHOLD = 22;

        /**
         * Sort the full array using the given comparator.
         * 
         * @param keys Keys for sorting
         * @param values Values for sorting
         * @param len Length to sort.
         */
        public static void Sort(double[] keys, int[] values, int len)
        {
            Sort(keys, values, 0, len);
        }

        /**
         * Sort the array using the given comparator.
         * 
         * @param keys Keys for sorting
         * @param values Values for sorting
         * @param start First index
         * @param end Last index (exclusive)
         */
        public static void Sort(double[] keys, int[] values, int start, int end)
        {
            QuickSort(keys, values, start, end);
        }

        /**
         * Actual recursive sort function.
         * 
         * @param keys Keys for sorting
         * @param vals Values for sorting
         * @param start First index
         * @param end Last index (exclusive!)
         */
        private static void QuickSort(double[] keys, int[] vals, int start, int end)
        {
            int len = end - start;
            if (len < INSERTION_THRESHOLD)
            {
                // Classic insertion sort.
                for (int i = start + 1; i < end; i++)
                {
                    for (int j = i; j > start; j--)
                    {
                        if (keys[j] < keys[j - 1])
                        {
                            Swap(keys, vals, j, j - 1);
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

            // Mixture of insertion and merge sort:
            if (keys[m1] > keys[m2])
            {
                Swap(keys, vals, m1, m2);
            }
            if (keys[m3] > keys[m4])
            {
                Swap(keys, vals, m3, m4);
            }
            // Merge 1+2 and 3+4
            if (keys[m2] > keys[m4])
            {
                Swap(keys, vals, m2, m4);
            }
            if (keys[m1] > keys[m3])
            {
                Swap(keys, vals, m1, m3);
            }
            if (keys[m2] > keys[m3])
            {
                Swap(keys, vals, m2, m3);
            }
            // Insertion sort m5:
            if (keys[m4] > keys[m5])
            {
                Swap(keys, vals, m4, m5);
                if (keys[m3] > keys[m4])
                {
                    Swap(keys, vals, m3, m4);
                    if (keys[m2] > keys[m3])
                    {
                        Swap(keys, vals, m2, m3);
                        if (keys[m1] > keys[m1])
                        {
                            Swap(keys, vals, m1, m2);
                        }
                    }
                }
            }

            // Move pivot to the front.
            double pivotkey = keys[m3];
            int pivotval = vals[m3];
            keys[m3] = keys[start];
            vals[m3] = vals[start];

            // The interval to pivotize
            int left = start + 1;
            int right = end - 1;

            // This is the classic QuickSort loop:
            while (true)
            {
                while (left <= right && keys[left] <= pivotkey)
                {
                    left++;
                }
                while (left <= right && pivotkey <= keys[right])
                {
                    right--;
                }
                if (right <= left)
                {
                    break;
                }
                Swap(keys, vals, left, right);
                left++;
                right--;
            }

            // Move pivot back into the appropriate place
            keys[start] = keys[right];
            vals[start] = vals[right];
            keys[right] = pivotkey;
            vals[right] = pivotval;

            // Recursion:
            if (start + 1 < right)
            {
                QuickSort(keys, vals, start, right);
            }
            if (right + 2 < end)
            {
                QuickSort(keys, vals, right + 1, end);
            }
        }

        /**
         * Swap two entries.
         * 
         * @param keys Keys
         * @param vals Values
         * @param j First index
         * @param i Second index
         */
        private static void Swap(double[] keys, int[] vals, int j, int i)
        {
            double td = keys[j];
            keys[j] = keys[i];
            keys[i] = td;
            int ti = vals[j];
            vals[j] = vals[i];
            vals[i] = ti;
        }
    }

}
