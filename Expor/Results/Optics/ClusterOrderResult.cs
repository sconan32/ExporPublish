using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Results.Optics
{

    public class ClusterOrderResult : BasicResult, IIterableResult<IClusterOrderEntry>
    {
        /**
         * Cluster order storage
         */
        private List<IClusterOrderEntry> clusterOrder;

        /**
         * Map of object IDs to their cluster order entry
         */
        private IWritableDataStore<IClusterOrderEntry> map;

        /**
         * The IDbIds we are defined for
         */
        IModifiableDbIds dbids;

        /**
         * Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         */
        public ClusterOrderResult(String name, String shortname) :
            base(name, shortname)
        {
            clusterOrder = new List<IClusterOrderEntry>();
            dbids = DbIdUtil.NewHashSet();
            map = DataStoreUtil.MakeStorage<IClusterOrderEntry>(dbids, DataStoreHints.Database, typeof(IClusterOrderEntry));

            AddChildResult(new ClusterOrderAdapter(clusterOrder, dbids));
            AddChildResult(new ReachabilityDistanceAdapter(map, dbids));
            AddChildResult(new PredecessorAdapter(map, dbids));
        }

        /**
         * Retrieve the complete cluster order.
         * 
         * @return cluster order
         */
        public List<IClusterOrderEntry> GetClusterOrder()
        {
            return clusterOrder;
        }

        /**
         * The cluster order is iterable
         */

        //public Iterator<ClusterOrderEntry> iterator() {
        //  return clusterOrder.iterator();
        //}

        /**
         * Add an object to the cluster order.
         * 
         * @param id Object ID
         * @param predecessor Predecessor ID
         * @param reachability Reachability distance
         */
        public void Add(IDbId id, IDbId predecessor, IDistanceValue reachability)
        {
            Add(new GenericClusterOrderEntry(id, predecessor, reachability));
            dbids.Add(id);
        }

        /**
         * Add an object to the cluster order.
         * 
         * @param ce Entry
         */
        public void Add(IClusterOrderEntry ce)
        {
            clusterOrder.Add(ce);
            map[ce.GetID()] = ce;
            dbids.Add(ce.GetID());
        }

        /**
         * Get the distance class
         * 
         * @return distance class. Can be {@code null} for an all-undefined result!
         */
        public Type GetDistanceClass()
        {
            foreach (IClusterOrderEntry ce in clusterOrder)
            {
                IDistanceValue dist = ce.GetReachability();
                if (dist != null)
                {
                    return dist.GetType();
                }
            }
            return null;
        }

        /**
         * Ordering part of the result.
         * 
         * @author Erich Schubert
         */
        class ClusterOrderAdapter : IOrderingResult, IResultAdapter
        {
            /**
             * Access reference.
             */
            private List<IClusterOrderEntry> clusterOrder;

            private IDbIds ids;
            /**
             * Constructor.
             * 
             * @param clusterOrder order to return
             */
            public ClusterOrderAdapter(List<IClusterOrderEntry> clusterOrder, IDbIds ids) :
                base()
            {
                this.clusterOrder = clusterOrder;
                this.ids = ids;
            }


            public IDbIds GetDbIds()
            {
                return ids;
            }

            /**
             * Use the cluster order to sort the given collection ids.
             * 
             * Implementation of the {@link OrderingResult} interface.
             */

            public IArrayModifiableDbIds Iter(IDbIds ids)
            {
                IArrayModifiableDbIds res = DbIdUtil.NewArray(ids.Count);
                foreach (IClusterOrderEntry e in clusterOrder)
                {
                    if (ids.Contains(e.GetID()))
                    {
                        res.Add(e.GetID());
                    }
                }
                return res;
            }


            public String LongName
            {
                get { return "Derived Object Order"; }
            }


            public String ShortName
            {
                get { return "clusterobjectorder"; }
            }
        }

        /**
         * Result containing the reachability distances.
         * 
         * @author Erich Schubert
         */
        class ReachabilityDistanceAdapter : IRelation, IResultAdapter
        {
            /**
             * Access reference.
             */
            private IDataStore<IClusterOrderEntry> map;

            /**
             * IDbIds
             */
            private IDbIds dbids;

            /**
             * Constructor.
             * 
             * @param map Map that stores the results.
             * @param dbids IDbIds we are defined for.
             */
            public ReachabilityDistanceAdapter(IDataStore<IClusterOrderEntry> map, IDbIds dbids) :
                base()
            {
                this.map = map;
                this.dbids = dbids;
            }

            public String LongName
            {
                get { return "Reachability"; }
            }


            public String ShortName
            {
                get { return "reachability"; }
            }


            public IDbIds GetDbIds()
            {
                return DbIdUtil.MakeUnmodifiable(dbids);
            }


            //public DbIdIter iterDbIds() {
            //  return dbids.iter();
            //}




            public IDatabase GetDatabase()
            {
                return null; // FIXME
            }
            public object this[IDbIdRef index]
            {
                get { return Get(index); }
                set { Set(index, (IDistanceValue)value); }
            }
            public IDistanceValue Get(IDbIdRef objID)
            {
                return map[(objID)].GetReachability();
            }
            public void Set(IDbIdRef id, IDistanceValue val)
            {
                throw new InvalidOperationException();
            }


            public void Delete(IDbIdRef id)
            {
                throw new InvalidOperationException();
            }

            public SimpleTypeInformation GetDataTypeInFormation()
            {
                return new SimpleTypeInformation(typeof(IDistanceValue));
            }


            public ResultHierarchy GetHierarchy()
            {
                return this.GetHierarchy();
            }


            public void SetHierarchy(ResultHierarchy hierarchy)
            {
                this.SetHierarchy(hierarchy);
            }


            public SimpleTypeInformation GetDataTypeInformation()
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return dbids.Count; }
            }

            public ResultHierarchy Hierarchy
            {
                get
                {
                    return this.GetHierarchy();
                }
                set
                {
                    this.SetHierarchy(value);
                }
            }

            public IEnumerator<IDbId> GetEnumerator()
            {
                return dbids.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return dbids.GetEnumerator();
            }
            IDataVector IRelation.DataAt(IDbIdRef id)
            {
                throw new InvalidOperationException("You Cannot read Data from this type");
            }
            INumberVector IRelation.VectorAt(IDbIdRef id)
            {
                throw new InvalidOperationException("You Cannot read data from this type");
            }
            IRelation Databases.Queries.IDatabaseQuery.Relation
            {
                get { return this; }
            }
        }

        /**
         * Result containing the predecessor ID.
         * 
         * @author Erich Schubert
         */
        class PredecessorAdapter : IRelation, IResultAdapter
        {
            /**
             * Access reference.
             */
            private IDataStore<IClusterOrderEntry> map;

            /**
             * Database IDs
             */
            private IDbIds dbids;

            /**
             * Constructor.
             * 
             * @param map Map that stores the results.
             * @param dbids IDbIds we are defined for
             */
            public PredecessorAdapter(IDataStore<IClusterOrderEntry> map, IDbIds dbids)
                : base()
            {
                this.map = map;
                this.dbids = dbids;
            }


            public IDbId Get(IDbIdRef objID)
            {
                return map[(objID)].GetPredecessorID();
            }


            public String LongName
            {
                get { return "Predecessor"; }
            }


            public String ShortName
            {
                get { return "predecessor"; }
            }


            public IDbIds GetDbIds()
            {
                return DbIdUtil.MakeUnmodifiable(dbids);
            }

            IDataVector IRelation.DataAt(IDbIdRef id)
            {
                throw new InvalidOperationException("You Cannot read Data from this type");
            }
            INumberVector IRelation.VectorAt(IDbIdRef id)
            {
                throw new InvalidOperationException("You Cannot read data from this type");
            }


            public IDatabase GetDatabase()
            {
                return null; // FIXME
            }


            public void Set(IDbIdRef id, IDbId val)
            {
                throw new InvalidOperationException();
            }


            public void Delete(IDbIdRef id)
            {
                throw new InvalidOperationException();
            }


            public SimpleTypeInformation GetDataTypeInformation()
            {
                return TypeUtil.DBID;
            }


            //public ResultHierarchy GetHierarchy()
            //{
            //    return this.GetHierarchy();
            //}


            //public void SetHierarchy(ResultHierarchy hierarchy)
            //{
            //    this.setHierarchy(hierarchy);
            //}


            public object this[IDbIdRef id]
            {
                get
                {
                    return map[(id)].GetPredecessorID();
                }
                set
                {
                    throw new InvalidOperationException();
                }
            }




            public int Count
            {
                get { return dbids.Count; }
            }

            public ResultHierarchy Hierarchy
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IEnumerator<IDbId> GetEnumerator()
            {
                return dbids.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return dbids.GetEnumerator();
            }
            IRelation Databases.Queries.IDatabaseQuery.Relation
            {
                get { return this; }
            }
        }

        public IEnumerator<IClusterOrderEntry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}
