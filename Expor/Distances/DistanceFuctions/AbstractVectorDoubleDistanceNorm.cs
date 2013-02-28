using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{
    public abstract class AbstractVectorDoubleDistanceNorm : AbstractVectorDoubleDistanceFunction, IDoubleNorm<INumberVector>
    {

        public IDistanceValue Norm(INumberVector obj)
        {
            return new DoubleDistanceValue(DoubleNorm(obj));
        }

        public abstract double DoubleNorm(INumberVector obj);
    }
}
