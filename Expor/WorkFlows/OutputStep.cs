using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Visualization.Gui;

namespace Socona.Expor.WorkFlows
{

    public class OutputStep : IWorkflowStep
    {
        /**
         * Output handler.
         */
        private IList<IResultHandler> resulthandlers = null;

        /**
         * Constructor.
         * 
         * @param resulthandlers Result handlers to use
         */
        public OutputStep(IList<IResultHandler> resulthandlers)
        {

            this.resulthandlers = resulthandlers;
        }

        /**
         * Run the result handlers.
         * 
         * @param result Result to run on
         */
        public void RunResultHandlers(IHierarchicalResult result)
        {
            // Run result handlers
            foreach (IResultHandler resulthandler in resulthandlers)
            {
                resulthandler.ProcessNewResult(result, result);
            }
        }

        /**
         * Set the default handler to the {@link ResultWriter}.
         */
        public static void SetDefaultHandlerWriter()
        {
            defaultHandlers = new List<Type>(1);
            defaultHandlers.Add(typeof(ResultWriter));
        }

        /**
         * Set the default handler to the {@link ResultVisualizer}.
         */
        public static void SetDefaultHandlerVisualizer()
        {
            defaultHandlers = new List<Type>(1);
            defaultHandlers.Add(typeof(ResultVisualizer));
        }

        protected static List<Type> defaultHandlers = null;

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
             * Output handlers.
             */
            private IList<IResultHandler> resulthandlers = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // result handlers
                ObjectListParameter<IResultHandler> resultHandlerParam =
                    new ObjectListParameter<IResultHandler>(OptionDescription.RESULT_HANDLER, typeof(IResultHandler));
                if (defaultHandlers != null)
                {
                    resultHandlerParam.SetDefaultValue(defaultHandlers);
                }
                if (config.Grab(resultHandlerParam))
                {
                    resulthandlers = resultHandlerParam.InstantiateClasses(config);
                }
            }


            protected override object MakeInstance()
            {
                return new OutputStep(resulthandlers);
            }


        }
    }
}
