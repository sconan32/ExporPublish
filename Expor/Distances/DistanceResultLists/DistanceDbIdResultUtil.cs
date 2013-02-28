using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceResultLists
{

    /**
     * Utility classes for distance DBID results.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses DistanceDBIDPair oneway - -
     */
    public sealed class DistanceDbIdResultUtil
    {
        /**
         * Get a comparator to sort by distance, then DBID
         * 
         * @return comparator
         */

        public static Comparison<IDistanceDbIdPair> DistanceComparator()
        {
            return BY_DISTANCE_THEN_DBID;
        }

        /**
         * Sort a Java list by distance.
         * 
         * @param list List to sort
         */

        public static void SortByDistance(List<IDistanceDbIdPair> list)
        {
            list.Sort(BY_DISTANCE_THEN_DBID);
        }

        /**
         * Static comparator.
         */
        private static Comparison<IDistanceDbIdPair> BY_DISTANCE_THEN_DBID =
            (v1, v2) =>
            {
                int d = ((IDistanceDbIdPair)v1).CompareByDistance((IDistanceDbIdPair)v2);
                return (d == 0) ? DbIdUtil.Compare(v1, v2) : d;
            };


        /**
         * Static comparator for heaps.
         */
        public static Comparison<IDistanceDbIdPair> BY_REVERSE_DISTANCE =
           (o1, o2) =>
           {
               return -((IDistanceDbIdPair)o1).CompareByDistance((IDistanceDbIdPair)o2);
           };

        public static String ToString(IDistanceDbIdList res)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append('[');
            // DistanceDBIDListIter<?> iter = res.iter();
            //for(; iter.valid(); iter.advance()) {
            foreach (var iter in (IEnumerable<IDistanceDbIdPair>)res)
            {
                if (buf.Length > 1)
                {
                    buf.Append(',').Append(' ');
                }
                buf.Append((iter as IDistanceDbIdPair).ToString());
            }
            buf.Append(']');
            return buf.ToString();
        }
    }
}
