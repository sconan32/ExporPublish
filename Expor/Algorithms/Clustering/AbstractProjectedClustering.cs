using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Algorithms.Clustering
{

    public abstract class AbstractProjectedClustering : AbstractAlgorithm, IClusteringAlgorithm
    {
        /**
         * Parameter to specify the number of clusters to find, must be an integer
         * greater than 0.
         * <p>
         * Key: {@code -projectedclustering.k}
         * </p>
         */
        public static OptionDescription K_ID = OptionDescription.GetOrCreate("projectedclustering.k",
            "The number of clusters to find.");

        /**
         * Parameter to specify the multiplier for the initial number of seeds, must
         * be an integer greater than 0.
         * <p>
         * Default value: {@code 30}
         * </p>
         * <p>
         * Key: {@code -projectedclustering.k_i}
         * </p>
         */
        public static OptionDescription K_I_ID = OptionDescription.GetOrCreate("projectedclustering.k_i",
            "The multiplier for the initial number of seeds.");

        /**
         * Parameter to specify the dimensionality of the clusters to find, must be an
         * integer greater than 0.
         * <p>
         * Key: {@code -projectedclustering.l}
         * </p>
         */
        public static OptionDescription L_ID = OptionDescription.GetOrCreate("projectedclustering.l", 
            "The dimensionality of the clusters to find.");

        /**
         * Holds the value of {@link #K_ID}.
         */
        protected int k;

        /**
         * Holds the value of {@link #K_I_ID}.
         */
        protected int k_i;

        /**
         * Holds the value of {@link #L_ID}.
         */
        protected int l;

        /**
         * The euclidean distance function.
         */
        private IDistanceFunction distanceFunction = EuclideanDistanceFunction.STATIC;

        /**
         * Internal constructor.
         * 
         * @param k K parameter
         * @param k_i K_i parameter
         * @param l L parameter
         */
        public AbstractProjectedClustering(int k, int k_i, int l) :
            base()
        {
            this.k = k;
            this.k_i = k_i;
            this.l = l;
        }

        /**
         * Returns the distance function.
         * 
         * @return the distance function
         */
        protected IDistanceFunction GetDistanceFunction()
        {
            return distanceFunction;
        }

        /**
         * Returns the distance function.
         * 
         * @return the distance function
         */
        protected IDistanceQuery GetDistanceQuery(IDatabase database)
        {
            return QueryUtil.GetDistanceQuery<INumberVector>(database, distanceFunction);
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected int k;

            protected int k_i;

            protected int l;

            /**
             * Get the parameter k, see {@link #K_ID}
             * 
             * @param config Parameterization
             */
            protected void ConfigK(IParameterization config)
            {
                IntParameter kP = new IntParameter(K_ID, new GreaterConstraint<int>(0));
                if (config.Grab(kP))
                {
                    k = kP.GetValue();
                }
            }

            /**
             * Get the parameter k_i, see {@link #K_I_ID}
             * 
             * @param config Parameterization
             */
            protected void ConfigKI(IParameterization config)
            {
                IntParameter k_iP = new IntParameter(K_I_ID, new GreaterConstraint<int>(0), 30);
                if (config.Grab(k_iP))
                {
                    k_i = k_iP.GetValue();
                }
            }

            /**
             * Get the parameter l, see {@link #L_ID}
             * 
             * @param config Parameterization
             */
            protected void ConfigL(IParameterization config)
            {
                IntParameter lP = new IntParameter(L_ID, new GreaterConstraint<int>(0));
                if (config.Grab(lP))
                {
                    l = lP.GetValue();
                }
            }
        }
    }
}
