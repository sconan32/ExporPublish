using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.WorkFlows
{

    public class InputStep : IWorkflowStep
    {
        /**
         * Holds the database to have the algorithms run with.
         */
        private IDatabase database;

        /**
         * Constructor.
         *
         * @param database Database to use
         */
        public InputStep(IDatabase database) :
            base()
        {
            this.database = database;
        }

        /**
         * Get the database to use.
         * 
         * @return Database
         */
        public IDatabase GetDatabase()
        {
            database.Initialize();
            return database;
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
             * Holds the database to have the algorithms run on.
             */
            protected IDatabase database = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IDatabase> dbP = new ObjectParameter<IDatabase>(
                    OptionDescription.DATABASE, typeof(IDatabase), typeof(StaticArrayDatabase));
                if (config.Grab(dbP))
                {
                    database = dbP.InstantiateClass(config);
                }
            }


            protected override object MakeInstance()
            {
                return new InputStep(database);
            }

          
        }
    }
}
