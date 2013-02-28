using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Generic;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log.Progress;

namespace Socona.Expor.Indexes.Preprocessed.SubspaceProj
{

    [Title("Local PCA Preprocessor")]
    [Description("Materializes the local PCA and the locally weighted matrix of objects of a database.")]
    public abstract class AbstractSubspaceProjectionIndex<NV, P> :
        AbstractPreprocessorIndex< P>, ISubspaceProjectionIndex
        where NV : INumberVector
        where P : IProjectionResult
    {
        /**
         * Contains the value of parameter epsilon;
         */
        protected IDistanceValue epsilon;

        /**
         * The distance function for the variance analysis.
         */
        protected IDistanceFunction rangeQueryDistanceFunction;

        /**
         * Holds the value of parameter minpts.
         */
        protected int minpts;

        /**
         * Constructor.
         *
         * @param relation Relation
         * @param epsilon Maximum Epsilon
         * @param rangeQueryDistanceFunction range query
         * @param minpts Minpts
         */
        public AbstractSubspaceProjectionIndex(IRelation relation, IDistanceValue epsilon,
            IDistanceFunction rangeQueryDistanceFunction, int minpts) :
            base(relation)
        {
            this.epsilon = epsilon;
            this.rangeQueryDistanceFunction = rangeQueryDistanceFunction;
            this.minpts = minpts;
        }

        /**
         * Preprocessing step.
         */
        protected void Preprocess()
        {
            if (relation == null || relation.Count <= 0)
            {
                throw new ArgumentException(ExceptionMessages.DATABASE_EMPTY);
            }
            if (storage != null)
            {
                // Preprocessor was already run.
                return;
            }
            storage = DataStoreUtil.MakeStorage<P>(relation.GetDbIds(),
                DataStoreHints.Hot | DataStoreHints.Temp, typeof(IProjectionResult));

            long start = DateTime.Now.ToFileTime();
            IRangeQuery rangeQuery = QueryUtil.GetRangeQuery(relation, rangeQueryDistanceFunction);

            FiniteProgress progress =
                GetLogger().IsVerbose ?
                new FiniteProgress(this.GetType().Name, relation.Count, GetLogger()) : null;
            //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation.GetDbIds())
            {
                IDistanceDbIdList neighbors = rangeQuery.GetRangeForDbId(iditer, epsilon);

                P pres;
                if (neighbors.Count >= minpts)
                {
                    pres = computeProjection(iditer, neighbors, relation);
                }
                else
                {
                    IDistanceDbIdPair firstQR = neighbors.ElementAt(0);
                    neighbors = new GenericDistanceDbIdList();
                    neighbors.Add(firstQR);
                    pres = computeProjection(iditer, neighbors, relation);
                }
                storage[iditer] = pres;

                if (progress != null)
                {
                    progress.IncrementProcessed(GetLogger());
                }
            }
            if (progress != null)
            {
                progress.EnsureCompleted(GetLogger());
            }

            long end = DateTime.Now.ToFileTime();
            // TODOin re-Add timing code!
            if (true)
            {
                long elapsedTime = end - start;
                GetLogger().Verbose(this.GetType().Name + " runtimein " + elapsedTime + " milliseconds.");
            }
        }


        public IProjectionResult GetLocalProjection(IDbIdRef objid)
        {
            if (storage == null)
            {
                Preprocess();
            }
            return storage[(objid)];
        }

        /**
         * This method implements the type of variance analysis to be computed for a
         * given point.
         * <p/>
         * Example1in for 4C, this method should implement a PCA for the given point.
         * Example2in for PreDeCon, this method should implement a simple axis-parallel
         * variance analysis.
         * 
         * @param id the given point
         * @param neighbors the neighbors as query results of the given point
         * @param relation the database for which the preprocessing is performed
         * 
         * @return local subspace projection
         */
        protected abstract P computeProjection(IDbIdRef id, IDistanceDbIdList neighbors, IRelation relation);

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses AbstractSubspaceProjectionIndex oneway - - 芦create禄
         */
        public abstract class Factory : ISubspaceProjectionIndexFactory, IParameterizable
        {
            /**
             * Contains the value of parameter epsilon;
             */
            protected IDistanceValue epsilon;

            /**
             * The distance function for the variance analysis.
             */
            protected IDistanceFunction rangeQueryDistanceFunction;

            /**
             * Holds the value of parameter minpts.
             */
            protected int minpts;

            /**
             * Constructor.
             * 
             * @param epsilon
             * @param rangeQueryDistanceFunction
             * @param minpts
             */
            public Factory(IDistanceValue epsilon, IDistanceFunction rangeQueryDistanceFunction, int minpts) :
                base()
            {
                this.epsilon = epsilon;
                this.rangeQueryDistanceFunction = rangeQueryDistanceFunction;
                this.minpts = minpts;
            }


            public abstract IIndex Instantiate(IRelation relation);


            public ITypeInformation GetInputTypeRestriction()
            {
                return TypeUtil.NUMBER_VECTOR_FIELD;
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
                 * Contains the value of parameter epsilon;
                 */
                protected IDistanceValue epsilon = null;

                /**
                 * The distance function for the variance analysis.
                 */
                protected IDistanceFunction rangeQueryDistanceFunction = null;

                /**
                 * Holds the value of parameter minpts.
                 */
                protected int minpts = 0;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    ConfigRangeQueryDistanceFunction(config);
                    ConfigEpsilon(config, rangeQueryDistanceFunction);
                    ConfigMinPts(config);
                }

                protected void ConfigRangeQueryDistanceFunction(IParameterization config)
                {
                    ObjectParameter<IDistanceFunction> rangeQueryDistanceP =
                        new ObjectParameter<IDistanceFunction>(AbstractProjectedDBSCAN<NV>.INNER_DISTANCE_FUNCTION_ID,
                            typeof(IDistanceFunction), typeof(EuclideanDistanceFunction));
                    if (config.Grab(rangeQueryDistanceP))
                    {
                        rangeQueryDistanceFunction = rangeQueryDistanceP.InstantiateClass(config);
                    }
                }

                protected void ConfigEpsilon(IParameterization config, IDistanceFunction rangeQueryDistanceFunction)
                {
                    IDistanceValue distanceParser =
                        rangeQueryDistanceFunction != null ? rangeQueryDistanceFunction.DistanceFactory : null;
                    DistanceParameter epsilonP =
                        new DistanceParameter(AbstractProjectedDBSCAN<NV>.EPSILON_ID, distanceParser);
                    // parameter epsilon
                    if (config.Grab(epsilonP))
                    {
                        epsilon = epsilonP.GetValue();
                    }
                }

                protected void ConfigMinPts(IParameterization config)
                {
                    IntParameter minptsP = new IntParameter(AbstractProjectedDBSCAN<NV>.MINPTS_ID, new GreaterConstraint<int>(0));
                    if (config.Grab(minptsP))
                    {
                        minpts = minptsP.GetValue();
                    }
                }
            }
        }
    }
}
