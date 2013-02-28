using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Log;

namespace Socona.Expor.Databases
{

    public class ProxyDatabase : AbstractDatabase
    {
        /**
         * Logger class.
         */
        private static Logging logger = Logging.GetLogger(typeof(ProxyDatabase));

        /**
         * Our DBIDs
         */
        protected IDbIds ids;

        /**
         * Our DBID representation
         */
        protected DbIdView idrep;

        /**
         * Constructor.
         * 
         * @param ids DBIDs to use
         */
        public ProxyDatabase(IDbIds ids)
            : base()
        {

            this.ids = ids;
            this.idrep = new DbIdView(this, this.ids);
            this.relations.Add(idrep);
            this.AddChildResult(idrep);
        }

        /**
         * Constructor.
         * 
         * @param ids DBIDs to use
         * @param relations Relations to contain
         */
        public ProxyDatabase(IDbIds ids, IEnumerable<IRelation> relations)
            : base()
        {

            this.ids = ids;
            this.idrep = new DbIdView(this, this.ids);
            this.relations.Add(idrep);
            this.AddChildResult(idrep);
            foreach (IRelation orel in relations)
            {
                IRelation relation = ProxyView.Wrap(this, ids, orel);
                this.relations.Add(relation);
                this.AddChildResult(relation);
            }
        }

        /**
         * Constructor.
         * 
         * @param ids DBIDs to use
         * @param relations Relations to contain
         */
        public ProxyDatabase(IDbIds ids, params IRelation[] relations) :
            this(ids, (IEnumerable<IRelation>)(relations))
        {
        }

        /**
         * Constructor, proxying all relations of an existing database.
         * 
         * @param ids ids to proxy
         * @param database Database to wrap
         */
        public ProxyDatabase(IDbIds ids, IDatabase database) :
            this(ids, database.GetRelations())
        {
        }


        public override void Initialize()
        {
            // Nothing to do - we were initialized on construction time.
        }

        /**
         * Add a new representation.
         * 
         * @param relation Representation to add.
         */
        public void AddRelation(IRelation relation)
        {
            this.relations.Add(relation);
        }


        protected override Logging GetLogger()
        {
            return logger;
        }
    }
}
