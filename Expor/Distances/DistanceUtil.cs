using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances
{

    public class DistanceUtil
    {
        /**
         * Returns the maximum of the given Distances or the first, if none is greater
         * than the other one.
         * 
         * @param <D> distance type
         * @param d1 first Distance
         * @param d2 second Distance
         * @return Distance the maximum of the given Distances or the first, if
         *         neither is greater than the other one
         */
        public static IDistanceValue Max(IDistanceValue d1, IDistanceValue d2)
        {
            if (d1 == null)
            {
                return d2;
            }
            if (d2 == null)
            {
                return d1;
            }
            if (d1.CompareTo(d2) > 0)
            {
                return d1;
            }
            else if (d2.CompareTo(d1) > 0)
            {
                return d2;
            }
            else
            {
                return d1;
            }
        }

        /**
         * Returns the minimum of the given Distances or the first, if none is less
         * than the other one.
         * 
         * @param <D> distance type
         * @param d1 first Distance
         * @param d2 second Distance
         * @return Distance the minimum of the given Distances or the first, if
         *         neither is less than the other one
         */
        public static IDistanceValue Min(IDistanceValue d1, IDistanceValue d2)
        {
            if (d1 == null)
            {
                return d2;
            }
            if (d2 == null)
            {
                return d1;
            }
            if (d1.CompareTo(d2) < 0)
            {
                return d1;
            }
            else if (d2.CompareTo(d1) < 0)
            {
                return d2;
            }
            else
            {
                return d1;
            }
        }
    }
}
