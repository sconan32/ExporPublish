using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed.SubspaceProj;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.Algorithms.Clustering.Subspace
{

    [Title("PreDeCon: Subspace Preference weighted Density Connected Clustering")]
    [Description("PreDeCon computes clusters of subspace preference weighted connected points. "
        + "The algorithm searches for local subgroups of a set of feature vectors having " +
        "a low variance along one or more (but not all) attributes.")]
    [Reference(Authors = "C. B枚hm, K. Kailing, H.-P. Kriegel, P. Kr枚ger",
        Title = "Density Connected Clustering with Local Subspace Preferences",
        BookTitle = "Proc. 4th IEEE Int. Conf. on Data Mining (ICDM'04), Brighton, UK, 2004",
        Url = "http://dx.doi.org/10.1109/ICDM.2004.10087")]
    public class PreDeCon : AbstractProjectedDBSCAN<INumberVector>
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(PreDeCon));

        /**
         * Constructor.
         * 
         * @param epsilon Epsilon value
         * @param minpts MinPts value
         * @param distanceFunction outer distance function
         * @param lambda Lambda value
         */
        public PreDeCon(DoubleDistanceValue epsilon, int minpts,
            LocallyWeightedDistanceFunction<INumberVector> distanceFunction, int lambda) :
            base(epsilon, minpts, distanceFunction, lambda)
        {
        }


        public override String GetLongResultName()
        {
            return "PreDeCon Clustering";
        }


        public override String GetShortResultName()
        {
            return "predecon-clustering";
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractProjectedDBSCAN<INumberVector>.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                configInnerDistance(config);
                configEpsilon(config, innerdist);
                configMinPts(config);
                configOuterDistance(config, epsilon, minpts, typeof(PreDeConSubspaceIndex<INumberVector>.Factory), innerdist);
                configLambda(config);
            }


            protected override object MakeInstance()
            {
                return new PreDeCon((DoubleDistanceValue)epsilon, minpts, outerdist, lambda);
            }
        }
    }
}
