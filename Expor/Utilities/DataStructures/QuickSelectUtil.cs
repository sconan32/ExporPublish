using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Utilities.DataStructures
{

    public class QuickSelectUtil
    {
        /**
         * For small arrays, use a simpler method:
         */
        private static int SMALL = 10;

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param rank Rank position that we are interested in (integer!)
         * @return Value at the given rank
         */
        public static double QuickSelect(double[] data, int rank)
        {
            quickSelect(data, 0, data.Length, rank);
            return data[rank];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @return Median value
         */
        public static double Median(double[] data)
        {
            return median(data, 0, data.Length);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @return Median value
         */
        public static double median(double[] data, int begin, int end)
        {
            int length = end - begin;
            Debug.Assert(length > 0);
            // Integer division is "floor" since we are non-negative.
            int left = begin + (length - 1) / 2;
            quickSelect(data, begin, end, left);
            if (length % 2 == 1)
            {
                return data[left];
            }
            else
            {
                quickSelect(data, begin, end, left + 1);
                return data[left] + (data[left + 1] - data[left]) / 2;
            }
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static double quantile(double[] data, double quant)
        {
            return quantile(data, 0, data.Length, quant);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static double quantile(double[] data, int begin, int end, double quant)
        {
            int length = end - begin;
            Debug.Assert(length > 0, "Quantile on empty set?");
            // Integer division is "floor" since we are non-negative.
            double dleft = begin + (length - 1) * quant;
            int ileft = (int)Math.Floor(dleft);
            double err = dleft - ileft;

            quickSelect(data, begin, end, ileft);
            if (err <= Double.MinValue)
            {
                return data[ileft];
            }
            else
            {
                quickSelect(data, begin, end, ileft + 1);
                // Mix:
                double mix = data[ileft] + (data[ileft + 1] - data[ileft]) * err;
                return mix;
            }
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * @param data Data to process
         * @param start Interval start
         * @param end Interval end (exclusive)
         * @param rank rank position we are interested in (starting at 0)
         */
        public static void quickSelect(double[] data, int start, int end, int rank)
        {
            // Optimization for small arrays
            // This also ensures a minimum.Count below
            if (start + SMALL > end)
            {
                insertionSort(data, start, end);
                return;
            }

            // Pick pivot from three candidates: start, middle, end
            // Since we compare them, we can also just "bubble sort" them.
            int middle = (start + end) / 2;
            if (data[start] > data[middle])
            {
                swap(data, start, middle);
            }
            if (data[start] > data[end - 1])
            {
                swap(data, start, end - 1);
            }
            if (data[middle] > data[end - 1])
            {
                swap(data, middle, end - 1);
            }
            // TODO: use more candidates for larger arrays?

            double pivot = data[middle];
            // Move middle element out of the way, just before end
            // (Since we already know that "end" is bigger)
            swap(data, middle, end - 2);

            // Begin partitioning
            int i = start + 1, j = end - 3;
            // This is classic quicksort stuff
            while (true)
            {
                while (data[i] <= pivot && i <= j)
                {
                    i++;
                }
                while (data[j] >= pivot && j >= i)
                {
                    j--;
                }
                if (i >= j)
                {
                    break;
                }
                swap(data, i, j);
            }

            // Move pivot (former middle element) back into the appropriate place
            swap(data, i, end - 2);

            // In contrast to quicksort, we only need to recurse into the half we are
            // interested in.
            if (rank < i)
            {
                quickSelect(data, start, i, rank);
            }
            else if (rank > i)
            {
                quickSelect(data, i + 1, end, rank);
            }
        }

        /**
         * Sort a small array using repetitive insertion sort.
         * 
         * @param data Data to sort
         * @param start Interval start
         * @param end Interval end
         */
        private static void insertionSort(double[] data, int start, int end)
        {
            for (int i = start + 1; i < end; i++)
            {
                for (int j = i; j > start && data[j - 1] > data[j]; j--)
                {
                    swap(data, j, j - 1);
                }
            }
        }

        /**
         * The usual swap method.
         * 
         * @param data Array
         * @param a First index
         * @param b Second index
         */
        private static void swap(double[] data, int a, int b)
        {
            double tmp = data[a];
            data[a] = data[b];
            data[b] = tmp;
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param rank Rank position that we are interested in (integer!)
         * @return Value at the given rank
         */
        public static T QuickSelect<T>(T[] data, int rank) where T : IComparable
        {
            QuickSelect(data, 0, data.Length, rank);
            return data[rank];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @return Median value
         */
        public static T Median<T>(T[] data) where T : IComparable
        {
            return Median(data, 0, data.Length);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * On an odd length, it will return the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @return Median value
         */
        public static T Median<T>(T[] data, int begin, int end) where T : IComparable
        {
            int length = end - begin;
            Debug.Assert(length > 0);
            // Integer division is "floor" since we are non-negative.
            int left = begin + (length - 1) / 2;
            QuickSelect(data, begin, end, left);
            return data[left];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(T[] data, double quant) where T : IComparable
        {
            return Quantile(data, 0, data.Length, quant);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * It will prefer the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(T[] data, int begin, int end, double quant) where T : IComparable
        {
            int length = end - begin;
            Debug.Assert(length > 0, "Quantile on empty set?");
            // Integer division is "floor" since we are non-negative.
            double dleft = begin + (length - 1) * quant;
            int ileft = (int)Math.Floor(dleft);

            QuickSelect(data, begin, end, ileft);
            return data[ileft];
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param start Interval start
         * @param end Interval end (exclusive)
         * @param rank rank position we are interested in (starting at 0)
         */
        public static void QuickSelect<T>(T[] data, int start, int end, int rank) where T:IComparable
        {
            // Optimization for small arrays
            // This also ensures a minimum.Count below
            if (start + SMALL > end)
            {
                InsertionSort(data, start, end);
                return;
            }

            // Pick pivot from three candidates: start, middle, end
            // Since we compare them, we can also just "bubble sort" them.
            int middle = (start + end) / 2;
            if (data[start].CompareTo(data[middle]) > 0)
            {
                Swap(data, start, middle);
            }
            if (data[start].CompareTo(data[end - 1]) > 0)
            {
                Swap(data, start, end - 1);
            }
            if (data[middle].CompareTo(data[end - 1]) > 0)
            {
                Swap(data, middle, end - 1);
            }
            // TODO: use more candidates for larger arrays?

            T pivot = data[middle];
            // Move middle element out of the way, just before end
            // (Since we already know that "end" is bigger)
            Swap(data, middle, end - 2);

            // Begin partitioning
            int i = start + 1, j = end - 3;
            // This is classic quicksort stuff
            while (true)
            {
                while (data[i].CompareTo(pivot) <= 0 && i <= j)
                {
                    i++;
                }
                while (data[j].CompareTo(pivot) >= 0 && j >= i)
                {
                    j--;
                }
                if (i >= j)
                {
                    break;
                }
                Swap(data, i, j);
            }

            // Move pivot (former middle element) back into the appropriate place
            Swap(data, i, end - 2);

            // In contrast to quicksort, we only need to recurse into the half we are
            // interested in.
            if (rank < i)
            {
                QuickSelect(data, start, i, rank);
            }
            else if (rank > i)
            {
                QuickSelect(data, i + 1, end, rank);
            }
        }

        /**
         * Sort a small array using repetitive insertion sort.
         * 
         * @param <T> object type
         * @param data Data to sort
         * @param start Interval start
         * @param end Interval end
         */
        private static void InsertionSort<T>(T[] data, int start, int end) where T : IComparable
        {
            for (int i = start + 1; i < end; i++)
            {
                for (int j = i; j > start && data[j - 1].CompareTo(data[j]) > 0; j--)
                {
                    Swap(data, j, j - 1);
                }
            }
        }

        /**
         * The usual swap method.
         * 
         * @param <T> object type
         * @param data Array
         * @param a First index
         * @param b Second index
         */
        private static void Swap<T>(T[] data, int a, int b)
        {
            T tmp = data[a];
            data[a] = data[b];
            data[b] = tmp;
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param rank Rank position that we are interested in (integer!)
         * @return Value at the given rank
         */
        public static T QuickSelect<T>(IList<T> data, int rank) where T : IComparable
        {
            QuickSelect(data, 0, data.Count, rank);
            return data[rank];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @return Median value
         */
        public static T Median<T>(IList<T> data) where T : IComparable
        {
            return Median(data, 0, data.Count());
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * On an odd length, it will return the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @return Median value
         */
        public static T Median<T>(IList<T> data, int begin, int end) where T : IComparable
        {
            int length = end - begin;
            Debug.Assert(length > 0);
            // Integer division is "floor" since we are non-negative.
            int left = begin + (length - 1) / 2;
            QuickSelect(data, begin, end, left);
            return data[left];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(IList<T> data, double quant) where T : IComparable
        {
            return Quantile(data, 0, data.Count, quant);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * It will prefer the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(IList<T> data, int begin, int end, double quant) where T : IComparable
        {
            int length = end - begin;
            Debug.Assert(length > 0, "Quantile on empty set?");
            // Integer division is "floor" since we are non-negative.
            double dleft = begin + (length - 1) * quant;
            int ileft = (int)Math.Floor(dleft);

            QuickSelect(data, begin, end, ileft);
            return data[ileft];
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param start Interval start
         * @param end Interval end (exclusive)
         * @param rank rank position we are interested in (starting at 0)
         */
        public static void QuickSelect<T>(IList<T> data, int start, int end, int rank) where T : IComparable
        {
            // Optimization for small arrays
            // This also ensures a minimum.Count below
            if (start + SMALL > end)
            {
                InsertionSort(data, start, end);
                return;
            }

            // Pick pivot from three candidates: start, middle, end
            // Since we compare them, we can also just "bubble sort" them.
            int middle = (start + end) / 2;
            if (data[start].CompareTo(data[middle]) > 0)
            {
                Swap(data, start, middle);
            }
            if (data[start].CompareTo(data[end - 1]) > 0)
            {
                Swap(data, start, end - 1);
            }
            if (data[middle].CompareTo(data[end - 1]) > 0)
            {
                Swap(data, middle, end - 1);
            }
            // TODO: use more candidates for larger arrays?

            T pivot = data[middle];
            // Move middle element out of the way, just before end
            // (Since we already know that "end" is bigger)
            Swap(data, middle, end - 2);

            // Begin partitioning
            int i = start + 1, j = end - 3;
            // This is classic quicksort stuff
            while (true)
            {
                while (data[i].CompareTo(pivot) <= 0 && i <= j)
                {
                    i++;
                }
                while (data[j].CompareTo(pivot) >= 0 && j >= i)
                {
                    j--;
                }
                if (i >= j)
                {
                    break;
                }
                Swap(data, i, j);
            }

            // Move pivot (former middle element) back into the appropriate place
            Swap(data, i, end - 2);

            // In contrast to quicksort, we only need to recurse into the half we are
            // interested in.
            if (rank < i)
            {
                QuickSelect(data, start, i, rank);
            }
            else if (rank > i)
            {
                QuickSelect(data, i + 1, end, rank);
            }
        }

        /**
         * Sort a small array using repetitive insertion sort.
         * 
         * @param <T> object type
         * @param data Data to sort
         * @param start Interval start
         * @param end Interval end
         */
        private static void InsertionSort<T>(IList<T> data, int start, int end) where T : IComparable
        {
            for (int i = start + 1; i < end; i++)
            {
                for (int j = i; j > start && data[j - 1].CompareTo(data[j]) > 0; j--)
                {
                    Swap<T>(data, j, j - 1);
                }
            }
        }

        /**
         * The usual swap method.
         * 
         * @param <T> object type
         * @param data Array
         * @param a First index
         * @param b Second index
         */
        private static void Swap<T>(IList<T> data, int a, int b)
        {
            T tmp = data[b];
            data[b] = data[a];
            data[a] = tmp; ;
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @param rank Rank position that we are interested in (integer!)
         * @return Value at the given rank
         */
        public static T QuickSelect<T>(IList<T> data, IComparer<T> comparator, int rank)
        {
            QuickSelect(data, comparator, 0, data.Count, rank);
            return data[rank];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @return Median value
         */
        public static T Median<T>(IList<T> data, IComparer<T> comparator)
        {
            return Median(data, comparator, 0, data.Count);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * On an odd length, it will return the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @return Median value
         */
        public static T Median<T>(IList<T> data, IComparer<T> comparator, int begin, int end)
        {
            int length = end - begin;
            Debug.Assert(length > 0);
            // Integer division is "floor" since we are non-negative.
            int left = begin + (length - 1) / 2;
            QuickSelect(data, comparator, begin, end, left);
            return data[left];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(IList<T> data, IComparer<T> comparator, double quant)
        {
            return Quantile(data, comparator, 0, data.Count, quant);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * It will prefer the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @param begin Begin of valid values
         * @param end End of valid values (inclusive!)
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static T Quantile<T>(IList<T> data, IComparer<T> comparator, int begin, int end, double quant)
        {
            int length = end - begin;
            Debug.Assert(length > 0, "Quantile on empty set?");
            // Integer division is "floor" since we are non-negative.
            double dleft = begin + (length - 1) * quant;
            int ileft = (int)Math.Floor(dleft);

            QuickSelect(data, comparator, begin, end, ileft);
            return data[ileft];
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * @param <T> object type
         * @param data Data to process
         * @param comparator Comparator to use
         * @param start Interval start
         * @param end Interval end (inclusive)
         * @param rank rank position we are interested in (starting at 0)
         */
        public static void QuickSelect<T>(IList<T> data, IComparer<T> comparator, int start, int end, int rank)
        {
            // Optimization for small arrays
            // This also ensures a minimum.Count below
            if (start + SMALL > end)
            {
                InsertionSort(data, comparator, start, end);
                return;
            }

            // Pick pivot from three candidates: start, middle, end
            // Since we compare them, we can also just "bubble sort" them.
            int middle = (start + end) / 2;
            if (comparator.Compare(data[start], data[middle]) > 0)
            {
                Swap(data, start, middle);
            }
            if (comparator.Compare(data[start], data[end - 1]) > 0)
            {
                Swap(data, start, end - 1);
            }
            if (comparator.Compare(data[middle], data[end - 1]) > 0)
            {
                Swap(data, middle, end - 1);
            }
            // TODO: use more candidates for larger arrays?

            T pivot = data[middle];
            // Move middle element out of the way, just before end
            // (Since we already know that "end" is bigger)
            Swap(data, middle, end - 2);

            // Begin partitioning
            int i = start + 1, j = end - 3;
            // This is classic quicksort stuff
            while (true)
            {
                while (comparator.Compare(data[i], pivot) <= 0 && i <= j)
                {
                    i++;
                }
                while (comparator.Compare(data[j], pivot) >= 0 && j >= i)
                {
                    j--;
                }
                if (i >= j)
                {
                    break;
                }
                Swap(data, i, j);
            }

            // Move pivot (former middle element) back into the appropriate place
            Swap(data, i, end - 2);

            // In contrast to quicksort, we only need to recurse into the half we are
            // interested in.
            if (rank < i)
            {
                QuickSelect(data, comparator, start, i, rank);
            }
            else if (rank > i)
            {
                QuickSelect(data, comparator, i + 1, end, rank);
            }
        }

        /**
         * Sort a small array using repetitive insertion sort.
         * 
         * @param <T> object type
         * @param data Data to sort
         * @param start Interval start
         * @param end Interval end
         */
        private static void InsertionSort<T>(IList<T> data, IComparer<T> comparator, int start, int end)
        {
            for (int i = start + 1; i < end; i++)
            {
                for (int j = i; j > start && comparator.Compare(data[j - 1], data[j]) > 0; j--)
                {
                    Swap(data, j, j - 1);
                }
            }
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @param rank Rank position that we are interested in (integer!)
         * @return Value at the given rank
         */
        public static IDbId QuickSelect(IArrayModifiableDbIds data, IComparer<IDbId> comparator, int rank)
        {
            QuickSelect(data, comparator, 0, data.Count, rank);
            return data[rank];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @return Median value
         */
        public static IDbId Median(IArrayModifiableDbIds data, IComparer<IDbId> comparator)
        {
            return Median(data, comparator, 0, data.Count);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * On an odd length, it will return the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @param begin Begin of valid values
         * @param end End of valid values (exclusive!)
         * @return Median value
         */
        public static IDbId Median(IArrayModifiableDbIds data, IComparer<IDbId> comparator, int begin, int end)
        {
            int length = end - begin;
            Debug.Assert(length > 0);
            // Integer division is "floor" since we are non-negative.
            int left = begin + (length - 1) / 2;
            QuickSelect(data, comparator, begin, end, left);
            return data[left];
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static IDbId Quantile(IArrayModifiableDbIds data, IComparer<IDbId> comparator, double quant)
        {
            return Quantile(data, comparator, 0, data.Count, quant);
        }

        /**
         * Compute the median of an array efficiently using the QuickSelect method.
         * 
         * It will prefer the lower element.
         * 
         * Note: the array is <b>modified</b> by this.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @param begin Begin of valid values
         * @param end End of valid values (inclusive!)
         * @param quant Quantile to compute
         * @return Value at quantile
         */
        public static IDbId Quantile(IArrayModifiableDbIds data, IComparer<IDbId> comparator, int begin, int end, double quant)
        {
            int length = end - begin;
            Debug.Assert(length > 0, "Quantile on empty set?");
            // Integer division is "floor" since we are non-negative.
            double dleft = begin + (length - 1) * quant;
            int ileft = (int)Math.Floor(dleft);

            QuickSelect(data, comparator, begin, end, ileft);
            return data[ileft];
        }

        /**
         * QuickSelect is essentially quicksort, except that we only "sort" that half
         * of the array that we are interested in.
         * 
         * @param data Data to process
         * @param comparator Comparator to use
         * @param start Interval start
         * @param end Interval end (inclusive)
         * @param rank rank position we are interested in (starting at 0)
         */
        public static void QuickSelect(IArrayModifiableDbIds data, IComparer<IDbId> comparator, int start, int end, int rank)
        {
            // Optimization for small arrays
            // This also ensures a minimum.Count below
            if (start + SMALL > end)
            {
                InsertionSort(data, comparator, start, end);
                return;
            }

            // Pick pivot from three candidates: start, middle, end
            // Since we compare them, we can also just "bubble sort" them.
            int middle = (start + end) / 2;
            if (comparator.Compare(data[start], data[(middle)]) > 0)
            {
                data.Swap(start, middle);
            }
            if (comparator.Compare(data[start], data[end - 1]) > 0)
            {
                data.Swap(start, end - 1);
            }
            if (comparator.Compare(data[(middle)], data[(end - 1)]) > 0)
            {
                data.Swap(middle, end - 1);
            }
            // TODO: use more candidates for larger arrays?

            IDbId pivot = data[middle];
            // Move middle element out of the way, just before end
            // (Since we already know that "end" is bigger)
            data.Swap(middle, end - 2);

            // Begin partitioning
            int i = start + 1, j = end - 3;
            // This is classic quicksort stuff
            while (true)
            {
                while (comparator.Compare(data[i], pivot) <= 0 && i <= j)
                {
                    i++;
                }
                while (comparator.Compare(data[j], pivot) >= 0 && j >= i)
                {
                    j--;
                }
                if (i >= j)
                {
                    break;
                }
                data.Swap(i, j);
            }

            // Move pivot (former middle element) back into the appropriate place
            data.Swap(i, end - 2);

            // In contrast to quicksort, we only need to recurse into the half we are
            // interested in.
            if (rank < i)
            {
                QuickSelect(data, comparator, start, i, rank);
            }
            else if (rank > i)
            {
                QuickSelect(data, comparator, i + 1, end, rank);
            }
        }

        /**
         * Sort a small array using repetitive insertion sort.
         * 
         * @param data Data to sort
         * @param start Interval start
         * @param end Interval end
         */
        private static void InsertionSort(IArrayModifiableDbIds data, IComparer<IDbId> comparator, int start, int end)
        {
            for (int i = start + 1; i < end; i++)
            {
                for (int j = i; j > start && comparator.Compare(data[j - 1], data[j]) > 0; j--)
                {
                    data.Swap(j, j - 1);
                }
            }
        }

    }
}
