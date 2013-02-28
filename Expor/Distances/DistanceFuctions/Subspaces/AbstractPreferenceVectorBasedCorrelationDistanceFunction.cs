using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes;
using Socona.Expor.Indexes.Preprocessed.Preference;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public abstract class AbstractPreferenceVectorBasedCorrelationDistanceFunction<V>
        : AbstractIndexBasedDistanceFunction<V>
      where V : INumberVector
    {
        /**
         * Parameter to specify the maximum distance between two vectors with equal
         * preference vectors before considering them as parallel, must be a double
         * equal to or greater than 0.
         * <p>
         * Default value: {@code 0.001}
         * </p>
         * <p>
         * Key: {@code -pvbasedcorrelationdf.epsilon}
         * </p>
         */
        public static OptionDescription EPSILON_ID =
            OptionDescription.GetOrCreate("distancefunction.epsilon", "The maximum distance between two vectors with equal preference vectors before considering them as parallel.");

        /**
         * Holds the value of {@link #EPSILON_ID}.
         */
        private double epsilon;

        /**
         * Constructor.
         * 
         * @param indexFactory Index factory
         * @param epsilon Epsilon value
         */
        public AbstractPreferenceVectorBasedCorrelationDistanceFunction(IIndexFactory indexFactory, double epsilon) :
            base(indexFactory)
        {
            this.epsilon = epsilon;
        }


        public override IDistanceValue DistanceFactory
        {
            get { return PreferenceVectorBasedCorrelationDistance.FACTORY; }
        }

        /**
         * Returns epsilon.
         * 
         * @return epsilon
         */
        public double GetEpsilon()
        {
            return epsilon;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            AbstractPreferenceVectorBasedCorrelationDistanceFunction<V> other =
                (AbstractPreferenceVectorBasedCorrelationDistanceFunction<V>)obj;
            return (this.indexFactory.Equals(other.indexFactory)) && this.epsilon == other.epsilon;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ indexFactory.GetHashCode() ^ epsilon.GetHashCode();
        }
        /**
         * Instance to compute the distances on an actual database.
         * 
         * @author Erich Schubert
         */
        abstract new public class Instance : AbstractIndexBasedDistanceFunction<V>.Instance
        {
            /**
             * The epsilon value
             */
            protected double epsilon;

            /**
             * Constructor.
             * 
             * @param database IDatabase
             * @param preprocessor Preprocessor
             * @param epsilon Epsilon
             * @param distanceFunction parent distance function
             */
            public Instance(IRelation database, IPreferenceVectorIndex preprocessor,
                double epsilon, AbstractPreferenceVectorBasedCorrelationDistanceFunction<V> distanceFunction) :
                base(database, preprocessor, distanceFunction)
            {
                this.epsilon = epsilon;
            }


            public override IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
            {
                BitArray preferenceVector1 = ((IPreferenceVectorIndex)index).GetPreferenceVector(id1);
                BitArray preferenceVector2 = ((IPreferenceVectorIndex)index).GetPreferenceVector(id2);
                V v1 = (V)relation[(id1)];
                V v2 = (V)relation[(id2)];
                return CorrelationDistance(v1, v2, preferenceVector1, preferenceVector2);
            }

            /**
             * Computes the correlation distance between the two specified vectors
             * according to the specified preference vectors.
             * 
             * @param v1 first vector
             * @param v2 second vector
             * @param pv1 the first preference vector
             * @param pv2 the second preference vector
             * @return the correlation distance between the two specified vectors
             */
            public abstract PreferenceVectorBasedCorrelationDistance CorrelationDistance(V v1, V v2, BitArray pv1, BitArray pv2);

            /**
             * Computes the weighted distance between the two specified vectors
             * according to the given preference vector.
             * 
             * @param v1 the first vector
             * @param v2 the second vector
             * @param weightVector the preference vector
             * @return the weighted distance between the two specified vectors according
             *         to the given preference vector
             */
            public double WeightedDistance(V v1, V v2, BitArray weightVector)
            {
                if (v1.Count != v2.Count)
                {
                    throw new ArgumentException("Different dimensionality of FeatureVectors\n  first argument: " +
                        v1.ToString() + "\n  second argument: " + v2.ToString());
                }

                double sqrDist = 0;
                for (int i = 1; i <= v1.Count; i++)
                {
                    if (weightVector.Get(i - 1))
                    {
                        double manhattanI = (double)v1.Get(i) - (double)v2.Get(i);
                        sqrDist += manhattanI * manhattanI;
                    }
                }
                return Math.Sqrt(sqrDist);
            }

            /**
             * Computes the weighted distance between the two specified vectors
             * according to the given preference vector.
             * 
             * @param id1 the id of the first vector
             * @param id2 the id of the second vector
             * @param weightVector the preference vector
             * @return the weighted distance between the two specified vectors according
             *         to the given preference vector
             */
            public double WeightedDistance(IDbId id1, IDbId id2, BitArray weightVector)
            {
                return WeightedDistance((V)relation[(id1)], (V)relation[id2], weightVector);
            }

            /**
             * Computes the weighted distance between the two specified data vectors
             * according to their preference vectors.
             * 
             * @param id1 the id of the first vector
             * @param id2 the id of the second vector
             * @return the weighted distance between the two specified vectors according
             *         to the preference vector of the first data vector
             */
            public double WeightedPrefereneceVectorDistance(IDbId id1, IDbId id2)
            {
                V v1 = (V)relation[(id1)];
                V v2 = (V)relation[(id2)];
                double d1 = WeightedDistance(v1, v2,
                    ((IPreferenceVectorIndex)index).GetPreferenceVector(id1));
                double d2 = WeightedDistance(v2, v1,
                    ((IPreferenceVectorIndex)index).GetPreferenceVector(id2));
                return Math.Max(d1, d2);
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new abstract class Parameterizer : AbstractIndexBasedDistanceFunction<V>.Parameterizer
        {
            protected double epsilon = 0.0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigEpsilon(config);
            }

            protected void ConfigEpsilon(IParameterization config)
            {
                DoubleParameter epsilonP = new DoubleParameter(EPSILON_ID,
                    new GreaterEqualConstraint<double>(0), 0.001);
                if (config.Grab(epsilonP))
                {
                    epsilon = epsilonP.GetValue();
                }
            }
        }
    }
}
