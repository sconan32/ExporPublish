using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public abstract class AbstractKMeansInitialization<V> : IKMeansInitialization<V>
        where V : INumberVector
    {
        /**
         * Holds the value of {@link KMeans#SEED_ID}.
         */
        protected long? seed;

        /**
         * Constructor.
         * 
         * @param seed Random seed.
         */
        public AbstractKMeansInitialization(long? seed)
        {
            this.seed = seed;
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
            protected long seed;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                LongParameter seedP = new LongParameter(AbstractKMeans.SEED_ID, true);
                if (config.Grab(seedP))
                {
                    seed = seedP.GetValue();
                }
            }
        }

        public abstract IList<V> ChooseInitialMeans(
            Databases.Relations.IRelation relation,
            int k,
            Distances.DistanceFuctions.IPrimitiveDistanceFunction<V> distanceFunction);

    }
}
