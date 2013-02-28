using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Constraints;
using Socona.Expor.Constraints.Pairwise;
using Socona.Expor.DataSources;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Databases
{
    class WithConstraintsStaticArrayDatabase : StaticArrayDatabase
    {
        /**
      * Parameter to specify the indexes to use.
      * <p>
      * Key: {@code -db.index}
      * </p>
      */
        public static readonly OptionDescription CONS_HANDLER_ID = OptionDescription.GetOrCreate("db.conshandler", "Method to deal with constraints");

        private static readonly Logging logger = Logging.GetLogger(typeof(WithConstraintsStaticArrayDatabase));

        IConstraintHandler conshandler;

        public IConstraintHandler ConstraintHandler
        {
            get { return conshandler; }
            // set { conshandler = value; }
        }

        public WithConstraintsStaticArrayDatabase(IDatabaseConnection databaseConnection, ICollection<IIndexFactory> indexFactories,
            IConstraintHandler conshandler)
            :
          base(databaseConnection, indexFactories)
        {
            this.conshandler = conshandler;
        }
        public override void Initialize()
        {
            MultipleObjectsBundle conspack;

            conspack = ((WithConstraintsFileBasedDatabaseConnection)databaseConnection).LoadConstraints();

            base.Initialize();
            if (conshandler != null)
            {
                conshandler.HandleConstraints(this.ids, conspack);
            }

        }

        public new class Parameterizer : AbstractParameterizer
        {
            /**
             * Holds the database connection to get the initial data from.
             */
            protected IDatabaseConnection databaseConnection = null;

            /**
             * Indexes to add.
             */
            private ICollection<IIndexFactory> indexFactories;

            protected IConstraintHandler conshandler = null;

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // Get database connection.
                ObjectParameter<IDatabaseConnection> dbcP = new ObjectParameter<IDatabaseConnection>(
                    OptionDescription.DATABASE_CONNECTION,
                    typeof(IDatabaseConnection), typeof(WithConstraintsFileBasedDatabaseConnection));
                if (config.Grab(dbcP))
                {
                    databaseConnection = dbcP.InstantiateClass(config);
                }
                // Get indexes.
                ObjectListParameter<IIndexFactory> indexFactoryP =  new ObjectListParameter<IIndexFactory>(
                        INDEX_ID, typeof(IIndexFactory), true);
                if (config.Grab(indexFactoryP))
                {
                    indexFactories = indexFactoryP.InstantiateClasses(config);
                }
                //Get Constraints handler
                ObjectParameter<IConstraintHandler> conshp = new ObjectParameter<IConstraintHandler>(
                  CONS_HANDLER_ID,
                   typeof(IConstraintHandler), typeof(PairwiseConstraintsHandler));
                if (config.Grab(conshp))
                {
                    conshandler = conshp.InstantiateClass(config);
                }
            }


            protected override object MakeInstance()
            {
                return new WithConstraintsStaticArrayDatabase(databaseConnection, indexFactories, conshandler);
            }


        }
    }
}
