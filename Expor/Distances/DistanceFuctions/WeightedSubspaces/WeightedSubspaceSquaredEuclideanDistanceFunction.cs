using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Distances.DistanceFuctions.WeightedSubspaces
{
    public class WeightedSubspaceSquaredEuclideanDistanceFunction : AbstractWeightedSubspaceDistanceFunction
    {
        double power;
        public WeightedSubspaceSquaredEuclideanDistanceFunction(WeightSubspace dimweight)
            : this(dimweight, 1.0)
        { }
        public WeightedSubspaceSquaredEuclideanDistanceFunction(WeightSubspace dimweight, double power)
            : base(dimweight)
        {
            this.power = power;
        }
        public override double DoubleDistance(Data.INumberVector v1, Data.INumberVector v2)
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors\n  " +
                    "first argumentin " + v1 + "\n  " + "second argumentin " + v2);
            }

            double sqrDist = 0;
            for (int di = 0; di < DimensionWeight.Count; di++)
            {
                double delta = v1[(di)] - v2[(di)];
                sqrDist += delta * delta * Math.Pow(DimensionWeight.Weights[di], power);
            }
            return sqrDist;
        }


    }
}
