using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;



namespace Socona.Expor.Algorithms
{
    public abstract class AbstractDistanceBasedAlgorithm<O> : AbstractAlgorithm
    {
        protected AbstractDistanceBasedAlgorithm()
        {

           
        }

        protected AbstractDistanceBasedAlgorithm(IDistanceValue distance)
        {

            this.Distance = distance;
        }

        public IDistanceValue Distance { get; set; }

        /**
 * OptionDescription for {@link #DISTANCE_FUNCTION_ID}
 */
        public static readonly OptionDescription DISTANCE_FUNCTION_ID = OptionDescription.GetOrCreate("Algorithm.Distancefunction", "Distance function to determine the distance between database objects.");

        /**
         * Holds the instance of the distance function specified by
         * {@link #DISTANCE_FUNCTION_ID}.
         */
        private IDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param distanceFunction Distance function
         */
        protected AbstractDistanceBasedAlgorithm(IDistanceFunction distanceFunction)
            : base()
        {

            this.distanceFunction = distanceFunction;
        }

        /**
         * Returns the distanceFunction.
         * 
         * @return the distanceFunction
         */
        public IDistanceFunction GetDistanceFunction()
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
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected IDistanceFunction distanceFunction;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IDistanceFunction> distanceFunctionP = 
                    MakeParameterDistanceFunction<IDistanceFunction>(typeof(EuclideanDistanceFunction), typeof(IDistanceFunction));
                if (config.Grab(distanceFunctionP as IParameter))
                {
                    distanceFunction = distanceFunctionP.InstantiateClass(config);
                }
            }
        }
    }
}
