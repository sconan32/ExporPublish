using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;


namespace Socona.Expor.Algorithms
{

    public abstract class AbstractPrimitiveDistanceBasedAlgorithm<O> : AbstractAlgorithm
    {

        /**
       * Holds the instance of the distance function specified by
       * {@link AbstractDistanceBasedAlgorithm#DISTANCE_FUNCTION_ID}.
       */
        private IPrimitiveDistanceFunction<O> distanceFunction;

        /**
         * Constructor.
         * 
         * @param distanceFunction Distance function
         */
        protected AbstractPrimitiveDistanceBasedAlgorithm(IPrimitiveDistanceFunction<O> distanceFunction)
            : base()
        {

            this.distanceFunction = distanceFunction;
        }

        /**
         * Returns the distanceFunction.
         * 
         * @return the distanceFunction
         */
        public IPrimitiveDistanceFunction<O> GetDistanceFunction()
        {
            return distanceFunction;
        }

        /**
         * Parameterization helper class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer :
            AbstractParameterizer
        {
            protected IPrimitiveDistanceFunction<O> distanceFunction;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IPrimitiveDistanceFunction<O>> distanceFunctionP =
                    MakeParameterDistanceFunction<IPrimitiveDistanceFunction<O>>(typeof(EuclideanDistanceFunction), typeof(IPrimitiveDistanceFunction<O>));
                if (config.Grab(distanceFunctionP as IParameter))
                {
                    distanceFunction = distanceFunctionP.InstantiateClass(config);
                }
            }
        }
    }
}
