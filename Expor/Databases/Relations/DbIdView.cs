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
    public class DbIdView : AbstractHierarchicalResult, IRelation
    {
        /**
         * The database
         */
        private IDatabase database;

        /**
         * The ids object
         */
        private IDbIds ids;

        /**
         * Constructor.
         * 
         * @param database
         * @param ids
         */
        public DbIdView(IDatabase database, IDbIds ids) :
            base()
        {
            this.database = database;
            this.ids = DbIdUtil.MakeUnmodifiable(ids);
        }


        public IDatabase GetDatabase()
        {
            return database;
        }


        public object this[IDbIdRef id]
        {
            get
            {
                Debug.Assert(ids.Contains(id));
                return id.DbId;
            }
            set { throw new InvalidOperationException("DbIds cannot be changed."); }
      
        }
        
        public IDbId Get(IDbIdRef id)
        {
            Debug.Assert(ids.Contains(id));
            return id.DbId;
        }

      

        public void Set(IDbIdRef id, object val)
        {
            throw new InvalidOperationException("DbIds cannot be changed.");
        }


        public void Delete(IDbIdRef id)
        {
            if (database is IUpdatableDatabase)
            {
                // TODO: skip GetDbId()
                ((IUpdatableDatabase)database).Delete(id.DbId);
            }
            else
            {
                throw new InvalidOperationException("Deletions are not supported.");
            }
        }


        public SimpleTypeInformation GetDataTypeInformation()
        {
            return TypeUtil.DBID;
        }


        public IDbIds GetDbIds()
        {
            return ids;
        }



        public int Size()
        {
            return ids.Count;
        }


        public String GetLongName()
        {
            return "Database IDs";
        }


        public String GetShortName()
        {
            return "DbId";
        }

        public override string LongName
        {
            get { throw new NotImplementedException(); }
        }

        public override string ShortName
        {
            get { throw new NotImplementedException(); }
        }




        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IDataVector IRelation.DataAt(IDbIdRef id)
        {
            throw new InvalidOperationException("You Cannot read Data from this type");
        }
        INumberVector IRelation.VectorAt(IDbIdRef id)
        {
            throw new InvalidOperationException("You Cannot read data from this type");
        }
        IRelation IDatabaseQuery.Relation
        {
            get { return this; }
        }
    }
}
