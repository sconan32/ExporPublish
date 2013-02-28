using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Databases
{

    [Description("Database using an in-memory hashtable and at least providing linear scans.")]
    public class HashmapDatabase : AbstractDatabase, IUpdatableDatabase, IParameterizable
    {
        /**
         * Our logger
         */
        private static Logging logger = Logging.GetLogger(typeof(HashmapDatabase));

        /**
         * IDs of this database
         */
        private IHashSetModifiableDbIds ids;

        /**
         * The IDbId representation we use
         */
        private DbIdView idrep;

        /**
         * The data source we Get the initial data from.
         */
        protected IDatabaseConnection databaseConnection;

        /**
         * Constructor.
         * 
         * @param databaseConnection Database connection to Get the initial data from.
         * @param indexFactories Indexes to Add
         */
        public HashmapDatabase(IDatabaseConnection databaseConnection,
            ICollection<IIndexFactory> indexFactories) :
            base()
        {
            this.databaseConnection = databaseConnection;
            this.ids = DbIdUtil.NewHashSet();
            this.idrep = new DbIdView(this, this.ids);
            this.relations.Add(idrep);
            this.AddChildResult(idrep);

            // Add indexes.
            if (indexFactories != null)
            {
                foreach (var ifac in indexFactories)
                {
                    this.indexFactories.Add(ifac);
                }
            }
        }

        /**
         * Constructor with no indexes.
         */
        public HashmapDatabase() :
            this(null, null)
        {
        }

        /**
         * Initialize the database by Getting the initial data from the database
         * connection.
         */

        public override void Initialize()
        {
            if (databaseConnection != null)
            {
                this.Insert(databaseConnection.LoadData());
                // Run at most once.
                databaseConnection = null;
            }
        }


        public IDbIds Insert(IObjectBundle objpackages)
        {
            if (objpackages.DataLength() == 0)
            {
                return DbIdUtil.EMPTYDBIDS;
            }
            // insert into db
            IArrayModifiableDbIds newids = DbIdUtil.NewArray(objpackages.DataLength());
            IRelation[] tarGets = alignColumns(objpackages);

            int idrepnr = -1;
            for (int i = 0; i < tarGets.Length; i++)
            {
                if (tarGets[i] == idrep)
                {
                    idrepnr = i;
                    break;
                }
            }

            for (int j = 0; j < objpackages.DataLength(); j++)
            {
                // insert object
                IDbId newid;
                if (idrepnr < 0)
                {
                    newid = DbIdUtil.GenerateSingleDbId();
                }
                else
                {
                    newid = (IDbId)objpackages.Data(j, idrepnr);
                }
                if (ids.Contains(newid))
                {
                    throw new AbortException("Duplicate IDbId conflict.");
                }
                ids.Add(newid);
                for (int i = 0; i < tarGets.Length; i++)
                {
                    // IDbIds were handled above.
                    if (i == idrepnr)
                    {
                        continue;
                    }

                    IRelation relation = (IRelation)tarGets[i];
                    relation[newid] = objpackages.Data(j, i);
                }
                newids.Add(newid);
            }

            // Notify indexes of insertions
            foreach (IIndex index in indexes)
            {
                index.InsertAll(newids);
            }

            // fire insertion event
            //eventManager.FireObjectsInserted(newids);
            return newids;
        }

        /**
         * Find a mapping from package columns to database columns, eventually Adding
         * new database columns when needed.
         * 
         * @param pack Package to process
         * @return Column mapping
         */
        protected IRelation[] alignColumns(IObjectBundle pack)
        {
            // align representations.
            IRelation[] tarGets = new IRelation[pack.MetaLength()];
            {
                BitArray used = new BitArray(relations.Count);
                for (int i = 0; i < tarGets.Length; i++)
                {
                    SimpleTypeInformation meta = pack.Meta(i);
                    // TODO: aggressively try to match exact metas first?
                    // Try to match unused representations only
                    for (int j = used.NextClearBit(0); j >= 0 && j < relations.Count; j = used.NextClearBit(j + 1))
                    {
                        IRelation relation = (IRelation)relations[(j)];
                        if (relation.GetDataTypeInformation().IsAssignableFromType(meta))
                        {
                            tarGets[i] = relation;
                            used.Set(j, true);
                            break;
                        }
                    }
                    if (tarGets[i] == null)
                    {
                        tarGets[i] = AddNewRelation(meta);
                        used.Set(relations.Count - 1, true);
                    }
                }
            }
            return tarGets;
        }

        /**
         * Add a new representation for the given meta.
         * 
         * @param meta meta data
         * @return new representation
         */
        private IRelation AddNewRelation(SimpleTypeInformation meta)
        {

            SimpleTypeInformation ometa = (SimpleTypeInformation)meta;
            IRelation relation = new MaterializedRelation<Object>(this, ometa, ids);
            relations.Add(relation);
            Hierarchy.Add(this, relation);
            // Try to Add indexes where appropriate
            foreach (IIndexFactory factory in indexFactories)
            {
                if (factory.GetInputTypeRestriction().IsAssignableFromType(meta))
                {

                    IIndexFactory ofact = (IIndexFactory)factory;

                    IRelation orep = (IRelation)relation;
                    AddIndex(ofact.Instantiate(orep));
                }
            }
            return relation;
        }

        /**
         * Removes the objects from the database (by calling {@link #doDelete(IDbId)}
         * for each object) and indexes and fires a deletion event.
         */

        public IObjectBundle Delete(IDbIds ids)
        {
            // Prepare bundle to return
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();
            foreach (IRelation relation in relations)
            {
                List<Object> data = new List<Object>(ids.Count);
                //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                foreach (var iter in ids)
                {
                    data.Add(relation[(iter)]);
                }
                bundle.AppendColumn(relation.GetDataTypeInformation(), data);
            }
            // remove from db
            //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                IDbId id = iter.DbId;
                DoDelete(id);
            }
            // Remove from indexes
            foreach (IIndex index in indexes)
            {
                index.DeleteAll(ids);
            }
            // fire deletion event
            // eventManager.FireObjectsRemoved(ids);

            return bundle;
        }

        /**
         * Removes the object with the specified id from this database.
         * 
         * @param id id the id of the object to be removed
         */
        private void DoDelete(IDbId id)
        {
            // Remove id
            ids.Remove(id);
            // Remove from all representations.
            foreach (IRelation relation in relations)
            {
                // ID has already been removed, and this would loop...
                if (relation != idrep)
                {
                    relation.Delete(id);
                }
            }
            RestoreID(id);
        }

        /**
         * Makes the given id reusable for new insertion operations.
         * 
         * @param id the id to become reusable
         */
        private void RestoreID(IDbId id)
        {
            DbIdFactoryBase.FACTORY.DeallocateSingleDbId(id);
        }


        protected override Logging GetLogger()
        {
            return logger;
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
             * Holds the database connection to Get the initial data from.
             */
            protected IDatabaseConnection databaseConnection = null;

            /**
             * Indexes to Add.
             */
            private ICollection<IIndexFactory> indexFactories;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // Get database connection.
                ObjectParameter<IDatabaseConnection> dbcP = new ObjectParameter<IDatabaseConnection>
                    (OptionDescription.DATABASE_CONNECTION, typeof(IDatabaseConnection),
                    typeof(FileBasedDatabaseConnection));
                if (config.Grab(dbcP))
                {
                    databaseConnection = dbcP.InstantiateClass(config);
                }
                // Get indexes.
                ObjectListParameter<IIndexFactory> indexFactoryP = new ObjectListParameter<IIndexFactory>(
                    INDEX_ID, typeof(IIndexFactory), true);
                if (config.Grab(indexFactoryP))
                {
                    indexFactories = indexFactoryP.InstantiateClasses(config);
                }
            }


            protected override object MakeInstance()
            {
                return new HashmapDatabase(databaseConnection, indexFactories);
            }
        }
    }
}
