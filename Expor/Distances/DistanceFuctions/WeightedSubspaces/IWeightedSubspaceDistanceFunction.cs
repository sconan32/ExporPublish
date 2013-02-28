using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Distances.DistanceFuctions.WeightedSubspaces
{
    public interface IWeightedSubspaceDistanceFunction : IPrimitiveDoubleDistanceFunction<INumberVector>
    {
        WeightSubspace DimensionWeight { get; set; }
    }
}
