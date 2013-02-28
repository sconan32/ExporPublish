using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Maths;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public class RandomlyGeneratedInitialMeans<V> : AbstractKMeansInitialization<V>
    where V : INumberVector
    {
        /**
         * Constructor.
         * 
         * @param seed Random seed.
         */
        public RandomlyGeneratedInitialMeans(long seed)
            : base(seed)
        {

        }


        public override IList<V> ChooseInitialMeans(IRelation relation, int k, 
            IPrimitiveDistanceFunction<V> distanceFunction)
        {
            int dim = DatabaseUtil.Dimensionality(relation);
            IPair<V, V> minmax = DatabaseUtil.ComputeMinMax<V>(relation);
            IList<V> means = new List<V>(k);
            Random random = (this.seed != null) ? new Random((int)this.seed) : new Random();
            for (int i = 0; i < k; i++)
            {
                double[] r = MathUtil.RandomDoubleArray(dim, random);
                // Rescale
                for (int d = 0; d < dim; d++)
                {
                    r[d] = minmax.First[d ] + (minmax.Second[d ] - minmax.First[d]) * r[d];
                }
                means.Add((V)minmax.First.NewNumberVector(r));
            }
            return means;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractKMeansInitialization<V>.Parameterizer
        {

            protected override object MakeInstance()
            {
                return new RandomlyGeneratedInitialMeans<V>(seed);
            }

           
        }
    }
}
