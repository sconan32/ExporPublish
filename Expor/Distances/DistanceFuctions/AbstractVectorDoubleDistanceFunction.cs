using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public abstract class AbstractVectorDoubleDistanceFunction :
        AbstractPrimitiveDistanceFunction<INumberVector>,
        IPrimitiveDoubleDistanceFunction<INumberVector>,
        INumberVectorDistanceFunction
    {
        /*
         * Constructor.
         */
        public AbstractVectorDoubleDistanceFunction()
            : base()
        {

        }


        public override ITypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.NUMBER_VECTOR_FIELD;
        }

        public override IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
        }
        public abstract double DoubleDistance(INumberVector o1, INumberVector o2);
        public override IDistanceValue Distance(INumberVector o1, INumberVector o2)
        {

            return new DoubleDistanceValue(this.DoubleDistance(o1, o2));
        }

    }
}
