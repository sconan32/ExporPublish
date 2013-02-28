using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Results;

namespace Socona.Expor.Databases.Relations
{

    public class ProxyView : AbstractHierarchicalResult, IRelation
    {
        /**
         * Our database
         */
        protected IDatabase database;

        /**
         * The DbIds we contain
         */
        protected IDbIds idview;

        /**
         * The wrapped representation where we get the IDs from.
         */
        protected IRelation inner;

        /**
         * Constructor.
         * 
         * @param idview Ids to expose
         * @param inner Inner representation
         */
        public ProxyView(IDatabase database, IDbIds idview, IRelation inner)
            : base()
        {

            this.database = database;
            this.idview = DbIdUtil.MakeUnmodifiable(idview);
            this.inner = inner;
        }


        public IDatabase GetDatabase()
        {
            return database;
        }

        /**
         * Constructor-like static method.
         * 
         * @param <O> Object type
         * @param idview Ids to expose
         * @param inner Inner representation
         * @return Instance
         */
        public static ProxyView Wrap(IDatabase database, IDbIds idview, IRelation inner)
        {
            return new ProxyView(database, idview, inner);
        }


        public virtual IDataVector DataAt(IDbIdRef id)
        {
            Debug.Assert(idview.Contains(id), "Accessing object not included in view.");
            return (IDataVector)inner[id];
        }
        public virtual INumberVector VectorAt(IDbIdRef id)
        {
            Debug.Assert(idview.Contains(id), "Accessing object not included in view.");
            return inner[id] as INumberVector;
        }



        public void Delete(IDbIdRef id)
        {
            throw new InvalidOperationException("Semantics of 'delete-from-virtual-partition' are not uniquely defined. Delete from IDs or from underlying data, please!");
        }


        public IDbIds GetDbIds()
        {
            return idview;
        }


        //public DbIdIter iterDbIds()
        //{
        //    return idview.iter();
        //}


        public int Count
        {
            get { return idview.Count; }
        }


        public virtual SimpleTypeInformation GetDataTypeInformation()
        {
            return inner.GetDataTypeInformation();
        }


        public override String LongName
        {
            get { return "Partition of " + inner.LongName; }
        }


        public override String ShortName
        {
            get { return "partition"; }
        }


        public virtual object this[IDbIdRef id]
        {
            get
            {
                return inner[id];
            }
            set
            {
                inner[id] = value;
            }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            return idview.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return idview.GetEnumerator();
        }
        IRelation IDatabaseQuery.Relation
        {
            get { return this; }
        }
    }
}
