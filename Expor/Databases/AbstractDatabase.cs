using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Queries.RKnnQueries;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.SimilarityFunctions;
using Socona.Expor.Indexes;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Log;

namespace Socona.Expor.Databases
{

    public abstract class AbstractDatabase: AbstractHierarchicalResult, IDatabase
    {
        /**
         * Parameter to specify the indexes to use.
         * <p>
         * Key: {@code -db.index}
         * </p>
         */
        public static readonly OptionDescription INDEX_ID = OptionDescription.GetOrCreate("db.index", "Database indexes to Add.");

        /**
         * The event manager, collects events and fires them on demand.
         */
        protected readonly DatabaseEventManager eventManager = new DatabaseEventManager();

        /**
         * The relations we manage.
         */
        protected readonly IList<IRelation> relations = new List<IRelation>();

        /**
         * Indexes
         */
        protected readonly IList<IIndex> indexes = new List<IIndex>();

        /**
         * Index factories
         */
        protected readonly ICollection<IIndexFactory> indexFactories = new List<IIndexFactory>();

        /**
         * Constructor.
         */
        public AbstractDatabase()
            : base()
        {
        }


        public virtual void AddIndex(IIndex index)
        {
            this.indexes.Add(index);
            // TODO: actually Add index to the representation used?
            this.AddChildResult(index);
        }


        public IList<IIndex> GetIndexes()
        {
            return new List<IIndex>(this.indexes);
        }


        public void RemoveIndex(IIndex index)
        {
            this.indexes.Remove(index);
            this.Hierarchy.Remove(this, index);
        }


        public SingleObjectBundle GetBundle(IDbId id)
        {
            Debug.Assert(id != null);
            // TODO: ensure that the ID actually exists in the database?
            try
            {
                // Build an object package
                SingleObjectBundle ret = new SingleObjectBundle();
                foreach (IRelation relation in relations)
                {
                    ret.Append(relation.GetDataTypeInformation(), relation[id]);
                }
                return ret;
            }
            catch (ApplicationException e)
            {
                if (id == null)
                {
                    throw new InvalidOperationException("AbstractDatabase.GetPackage(null) called!");
                }
                // throw e upwards.
                throw e;
            }
        }

        public ICollection<IRelation> GetRelations()
        {
            return new List<IRelation>(relations);
        }


        public IRelation GetRelation(ITypeInformation restriction, params Object[] hints)
        {
            // Get first match
            foreach (IRelation relation in relations)
            {
                var datatype = relation.GetDataTypeInformation();
                if (restriction.IsAssignableFromType(datatype)) 
                {
                    return (IRelation)relation;
                }
            }
            List<ITypeInformation> types = new List<ITypeInformation>(relations.Count);
            foreach (IRelation relation in relations)
            {
                types.Add(relation.GetDataTypeInformation());
            }
            throw new NoSupportedDataTypeException(restriction, types);
        }


        public IDistanceQuery GetDistanceQuery(IRelation objQuery, IDistanceFunction distanceFunction, params Object[] hints)
  
        {

            if (distanceFunction == null)
            {
                throw new AbortException("Distance query requested for 'null' distance!");
            }
            return distanceFunction.Instantiate(objQuery);
        }


        public ISimilarityQuery GetSimilarityQuery(IRelation objQuery, ISimilarityFunction similarityFunction, params object[] hints)
        {
            if (similarityFunction == null)
            {
                throw new AbortException("Similarity query requested for 'null' similarity!");
            }
            return similarityFunction.Instantiate(objQuery);
        }


        public IKNNQuery GetKNNQuery(IDistanceQuery distanceQuery, params object[] hints)
        {
            if (distanceQuery == null)
            {
                throw new AbortException("kNN query requested for 'null' distance!");
            }
            for (int i = 0; i < indexes.Count; i++)
            {
                int j = indexes.Count - i - 1;

                IIndex idx = indexes[j];
                if (idx is IKNNIndex)
                {
                    if (GetLogger().IsDebugging)
                    {
                        GetLogger().Debug("Considering index for kNN Query: " + idx);
                    }

                    IKNNIndex knnIndex = (IKNNIndex)idx;
                    IKNNQuery q = knnIndex.GetKNNQuery(distanceQuery, hints);
                    if (q != null)
                    {
                        return q;
                    }
                }


            }

            // Default
            foreach (Object hint in hints)
            {
                if (hint.ToString() == DatabaseQueryHints.HINT_OPTIMIZED_ONLY)
                {
                    return null;
                }
            }
            return QueryUtil.GetLinearScanKNNQuery(distanceQuery);
        }


