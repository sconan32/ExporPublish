using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed.Knn
{

    /**
     * A preprocessor for annotation of the k nearest neighbors (and their
     * distances) to each database object.
     * 
     * Used for example by {@link de.lmu.ifi.dbs.elki.algorithm.outlier.lof.LOF}.
     * 
     * @author Erich Schubert
     */
    [Title("Materialize kNN Neighborhood preprocessor")]
    [Description("Materializes the k nearest neighbors of objects of a database.")]
    public class MaterializeKNNPreprocessor : AbstractMaterializeKNNPreprocessor<INumberVector>, IDynamicIndex
    {
        /**
         * Logger to use.
         */
        private static Logging LOG = Logging.GetLogger(typeof(MaterializeKNNPreprocessor));

        /**
         * Flag to use bulk operations.
         * 
         * TODO: right now, bulk is not that good - so don't use
         */
        private static bool usebulk = false;

        /**
         * KNNQuery instance to use.
         */
        protected IKNNQuery knnQuery;

        /**
         * Holds the listener.
         */
        //protected  EventListenerList listenerList = new EventListenerList();

        /**
         * Constructor with preprocessing step.
         * 
         * @param relation Relation to preprocess
         * @param distanceFunction the distance function to use
         * @param k query k
         */
        public MaterializeKNNPreprocessor(IRelation relation, IDistanceFunction distanceFunction, int k) :
            base(relation, distanceFunction, k)
        {
            this.knnQuery = relation.GetDatabase().GetKNNQuery(distanceQuery, k, DatabaseQueryHints.HINT_BULK, DatabaseQueryHints.HINT_HEAVY_USE, DatabaseQueryHints.HINT_NO_CACHE);
        }

        /**
         * The actual preprocessing step.
         */

        protected override void Preprocess()
        {
            CreateStorage();

            IArrayDbIds ids = DbIdUtil.EnsureArray(relation.GetDbIds());

            //if (LOG.isStatistics()) {
            //  LOG.statistics(new LongStatistic(this.getClass().getName() + ".k", k));
            //}
            //Duration duration = LOG.isStatistics() ? LOG.newDuration(this.getClass().getName() + ".precomputation-time") : null;
            //if (duration != null) {
            //  duration.begin();
            //}
            //FiniteProgress progress = getLogger().isVerbose() ? new FiniteProgress("Materializing k nearest neighbors (k=" + k + ")", ids.size(), getLogger()) : null;
            // Try bulk
            IList<IKNNList> kNNList = null;
            if (usebulk)
            {
                kNNList = knnQuery.GetKNNForBulkDbIds(ids, k);
                if (kNNList != null)
                {
                    int i = 0;
                    //for (DbIdIter id = ids.iter(); id.valid(); id.advance(), i++) {
                    foreach (var id in ids)
                    {
                        storage[id] = kNNList[(i)];
                        //if (progress != null) {
                        //  progress.incrementProcessed(getLogger());
                        //}
                    }
                }
            }
            else
            {
                // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var id in ids)
                {
                    IKNNList knn = knnQuery.GetKNNForDbId(id, k);
                    storage[id] = knn;
                    //  if (progress != null) {
                    //    progress.incrementProcessed(getLogger());
                    //  }
                }
            }
            //if (progress != null) {
            //  progress.ensureCompleted(getLogger());
            //}
            //if (duration != null) {
            //  duration.end();
            //  LOG.statistics(duration);
            //}
        }


        public void Insert(IDbIdRef id)
        {
            ObjectsInserted(DbIdUtil.Deref(id));
        }


        public override void InsertAll(IDbIds ids)
        {
            if (storage == null && ids.Count > 0)
            {
                Preprocess();
            }
            else
            {
                ObjectsInserted(ids);
            }
        }


        public bool Delete(IDbIdRef id)
        {
            ObjectsRemoved(DbIdUtil.Deref(id));
            return true;
        }


        public override void DeleteAll(IDbIds ids)
        {
            ObjectsRemoved(ids);
        }

        /**
         * Called after new objects have been inserted, updates the materialized
         * neighborhood.
         * 
         * @param ids the ids of the newly inserted objects
         */
        protected void ObjectsInserted(IDbIds ids)
        {
            //StepProgress stepprog = getLogger().isVerbose() ? new StepProgress(3) : null;

            IArrayDbIds aids = DbIdUtil.EnsureArray(ids);
            // materialize the new kNNs
            //if (stepprog != null) {
            //  stepprog.beginStep(1, "New insertions ocurred, materialize their new kNNs.", getLogger());
            //}
            // Bulk-query kNNs
            IList<IKNNList> kNNList = knnQuery.GetKNNForBulkDbIds(aids, k);
            // Store in storage
            IEnumerator<IDbId> iter = aids.GetEnumerator();
            for (int i = 0; i < aids.Count; i++, iter.MoveNext())
            {

                storage[iter.Current] = kNNList[i];
            }

            // update the affected kNNs
            //if (stepprog != null) {
            //  stepprog.beginStep(2, "New insertions ocurred, update the affected kNNs.", getLogger());
            //}
            IArrayDbIds rkNN_ids = UpdateKNNsAfterInsertion(ids);

            // inform listener
            //if (stepprog != null) {
            //  stepprog.beginStep(3, "New insertions ocurred, inform listeners.", getLogger());
            //}
            FireKNNsInserted(ids, rkNN_ids);

            //if (stepprog != null) {
            //  stepprog.setCompleted(getLogger());
            //}
        }

        /**
         * Updates the kNNs of the RkNNs of the specified ids.
         * 
         * @param ids the ids of newly inserted objects causing a change of
         *        materialized kNNs
         * @return the RkNNs of the specified ids, i.e. the kNNs which have been
         *         updated
         */
        private IArrayDbIds UpdateKNNsAfterInsertion(IDbIds ids)
        {
            IArrayModifiableDbIds rkNN_ids = DbIdUtil.NewArray();
            IDbIds oldids = DbIdUtil.Difference(relation.GetDbIds(), ids);
            //for (DbIdIter iter = oldids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in oldids)
            {
                IKNNList kNNs = storage[(iter)];
                IDistanceValue knnDist = kNNs[(kNNs.Count - 1)].Distance;
                // look for new kNNs
                IKNNHeap heap = null;
                //for (DbIdIter iter2 = ids.iter(); iter2.valid(); iter2.advance()) {
                foreach (var iter2 in ids)
                {
                    IDistanceValue dist = distanceQuery.Distance(iter, iter2);
                    if (dist.CompareTo(knnDist) <= 0)
                    {
                        if (heap == null)
                        {
                            heap = DbIdUtil.NewHeap(kNNs);
                        }
                        heap.Insert(dist, iter2);
                    }
                }
                if (heap != null)
                {
                    kNNs = heap.ToKNNList();
                    storage[iter] = kNNs;
                    rkNN_ids.Add(iter);
                }
            }
            return rkNN_ids;
        }

        /**
         * Updates the kNNs of the RkNNs of the specified ids.
         * 
         * @param ids the ids of deleted objects causing a change of materialized kNNs
         * @return the RkNNs of the specified ids, i.e. the kNNs which have been
         *         updated
         */
        private IArrayDbIds UpdateKNNsAfterDeletion(IDbIds ids)
        {
            ISetDbIds idsSet = DbIdUtil.EnsureSet(ids);
            IArrayModifiableDbIds rkNN_ids = DbIdUtil.NewArray();
            // for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation)
            {
                IKNNList kNNs = storage[(iditer)];
                // for (DbIdIter it = kNNs.iter(); it.valid(); it.advance()) {
                foreach (var it in (IEnumerable<IDbIds>)kNNs)
                {
                    if (idsSet.Contains(it))
                    {
                        rkNN_ids.Add(iditer);
                        break;
                    }
                }
            }

            // update the kNNs of the RkNNs
            IList<IKNNList> kNNList = knnQuery.GetKNNForBulkDbIds(rkNN_ids, k);
            // DbIdIter iter = rkNN_ids.iter();
            IEnumerator<IDbId> iter = rkNN_ids.GetEnumerator();
            for (int i = 0; i < rkNN_ids.Count; i++, iter.MoveNext())
            {
                storage[iter.Current] = kNNList[i];
            }

            return rkNN_ids;
        }

        /**
         * Called after objects have been removed, updates the materialized
         * neighborhood.
         * 
         * @param ids the ids of the removed objects
         */
        protected void ObjectsRemoved(IDbIds ids)
        {
            // StepProgress stepprog = getLogger().isVerbose() ? new StepProgress(3) : null;

            // delete the materialized (old) kNNs
            //if (stepprog != null) {
            //  stepprog.beginStep(1, "New deletions ocurred, remove their materialized kNNs.", getLogger());
            //}
            //  for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                storage.Delete(iter);
            }

            // update the affected kNNs
            //if (stepprog != null) {
            //  stepprog.beginStep(2, "New deletions ocurred, update the affected kNNs.", getLogger());
            //}
            IArrayDbIds rkNN_ids = UpdateKNNsAfterDeletion(ids);

            // inform listener
            //if (stepprog != null) {
            //  stepprog.beginStep(3, "New deletions ocurred, inform listeners.", getLogger());
            //}
            FireKNNsRemoved(ids, rkNN_ids);

            //if (stepprog != null) {
            //  stepprog.ensureCompleted(getLogger());
            //}
        }

        /**
         * Informs all registered KNNListener that new kNNs have been inserted and as
         * a result some kNNs have been changed.
         * 
         * @param insertions the ids of the newly inserted kNNs
         * @param updates the ids of kNNs which have been changed due to the
         *        insertions
         * @see KNNListener
         */
        protected void FireKNNsInserted(IDbIds insertions, IDbIds updates)
        {
            //KNNChangeEvent e = new KNNChangeEvent(this, KNNChangeEvent.Type.INSERT, insertions, updates);
            //Object[] listeners = listenerList.getListenerList();
            //for (int i = listeners.length - 2; i >= 0; i -= 2) {
            //  if (listeners[i] == KNNListener.class) {
            //    ((KNNListener) listeners[i + 1]).kNNsChanged(e);
            //  }
            //}
        }

        /**
         * Informs all registered KNNListener that existing kNNs have been removed and
         * as a result some kNNs have been changed.
         * 
         * @param removals the ids of the removed kNNs
         * @param updates the ids of kNNs which have been changed due to the removals
         * @see KNNListener
         */
        protected void FireKNNsRemoved(IDbIds removals, IDbIds updates)
        {
            //KNNChangeEvent e = new KNNChangeEvent(this, KNNChangeEvent.Type.DELETE, removals, updates);
            //Object[] listeners = listenerList.getListenerList();
            //for (int i = listeners.length - 2; i >= 0; i -= 2) {
            //  if (listeners[i] == KNNListener.class) {
            //    ((KNNListener) listeners[i + 1]).kNNsChanged(e);
            //  }
            //}
        }

        /**
         * Adds a {@link KNNListener} which will be invoked when the kNNs of objects
         * are changing.
         * 
         * @param l the listener to add
         * @see #removeKNNListener
         * @see KNNListener
         */
        //public void AddKNNListener(KNNListener l) {
        //  listenerList.add(KNNListener.class, l);
        //}

        /**
         * Removes a {@link KNNListener} previously added with {@link #addKNNListener}
         * .
         * 
         * @param l the listener to remove
         * @see #addKNNListener
         * @see KNNListener
         */
        //public void removeKNNListener(KNNListener l) {
        //  listenerList.remove(KNNListener.class, l);
        //}


        public override String LongName
        {
            get { return "kNN Preprocessor"; }
        }


        public override String ShortName
        {
            get { return "knn preprocessor"; }
        }


        public void LogStatistics()
        {
            // TODO: can we log some sensible statistics?
        }


        protected override Logging GetLogger()
        {
            return LOG;
        }

        /**
         * The parameterizable factory.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.landmark
         * @apiviz.stereotype factory
         * @apiviz.uses MaterializeKNNPreprocessor oneway - - 芦create禄
         * 
         * @param <O> The object type
         * @param <D> The distance type
         */
        public new class Factory : AbstractMaterializeKNNPreprocessor<INumberVector>.Factory
        {
            /**
             * Index factory.
             * 
             * @param k k parameter
             * @param distanceFunction distance function
             */
            public Factory(int k, IDistanceFunction distanceFunction) :
                base(k, distanceFunction)
            {
            }


            public override IIndex Instantiate(IRelation relation)
            {
                MaterializeKNNPreprocessor instance = new MaterializeKNNPreprocessor(relation, distanceFunction, k);
                return instance;
            }

            /**
             * Parameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public new class Parameterizer : AbstractMaterializeKNNPreprocessor<INumberVector>.Factory.Parameterizer
            {

                protected override object MakeInstance()
                {
                    return new Factory(k, distanceFunction);
                }
            }
        }
    }
}
