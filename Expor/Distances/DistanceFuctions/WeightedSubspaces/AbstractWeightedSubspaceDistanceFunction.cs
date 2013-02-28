using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions.WeightedSubspaces
{
    public abstract class AbstractWeightedSubspaceDistanceFunction : AbstractPrimitiveDistanceFunction<INumberVector>,
        IPrimitiveDoubleDistanceFunction<INumberVector>,
         IWeightedSubspaceDistanceFunction,
        ISpatialPrimitiveDoubleDistanceFunction
    {
        WeightSubspace dimWeight;

        public AbstractWeightedSubspaceDistanceFunction(WeightSubspace dimweight)
            : base()
        {
            this.dimWeight = dimweight;
        }
        public override DistanceValues.IDistanceValue Distance(INumberVector o1, INumberVector o2)
        {
            return new DoubleDistanceValue(this.DoubleDistance(o1, o2));
        }

        public override DistanceValues.IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
        }

        public abstract double DoubleDistance(INumberVector o1, INumberVector o2);


        public WeightSubspace DimensionWeight
        {
            get { return dimWeight; }
            set { dimWeight = value; }
        }

        public double MinDoubleDistance(Data.Spatial.ISpatialComparable mbr1, Data.Spatial.ISpatialComparable mbr2)
        {
            return this.DoubleDistance((INumberVector)mbr1, (INumberVector)mbr2);
        }
        public double DoubleDistance(Data.Spatial.ISpatialComparable o1, Data.Spatial.ISpatialComparable o2)
        {
            return this.DoubleDistance((INumberVector)o1, (INumberVector)o2);
        }
        public IDistanceValue MinDistance(Data.Spatial.ISpatialComparable mbr1, Data.Spatial.ISpatialComparable mbr2)
        {
            return this.Distance((INumberVector)mbr1, (INumberVector)mbr2);
        }
        public IDistanceValue Distance(Data.Spatial.ISpatialComparable o1, Data.Spatial.ISpatialComparable o2)
        {
            return this.Distance((INumberVector)o1, (INumberVector)o2);
        }

        public override Data.Types.ITypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.NUMBER_VECTOR_FIELD;
        }
    }
}
