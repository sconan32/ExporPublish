using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Applications
{
    public class ExporCliApp : AbstractApplication
    {
        /**
         * The KDD Task to perform.
         */
        ExporTask task;

        /**
         * Constructor.
         * 
         * @param verbose Verbose flag
         * @param task Task to run
         */
        public ExporCliApp(bool verbose, ExporTask task)
            : base(verbose)
        {
            this.task = task;
        }


        public override void Run()
        {
            task.Run();
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractApplication.Parameterizer
        {
            /**
             * The KDD Task to perform.
             */
            protected ExporTask task;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                task = config.TryInstantiate<ExporTask>(typeof(ExporTask));
            }


            protected override object MakeInstance()
            {
                return new ExporCliApp(false, task);
            }

         
        }


    }
}
