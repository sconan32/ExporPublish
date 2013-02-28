using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Distances.DistanceFuctions.WeightedSubspaces
{
    class WeightedSubspaceLPNormDistanceFunction : AbstractWeightedSubspaceDistanceFunction
    {
        private double _p = 0;

        public WeightedSubspaceLPNormDistanceFunction(double p, WeightSubspace dimweight)
            : base(dimweight)
        {
            this._p = p;

        }
        public double GetP()
        {
            return _p;
        }
        public override double DoubleDistance(Data.INumberVector o1, Data.INumberVector o2)
        {
            if (o1.Count != o2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argumentin " +
                    o1.ToString() + "\n  " + "second argumentin " + o2.ToString());
            }
            double sqrDist = 0;
            for (int di = 0; di < DimensionWeight.Count; di++)
            {
                double delta;
                double max1 = o1.GetMax(di);
                double min2 = o2.GetMin(di);
                if (max1 < min2)
                {
                    delta = min2 - max1;
                }
                else
                {
                    double min1 = o1.GetMin(di);
                    double max2 = o2.GetMax(di);
                    if (min1 > max2)
                    {
                        delta = min1 - max2;
                    }
                    else
                    { // The mbrs intersect!
                        continue;
                    }
                }
                sqrDist += Math.Pow(delta, _p) * DimensionWeight.Weights[di];
            }
            return Math.Pow(sqrDist, 1.0 / _p);
        }

      
    }
}
