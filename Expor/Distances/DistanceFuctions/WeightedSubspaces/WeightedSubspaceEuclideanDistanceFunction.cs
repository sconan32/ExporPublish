using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Distances.DistanceFuctions.WeightedSubspaces
{
    class WeightedSubspaceEuclideanDistanceFunction : WeightedSubspaceLPNormDistanceFunction
    {
        public WeightedSubspaceEuclideanDistanceFunction(WeightSubspace dimweight)
            : base(2, dimweight)
        { }
    }
}