        public IRangeQuery GetRangeQuery(IDistanceQuery distanceQuery, params object[] hints)
        {
            if (distanceQuery == null)
            {
                throw new AbortException("Range query requested for 'null' distance!");
            }
            for (int i = 0; i < indexes.Count; i++)
            {
                int j = indexes.Count - i - 1;
                IIndex idx = indexes[j];
                if (idx is IRangeIndex)
                {
                    if (GetLogger().IsDebugging)
                    {
                        GetLogger().Debug("Considering index for range query: " + idx);
                    }

                    IRangeIndex rangeIndex = (IRangeIndex)idx;
                    IRangeQuery q = rangeIndex.GetRangeQuery(distanceQuery, hints);
                    if (q != null)
                    {
                        return q;
                    }
                }

            }


            // Default
            foreach (Object hint in hints)
            {
                if (hint.ToString() == DatabaseQueryHints.HINT_OPTIMIZED_ONLY)
                {
                    return null;
                }
            }
            return QueryUtil.GetLinearScanRangeQuery(distanceQuery);
        }


        public IRKNNQuery GetRKNNQuery(IDistanceQuery distanceQuery, params object[] hints)
        {
            if (distanceQuery == null)
            {
                throw new AbortException("RKNN query requested for 'null' distance!");
            }
            for (int i = 0; i < indexes.Count; i++)
            {
                int j = indexes.Count - i - 1;
                IIndex idx = indexes[j];
                if (idx is IRKNNIndex<INumberVector>)
                {
                    if (GetLogger().IsDebugging)
                    {
                        GetLogger().Debug("Considering index for range query: " + idx);
                    }

                    IRKNNIndex<INumberVector> rknnIndex = (IRKNNIndex<INumberVector>)idx;
                    IRKNNQuery q = rknnIndex.GetRKNNQuery(distanceQuery, hints);
                    if (q != null)
                    {
                        return q;
                    }
                }

            }

            Int32? maxk = null;
            // Default
            foreach (Object hint in hints)
            {
                if (hint.ToString() == DatabaseQueryHints.HINT_OPTIMIZED_ONLY)
                {
                    return null;
                }
                if (hint is Int32)
                {
                    maxk = (Int32)hint;
                }
            }
            IKNNQuery knnQuery = GetKNNQuery(distanceQuery, DatabaseQueryHints.HINT_BULK, maxk);

            return new LinearScanRKNNQuery<INumberVector>(distanceQuery, knnQuery, (int)maxk);
        }

        //
        //public void AddDataStoreListener(DataStoreListener l) {
        //  eventManager.AddListener(l);
        //}

        //
        //public void removeDataStoreListener(DataStoreListener l) {
        //  eventManager.removeListener(l);
        //}

        //
        //public void accumulateDataStoreEvents() {
        //  eventManager.accumulateDataStoreEvents();
        //}

        //
        //public void flushDataStoreEvents() {
        //  eventManager.flushDataStoreEvents();
        //}


        public override String LongName
        {
            get
            {
                return "Database";
            }
        }


        public override String ShortName
        {
            get
            {
                return "database";
            }
        }

        abstract protected Logging GetLogger();

        public abstract void Initialize();

        IList<IRelation> IDatabase.GetRelations()
        {
            return relations;
        }

        public MultipleObjectsBundle GetBundles(IDbIds id)
        {
            throw new NotImplementedException();
        }

        ICollection<IIndex> IDatabase.GetIndexes()
        {
            return new List<IIndex>(this.indexes);
        }

        public void FlushDataStoreEvents()
        {
            throw new NotImplementedException();
        }

        protected void OnObjectsInserted(IArrayDbIds dbids)
        {
            if (ObjectsInserted != null)
            {
                ObjectsInserted(this, new ObjectsChangeEventArgs() { DbIds = dbids });
            }
        }


        public event EventHandler<ObjectsChangeEventArgs> ObjectsInserted;
    }
}
