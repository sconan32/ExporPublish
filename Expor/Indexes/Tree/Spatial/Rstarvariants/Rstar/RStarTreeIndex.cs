using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Queries;
using Socona.Expor.Persistent;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Rstar
{
    /**
     * The common use of the rstar tree: indexing number vectors.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type
     */
    public class RStarTreeIndex<O> : RStarTree, IRangeIndex, IKNNIndex
    where O : ISpatialComparable
    {
        /**
         * The appropriate logger for this index.
         */
        private static Logging logger = Logging.GetLogger(typeof(RStarTreeIndex<O>));

        /**
         * Relation
         */
        private IRelation relation;

        /**
         * Constructor.
         * 
         * @param relation Relation to index
         * @param pagefile Page file
         */
        public RStarTreeIndex(IRelation relation, IPageFile<RStarTreeNode> pagefile) :
            base(pagefile)
        {
            this.relation = relation;
            this.Initialize();
        }

        /**
         * Create a new leaf entry.
         * 
         * @param id Object id
         * @return Spatial leaf entry
         */
        protected SpatialPointLeafEntry CreateNewLeafEntry(IDbId id)
        {
            return new SpatialPointLeafEntry(id, relation.VectorAt(id));
        }

        /**
         * Inserts the specified reel vector object into this index.
         * 
         * @param id the object id that was inserted
         */

        public void Insert(IDbId id)
        {
            InsertLeaf(CreateNewLeafEntry(id));
        }

        /**
         * Inserts the specified objects into this index. If a bulk load mode is
         * implemented, the objects are inserted in one bulk.
         * 
         * @param ids the objects to be inserted
         */

        public void InsertAll(IDbIds ids)
        {
            if (ids.IsEmpty() || (ids.Count == 1))
            {
                return;
            }

            // Make an example leaf
            if (CanBulkLoad())
            {
                List<ISpatialEntry> leafs = new List<ISpatialEntry>(ids.Count);
                //  for (DBIDIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var id in ids)
                {
                    leafs.Add(CreateNewLeafEntry(id));
                }
                BulkLoad(leafs);
            }
            else
            {
                //for (DBIDIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var id in ids)
                {
                    Insert(id);
                }
            }

            DoExtraIntegrityChecks();
        }

        /**
         * Deletes the specified object from this index.
         * 
         * @return true if this index did contain the object with the specified id,
         *         false otherwise
         */

        public bool Delete(IDbId id)
        {
            // find the leaf node containing o
            INumberVector obj = relation.VectorAt(id);
            IndexTreePath<ISpatialEntry> deletionPath = FindPathToObject(GetRootPath(), obj, id);
            if (deletionPath == null)
            {
                return false;
            }
            DeletePath(deletionPath);
            return true;
        }

        public void DeleteAll(IDbIds ids)
        {
            //  for (DBIDIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }


        public IRangeQuery GetRangeQuery(IDistanceQuery distanceQuery, params Object[] hints)
        {
            // Query on the relation we index
            if (distanceQuery.Relation != relation)
            {
                return null;
            }
            // Can we support this distance function - spatial distances only!
            if (!(distanceQuery is ISpatialDistanceQuery))
            {
                return null;
            }
            ISpatialDistanceQuery dq = (ISpatialDistanceQuery)distanceQuery;
            return RStarTreeUtil.GetRangeQuery(this, dq, hints);
        }


        public IKNNQuery GetKNNQuery(IDistanceQuery distanceQuery, params Object[] hints)
        {
            // Query on the relation we index
            if (distanceQuery.Relation != relation)
            {
                return null;
            }
            // Can we support this distance function - spatial distances only!
            if (!(distanceQuery is ISpatialDistanceQuery))
            {
                return null;
            }
            ISpatialDistanceQuery dq = (ISpatialDistanceQuery)distanceQuery;
            return RStarTreeUtil.GetKNNQuery(this, dq, hints);
        }


        public String LongName
        {
            get { return "R*-Tree"; }
        }


        public String ShortName
        {
            get { return "rstartree"; }
        }


        protected override Logging GetLogger()
        {
            return logger;
        }
    }
}
