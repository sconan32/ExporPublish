using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Databases
{
    public class StaticArrayDatabase : AbstractDatabase, IParameterizable
    {
        /**
         * Our logger
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(StaticArrayDatabase));

        /**
         * IDs of this database
         */
        protected IArrayDbIds ids;

        /**
         * The DbId representation we use
         */
        private DbIdView idrep;

        /**
         * The data source we get the initial data from.
         */
        protected IDatabaseConnection databaseConnection;

        /**
         * Constructor.
         * 
         * @param databaseConnection Database connection to get the initial data from.
         * @param indexFactories Indexes to add
         */
        public StaticArrayDatabase(IDatabaseConnection databaseConnection, ICollection<IIndexFactory> indexFactories)
            :
          base()
        {
            this.databaseConnection = databaseConnection;
            this.ids = null;
            this.idrep = null;

            // Add indexes.
            if (indexFactories != null)
            {
                this.indexFactories.Concat(indexFactories);
            }
        }

        /**
         * Constructor with no indexes.
         */
        public StaticArrayDatabase() :
            this(null, null)
        {
        }

        /**
         * Initialize the database by getting the initial data from the database
         * connection.
         */

        public override void Initialize()
        {
            if (databaseConnection != null)
            {
                if (logger.IsDebugging)
                {
                    logger.Debug("Loading data from database connection.");
                }
                MultipleObjectsBundle objpackages = databaseConnection.LoadData();
                // Run at most once.
                databaseConnection = null;

                // Find DbId column
                int idrepnr = FindDbIdColumn(objpackages);
                // Build DbId array
                if (idrepnr == -1)
                {
                    this.ids = DbIdUtil.GenerateStaticDbIdRange(objpackages.DataLength());
                }
                else
                {
                    IArrayModifiableDbIds newids = DbIdUtil.NewArray(objpackages.DataLength());
                    for (int j = 0; j < objpackages.DataLength(); j++)
                    {
                        IDbId newid = (IDbId)objpackages.Data(j, idrepnr);
                        newids.Add(newid);
                    }
                    this.ids = newids;
                }
                // Replace id representation.
                // TODO: this is an ugly hack
                this.idrep = new DbIdView(this, this.ids);
                relations.Add((IRelation)this.idrep);

                this.Hierarchy.Add(this, idrep);

                // insert into db - note: DbIds should have been prepared before this!
                IRelation[] targets = AlignColumns(objpackages);

                for (int j = 0; j < objpackages.DataLength(); j++)
                {
                    // insert object
                    IDbId newid = ids[j];
                    for (int i = 0; i < targets.Length; i++)
                    {
                        // DbIds were handled above.
                        if (i == idrepnr)
                        {
                            continue;
                        }

                        // IRelation relation = (IRelation)targets[i];
                        // relation.Set(newid, objpackages.Data(j, i));
                        targets[i][newid] = objpackages.Data(j, i);
                    }
                }

                foreach (IRelation relation in relations)
                {
                    SimpleTypeInformation meta = relation.GetDataTypeInformation();
                    // Try to add indexes where appropriate
                    foreach (IIndexFactory factory in indexFactories)
                    {
                        if (factory.GetInputTypeRestriction().IsAssignableFromType(meta))
                        {

                            IIndexFactory ofact = (IIndexFactory)factory;

                            IRelation orep = (IRelation)relation;
                            IIndex index = ofact.Instantiate(orep);
                            AddIndex(index);
                            index.InsertAll(ids);
                        }
                    }
                }

                // fire insertion event
                // eventManager.FireObjectsInserted(ids);
                OnObjectsInserted(ids);
            }
        }


        public override void AddIndex(IIndex index)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Adding index: " + index);
            }
            this.indexes.Add(index);
            // TODO: actually add index to the representation used?
            this.AddChildResult(index);
        }


        /**
         * Find an DbId column.
         * 
         * @param pack Package to process
         * @return DbId column
         */
        protected int FindDbIdColumn(IObjectBundle pack)
        {
            for (int i = 0; i < pack.MetaLength(); i++)
            {
                SimpleTypeInformation meta = pack.Meta(i);
                if (TypeUtil.DBID.IsAssignableFromType(meta))
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Find a mapping from package columns to database columns, eventually adding
         * new database columns when needed.
         * 
         * @param pack Package to process
         * @return Column mapping
         */
        protected IRelation[] AlignColumns(IObjectBundle pack)
        {
            // align representations.
            IRelation[] targets = new IRelation[pack.MetaLength()];

            //2013/9/25
            //这个地方有问题，在java中bitset的构造函数好像没有什么作用，
            //下标越界也不会报错。
            //但是在C#中这个会报下标越界。
            //是修改这个地方还是下面的逻辑待查
            BitArray used = new BitArray(3 * relations.Count);
            for (int i = 0; i < targets.Length; i++)
            {
                SimpleTypeInformation meta = pack.Meta(i);
                // TODO: aggressively try to match exact metas first?
                // Try to match unused representations only
                for (int j = 0; j >= 0 && j < relations.Count; j = j + 1)
                {
                    if (used[j] == false)
                    {
                        IRelation relation = relations[j];
                        if (relation.GetDataTypeInformation().IsAssignableFromType(meta))
                        {
                            targets[i] = relation;

                            used.Set(j, true);
                            break;
                        }
                        if (targets[i] == null)
                        {
                            targets[i] = AddNewRelation(meta);

                            used.Set(relations.Count - 1, true);
                        }
                    }
                }

            }
            return targets;
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
            IRelation relation = new MaterializedRelation<object>(this, ometa, ids);
            relations.Add(relation);
            this.Hierarchy.Add(this, relation);
            return relation;
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
             * Holds the database connection to get the initial data from.
             */
            protected IDatabaseConnection databaseConnection = null;

            /**
             * Indexes to add.
             */
            private ICollection<IIndexFactory> indexFactories;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // Get database connection.
                ObjectParameter<IDatabaseConnection> dbcP = new ObjectParameter<IDatabaseConnection>(
                    OptionDescription.DATABASE_CONNECTION,
                    typeof(IDatabaseConnection), typeof(FileBasedDatabaseConnection));
                if (config.Grab(dbcP))
                {
                    databaseConnection = dbcP.InstantiateClass(config);
                }
                // Get indexes.
                ObjectListParameter<IIndexFactory> indexFactoryP =
                    new ObjectListParameter<IIndexFactory>(
                        INDEX_ID, typeof(IIndexFactory), true);
                if (config.Grab(indexFactoryP))
                {
                    indexFactories = indexFactoryP.InstantiateClasses(config);
                }
            }


            protected override object MakeInstance()
            {
                return new StaticArrayDatabase(databaseConnection, indexFactories);
            }


        }
    }
}
