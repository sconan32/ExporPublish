using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Results;

namespace Socona.Expor.Databases.Relations
{

    public class MaterializedRelation<O> : AbstractHierarchicalResult, IRelation
    {
        /**
         * Our database
         */
        private IDatabase database;

        /**
         * The class of objects we store.
         */
        private SimpleTypeInformation type;

        /**
         * Map to hold the objects of the database.
         */
        private IDataStore<O> content;

        /**
         * The DbIds this is supposed to be defined for.
         * 
         * Note: we only keep an unmodifiable reference.
         */
        private IStaticDbIds ids;

        /**
         * The relation name.
         */
        private String name;

        /**
         * The relation name (short version)
         */
        private String shortname = "relation";

        /**
         * Constructor.
         * 
         * @param database Database
         * @param type Type information
         * @param ids IDs
         */
        public MaterializedRelation(IDatabase database, SimpleTypeInformation type, IDbIds ids) :
            this(database, type, ids, null)
        {
        }

        /**
         * Constructor.
         * 
         * @param database Database
         * @param type Type information
         * @param ids IDs
         * @param name Name
         */
        // We can't call this() since we'll have generics issues then.
        public MaterializedRelation(IDatabase database, SimpleTypeInformation type, IDbIds ids, String name)
            : base()
        {
            this.database = database;
            this.type = type;
            this.ids = DbIdUtil.MakeUnmodifiable(ids);
            this.name = name;
            this.content = DataStoreUtil.MakeStorage<O>(ids, DataStoreHints.Database, type.GetRestrictionClass());
        }

        /**
         * Constructor.
         * 
         * @param database Database
         * @param type Type information
         * @param ids IDs
         * @param name Name
         * @param content Content
         */
        public MaterializedRelation(IDatabase database,
            SimpleTypeInformation type, IDbIds ids, String name, IDataStore<O> content)
            : base()
        {
            this.database = database;
            this.type = type;
            this.ids = DbIdUtil.MakeUnmodifiable(ids);
            this.name = name;
            this.content = content;
        }

        /**
         * Constructor.
         * @param name Name
         * @param shortname Short name of the result
         * @param type Type information
         * @param content Content
         * @param ids IDs
         */
        public MaterializedRelation(String name, String shortname,
            SimpleTypeInformation type, IDataStore<O> content, IDbIds ids)
            : base()
        {
            this.database = null;
            this.type = type;
            this.ids = DbIdUtil.MakeUnmodifiable(ids);
            this.name = name;
            this.shortname = shortname;
            this.content = content;
        }


        public IDatabase GetDatabase()
        {
            return database;
        }

        public object this[IDbIdRef id]
        {
            get { return content[(id)]; }
            set
            {
                ((IWritableDataStore<O>)content)[id] = (O)value;
            }
        }

        IDataVector IRelation.DataAt(IDbIdRef id)
        {

            return content[id] as IDataVector;
        }
        INumberVector IRelation.VectorAt(IDbIdRef id)
        {
            return content[id] as INumberVector;
        }
        public O Get(IDbIdRef id)
        {
            return (O)content[id];
        }


        public void Set(IDbIdRef id, object val)
        {
            Debug.Assert(val is O);
            O oval = (O)val;
            Debug.Assert(ids.Contains(id));
            if (content is IWritableDataStore<O>)
            {
                ((IWritableDataStore<O>)content)[id] = oval;
            }
        }

        /**
         * Delete an objects values.
         * 
         * @param id ID to delete
         */

        public void Delete(IDbIdRef id)
        {
            Debug.Assert(!ids.Contains(id));
            if (content is IWritableDataStore<O>)
            {
                ((IWritableDataStore<O>)content).Delete(id);
            }
        }


        public IStaticDbIds GetDbIds()
        {
            return ids;
        }



        public int Size()
        {
            return ids.Count;
        }


        public SimpleTypeInformation GetDataTypeInformation()
        {
            return type;
        }


        public String GetLongName()
        {
            if (name != null)
            {
                return name;
            }
            return type.ToString();
        }


        public String GetShortName()
        {
            return shortname;
        }

        public override string LongName
        {
            get { return type.ToString(); }
        }

        public override string ShortName
        {
            get { return shortname; }
        }


        IDbIds IRelation.GetDbIds()
        {
            return ids;
        }

        public int Count
        {
            get { return ids.Count; }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            return ids.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        IRelation IDatabaseQuery.Relation
        {
            get { return this; }
        }
    }
}
