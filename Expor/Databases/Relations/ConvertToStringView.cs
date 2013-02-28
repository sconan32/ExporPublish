using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Results;

namespace Socona.Expor.Databases.Relations
{

    public class ConvertToStringView<T> : AbstractHierarchicalResult, IRelation
    {
        /**
         * The database we use
         */
        IRelation existing;

        /**
         * Constructor.
         * 
         * @param existing Existing representation
         */
        public ConvertToStringView(IRelation existing)
            : base()
        {

            this.existing = existing;
        }


        public IDatabase GetDatabase()
        {
            return existing.GetDatabase();
        }

        public object this[IDbIdRef id]
        {
            get { return existing[id].ToString(); }
            set { throw new InvalidOperationException("Covnersion representations are not writable!"); }
        }
        public String Get(IDbIdRef id)
        {
            return existing[id].ToString();
        }


        public void Set(IDbIdRef id, object val)
        {
            throw new InvalidOperationException("Covnersion representations are not writable!");
        }


        public void Delete(IDbIdRef id)
        {
            throw new InvalidOperationException("Covnersion representations are not writable!");
        }


        public IDbIds GetDbIds()
        {
            return existing.GetDbIds();
        }





        public SimpleTypeInformation GetDataTypeInformation()
        {
            return TypeUtil.STRING;
        }


        public override String LongName
        {
            get { return "toString(" + existing.LongName + ")"; }
        }


        public override String ShortName
        {
            get { return "tostring-" + existing.ShortName; }
        }



        public int Count
        {
            get { return existing.Count; }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            return existing.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return existing.GetEnumerator();
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
}
