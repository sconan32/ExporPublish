using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Log.Progress;

namespace Socona.Expor.Indexes.Preprocessed.Snn
{

    [Title("Shared nearest neighbor Preprocessor")]
    [Description("Computes the k nearest neighbors of objects of a certain database.")]
    public class SharedNearestNeighborPreprocessor<O> : AbstractPreprocessorIndex<IArrayDbIds>, ISharedNearestNeighborIndex<O>
        where O : ISpatialComparable
    {
        /**
         * Get a logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(SharedNearestNeighborPreprocessor<O>));

        /**
         * Holds the number of nearest neighbors to be used.
         */
        protected int numberOfNeighbors;

        /**
         * Hold the distance function to be used.
         */
        protected IDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param relation Database to use
         * @param numberOfNeighbors Number of neighbors
         * @param distanceFunction Distance function
         */
        public SharedNearestNeighborPreprocessor(IRelation relation,
            int numberOfNeighbors, IDistanceFunction distanceFunction) :
            base(relation)
        {
            this.numberOfNeighbors = numberOfNeighbors;
            this.distanceFunction = distanceFunction;
        }

        /**
         * Preprocessing step.
         */
        protected void preprocess()
        {
            if (GetLogger().IsVerbose)
            {
                GetLogger().Verbose("Assigning nearest neighbor lists to database objects");
            }
            storage = DataStoreUtil.MakeStorage<IArrayDbIds>(relation.GetDbIds(),
                DataStoreHints.Hot | DataStoreHints.Temp, typeof(IArrayDbIds));
            IKNNQuery knnquery = QueryUtil.GetKNNQuery(relation, distanceFunction, numberOfNeighbors);

            FiniteProgress progress = GetLogger().IsVerbose ? new FiniteProgress("assigning nearest neighbor lists", relation.Count, GetLogger()) : null;
            // for(DBIDIter iditer = relation.iterIDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation.GetDbIds())
            {
                IArrayModifiableDbIds neighbors = DbIdUtil.NewArray(numberOfNeighbors);
                IKNNList kNN = knnquery.GetKNNForDbId(iditer, numberOfNeighbors);
                foreach (IDistanceResultPair pair in (IEnumerable<IDistanceDbIdPair>)kNN)
                {
                    // if(!id.equals(nid)) {
                    neighbors.Add(pair);
                    // }
                    // Size limitation to exactly numberOfNeighbors
                    if (neighbors.Count >= numberOfNeighbors)
                    {
                        break;
                    }
                }
                neighbors.Sort();
                storage[iditer] = neighbors;
                if (progress != null)
                {
                    progress.IncrementProcessed(GetLogger());
                }
            }
            if (progress != null)
            {
                progress.EnsureCompleted(GetLogger());
            }
        }


        public IArrayDbIds GetNearestNeighborSet(IDbIdRef objid)
        {
            if (storage == null)
            {
                preprocess();
            }
            return (IArrayDbIds)storage[(objid)];
        }


        protected override Logging GetLogger()
        {
            return logger;
        }


        public override String LongName
        {
            get
            {
                return "SNN id index";
            }
        }

        public override String ShortName
        {
            get
            {
                return "SNN-index";
            }
        }

        /**
         * Get the number of neighbors
         * 
         * @return NN size
         */

        public int GetNumberOfNeighbors()
        {
            return numberOfNeighbors;
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses SharedNearestNeighborPreprocessor oneway - - 芦create禄
         */
        public class Factory : ISharedNearestNeighborIndexFactory<O>, IParameterizable
        {
            /**
             * Parameter to indicate the number of neighbors to be taken into account
             * for the shared-nearest-neighbor similarity.
             * <p/>
             * <p>
             * Default value: 1
             * </p>
             * <p>
             * Key: {@code sharedNearestNeighbors}
             * </p>
             */
            public static OptionDescription NUMBER_OF_NEIGHBORS_ID = OptionDescription.GetOrCreate("sharedNearestNeighbors", "number of nearest neighbors to consider (at least 1)");

            /**
             * Parameter to indicate the distance function to be used to ascertain the
             * nearest neighbors.
             * <p/>
             * <p>
             * Default value: {@link EuclideanDistanceFunction}
             * </p>
             * <p>
             * Key: {@code SNNDistanceFunction}
             * </p>
             */
            public static OptionDescription DISTANCE_FUNCTION_ID = OptionDescription.GetOrCreate("SNNDistanceFunction", "the distance function to asses the nearest neighbors");

            /**
             * Holds the number of nearest neighbors to be used.
             */
            protected int numberOfNeighbors;

            /**
             * Hold the distance function to be used.
             */
            protected IDistanceFunction distanceFunction;

            /**
             * Constructor.
             * 
             * @param numberOfNeighbors Number of neighbors
             * @param distanceFunction Distance function
             */
            public Factory(int numberOfNeighbors, IDistanceFunction distanceFunction) :
                base()
            {
                this.numberOfNeighbors = numberOfNeighbors;
                this.distanceFunction = distanceFunction;
            }


            public IIndex Instantiate(IRelation relation)
            {
                return new SharedNearestNeighborPreprocessor<O>(relation, numberOfNeighbors, distanceFunction);
            }

            /**
             * Get the number of neighbors
             * 
             * @return NN size
             */

            public int GetNumberOfNeighbors()
            {
                return numberOfNeighbors;
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
            public class Parameterizer : AbstractParameterizer
            {
                /**
                 * Holds the number of nearest neighbors to be used.
                 */
                protected int numberOfNeighbors;

                /**
                 * Hold the distance function to be used.
                 */
                protected IDistanceFunction distanceFunction;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    IntParameter numberOfNeighborsP = new IntParameter(NUMBER_OF_NEIGHBORS_ID, new GreaterEqualConstraint<int>(1));
                    if (config.Grab(numberOfNeighborsP))
                    {
                        numberOfNeighbors = numberOfNeighborsP.GetValue();
                    }

                    ObjectParameter<IDistanceFunction> distanceFunctionP =
                        new ObjectParameter<IDistanceFunction>(DISTANCE_FUNCTION_ID, typeof(IDistanceFunction), typeof(EuclideanDistanceFunction));
                    if (config.Grab(distanceFunctionP))
                    {
                        distanceFunction = distanceFunctionP.InstantiateClass(config);
                    }
                }


                protected override object MakeInstance()
                {
                    return new Factory(numberOfNeighbors, distanceFunction);
                }
            }
        }
    }
}
