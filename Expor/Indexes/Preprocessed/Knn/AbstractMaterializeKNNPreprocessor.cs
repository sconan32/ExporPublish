using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Preprocessed.Knn
{

    /**
     * Abstract base class for KNN Preprocessors.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type
     * @param <D> Distance type
     * @param <T> Result type
     */
    public abstract class AbstractMaterializeKNNPreprocessor<O> : AbstractPreprocessorIndex<IKNNList>, IKNNIndex
        where O : INumberVector
    {
        /**
         * The query k value.
         */
        protected int k;

        /**
         * The distance function to be used.
         */
        protected IDistanceFunction distanceFunction;

        /**
         * The distance query we used.
         */
        protected IDistanceQuery distanceQuery;

        /**
         * Constructor.
         * 
         * @param relation Relation
         * @param distanceFunction Distance function
         * @param k k
         */
        public AbstractMaterializeKNNPreprocessor(IRelation relation, IDistanceFunction distanceFunction, int k) :
            base(relation)
        {
            this.k = k;
            this.distanceFunction = distanceFunction;
            this.distanceQuery = distanceFunction.Instantiate(relation);
        }

        /**
         * Get the distance factory.
         * 
         * @return distance factory
         */
        public IDistanceValue GetDistanceFactory()
        {
            return distanceFunction.DistanceFactory;
        }

        /**
         * The distance query we used.
         * 
         * @return Distance query
         */
        public IDistanceQuery GetDistanceQuery()
        {
            return distanceQuery;
        }

        /**
         * Get the value of 'k' supported by this preprocessor.
         * 
         * @return k
         */
        public int GetK()
        {
            return k;
        }

        /**
         * Perform the preprocessing step.
         */
        protected abstract void Preprocess();

        /**
         * Get the k nearest neighbors.
         * 
         * @param id Object ID
         * @return Neighbors
         */
        public IKNNList Get(IDbIdRef id)
        {
            if (storage == null)
            {
                if (GetLogger().IsDebugging)
                {
                    GetLogger().Debug("Running kNN preprocessor: " + this.GetType());
                }
                Preprocess();
            }
            return storage[(id)];
        }

        /**
         * Create the default storage.
         */
        protected void CreateStorage()
        {
            IWritableDataStore<IKNNList> s = DataStoreUtil.MakeStorage<IKNNList>(
                relation.GetDbIds(), DataStoreHints.Hot, typeof(IKNNList));
            storage = s;
        }


        public void Initialize()
        {
            if (storage == null)
            {
                if (relation.Count > 0)
                {
                    Preprocess();
                }
            }
            else
            {
                throw new InvalidOperationException("Preprocessor already ran.");
            }
        }


        public IKNNQuery GetKNNQuery(IDistanceQuery distQ, params Object[] hints)
        {
            if (!this.distanceFunction.Equals(distQ.DistanceFunction))
            {
                return null;
            }
            // k max supported?
            foreach (Object hint in hints)
            {
                if (hint is int)
                {
                    if (((int)hint) > k)
                    {
                        return null;
                    }
                    break;
                }
            }
            // To make compilers happy:
            AbstractMaterializeKNNPreprocessor<O> tmp = this;
            return new PreprocessorKNNQuery<O>(relation, (AbstractMaterializeKNNPreprocessor<O>)tmp);
        }

        /**
         * The parameterizable factory.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.landmark
         * @apiviz.stereotype factory
         * @apiviz.uses AbstractMaterializeKNNPreprocessor oneway - - 芦create禄
         * 
         * @param <O> The object type
         * @param <D> The distance type
         */
        public abstract class Factory : IIndexFactory
        {
            /**
             * Parameter to specify the number of nearest neighbors of an object to be
             * materialized. must be an integer greater than 1.
             * <p>
             * Key: {@code -materialize.k}
             * </p>
             */
            public static OptionDescription K_ID = new OptionDescription("materialize.k",
                "The number of nearest neighbors of an object to be materialized.");

            /**
             * Parameter to indicate the distance function to be used to ascertain the
             * nearest neighbors.
             * <p/>
             * <p>
             * Default value: {@link EuclideanDistanceFunction}
             * </p>
             * <p>
             * Key: {@code materialize.distance}
             * </p>
             */
            public static OptionDescription DISTANCE_FUNCTION_ID = new OptionDescription("materialize.distance",
                "the distance function to materialize the nearest neighbors");

            /**
             * Holds the value of {@link #K_ID}.
             */
            protected int k;

            /**
             * Hold the distance function to be used.
             */
            protected IDistanceFunction distanceFunction;

            /**
             * Index factory.
             * 
             * @param k k parameter
             * @param distanceFunction distance function
             */
            public Factory(int k, IDistanceFunction distanceFunction)
            {

                this.k = k;
                this.distanceFunction = distanceFunction;
            }


            public abstract IIndex Instantiate(IRelation relation);

            /**
             * Get the distance function.
             * 
             * @return Distance function
             */
            // TODO: hide this?
            public IDistanceFunction GetDistanceFunction()
            {
                return distanceFunction;
            }

            /**
             * Get the distance factory.
             * 
             * @return Distance factory
             */
            public IDistanceValue GetDistanceFactory()
            {
                return distanceFunction.DistanceFactory;
            }


            public ITypeInformation GetInputTypeRestriction()
            {
                return distanceFunction.GetInputTypeRestriction();
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
                /**
                 * Holds the value of {@link #K_ID}.
                 */
                protected int k;

                /**
                 * Hold the distance function to be used.
                 */
                protected IDistanceFunction distanceFunction;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    // number of neighbors
                    IntParameter kP = new IntParameter(K_ID);
                    kP.AddConstraint(CommonConstraints.GREATER_THAN_ONE_INT);
                    if (config.Grab(kP))
                    {
                        k = kP.GetValue();
                    }

                    // distance function
                    ObjectParameter<IDistanceFunction> distanceFunctionP =
                        new ObjectParameter<IDistanceFunction>(DISTANCE_FUNCTION_ID,
                            typeof(IDistanceFunction), typeof(EuclideanDistanceFunction));
                    if (config.Grab(distanceFunctionP))
                    {
                        distanceFunction = distanceFunctionP.InstantiateClass(config);
                    }
                }


                protected override abstract object MakeInstance();
            }
        }
    }
}
