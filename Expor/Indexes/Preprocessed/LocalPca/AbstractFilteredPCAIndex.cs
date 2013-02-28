using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log.Progress;

namespace Socona.Expor.Indexes.Preprocessed.LocalPca
{

    [Title("Local PCA Preprocessor")]
    [Description("Materializes the local PCA and the locally weighted matrix of objects of a database.")]
    public abstract class AbstractFilteredPCAIndex<NV> :
        AbstractPreprocessorIndex<PCAFilteredResult>, IFilteredLocalPCAIndex
        where NV : INumberVector
    {
        /**
         * PCA utility object.
         */
        protected PCAFilteredRunner<NV> pca;

        /**
         * Constructor.
         * 
         * @param relation Relation to use
         * @param pca PCA runner to use
         */
        public AbstractFilteredPCAIndex(IRelation relation, PCAFilteredRunner<NV> pca)
            : base(relation)
        {
            this.pca = pca;
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

            // Note: this is required for ERiC to work properly, otherwise the data is
            // recomputed for the partitions!
            if (storage != null)
            {
                return;
            }

            storage = DataStoreUtil.MakeStorage<PCAFilteredResult>(relation.GetDbIds(),
                DataStoreHints.Hot | DataStoreHints.Temp, typeof(PCAFilteredResult));

            long start = DateTime.Now.ToFileTime();
            FiniteProgress progress = GetLogger().IsVerbose ? new FiniteProgress("Performing local PCA", relation.Count, GetLogger()) : null;

            // TODO: use a bulk operation?
            //for (DBIDIter iditer = relation.iterDBIDs(); iditer.valid(); iditer.advance())
            foreach (var iditer in relation.GetDbIds())
            {
                IDbId id = iditer.DbId;
                IEnumerable<IDistanceDbIdPair> objects = ObjectsForPCA(id);

                PCAFilteredResult pcares = (PCAFilteredResult)pca.ProcessQueryResult(objects.ToList(), relation);

                storage[id] = pcares;

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
            if (GetLogger().IsVerbose)
            {
                long elapsedTime = end - start;
                GetLogger().Verbose(this.GetType().Name + " runtime: " + elapsedTime + " milliseconds.");
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
         * Returns the objects to be considered within the PCA for the specified query
         * object.
         * 
         * @param id the id of the query object for which a PCA should be performed
         * @return the list of the objects (i.e. the ids and the distances to the
         *         query object) to be considered within the PCA
         */
        protected abstract IEnumerable<IDistanceDbIdPair> ObjectsForPCA(IDbId id);

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses AbstractFilteredPCAIndex oneway - - 芦create禄
         */
        public abstract class Factory : IFilteredLocalPCAIndexFactory, IParameterizable
        {
            /**
             * Parameter to specify the distance function used for running PCA.
             * 
             * Key: {@code -localpca.distancefunction}
             */
            public static OptionDescription PCA_DISTANCE_ID = OptionDescription.GetOrCreate(
                "localpca.distancefunction", "The distance function used to select objects for running PCA.");

            /**
             * Holds the instance of the distance function specified by
             * {@link #PCA_DISTANCE_ID}.
             */
            protected IDistanceFunction pcaDistanceFunction;

            /**
             * PCA utility object.
             */
            protected PCAFilteredRunner<NV> pca;

            /**
             * Constructor.
             * 
             * @param pcaDistanceFunction distance Function
             * @param pca PCA runner
             */
            public Factory(IDistanceFunction pcaDistanceFunction, PCAFilteredRunner<NV> pca) :
                base()
            {
                this.pcaDistanceFunction = pcaDistanceFunction;
                this.pca = pca;
            }


            public abstract IIndex Instantiate(IRelation relation);

            public ITypeInformation GetInputTypeRestriction()
            {
                return pcaDistanceFunction.GetInputTypeRestriction();
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
                 * Holds the instance of the distance function specified by
                 * {@link #PCA_DISTANCE_ID}.
                 */
                protected IDistanceFunction pcaDistanceFunction;

                /**
                 * PCA utility object.
                 */
                protected PCAFilteredRunner<NV> pca;

                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    ObjectParameter<IDistanceFunction> pcaDistanceFunctionP = new ObjectParameter<IDistanceFunction>(
                        PCA_DISTANCE_ID, typeof(IDistanceFunction), typeof(EuclideanDistanceFunction));

                    if (config.Grab(pcaDistanceFunctionP))
                    {
                        pcaDistanceFunction = pcaDistanceFunctionP.InstantiateClass(config);
                    }

                    Type cls = ClassGenericsUtil.UglyCastIntoSubclass(typeof(PCAFilteredRunner<NV>));
                    pca = config.TryInstantiate<PCAFilteredRunner<NV>>(cls);
                }
            }
        }
    }
}
