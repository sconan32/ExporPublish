using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.DataSources.Filters;
using Socona.Expor.DataSources.Parsers;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.DataSources
{
    public class InputStreamDatabaseConnection : AbstractDatabaseConnection
    {
        /**
         * The logger for this class.
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(InputStreamDatabaseConnection));

        /**
         * Holds the instance of the parser.
         */
        protected IParser parser;

        /**
         * The input stream to parse from.
         */
        protected Stream ins;

        /**
         * Constructor.
         * 
         * @param filters Filters to use
         * @param parser the parser to provide a database
         */
        public InputStreamDatabaseConnection(IList<IObjectFilter> filters, IParser parser) :
            base(filters)
        {
            ins = (System.Console.OpenStandardInput());
            this.parser = parser;
        }


        public override MultipleObjectsBundle LoadData()
        {
            // Run parser
            if (logger.IsDebugging)
            {
                logger.Debug("Invoking parsers.");
            }
            if (parser is IStreamingParser)
            {
                IStreamingParser streamParser = (IStreamingParser)parser;
                streamParser.InitStream(ins);

                // normalize objects and transform labels
                if (logger.IsDebugging)
                {
                    logger.Debug("Invoking filters.");
                }
                MultipleObjectsBundle objects = MultipleObjectsBundle.FromStream(InvokeFilters(streamParser));
                return objects;
            }
            else
            {
                MultipleObjectsBundle parsingResult = parser.Parse(ins);

                // normalize objects and transform labels
                if (logger.IsDebugging)
                {
                    logger.Debug("Invoking filters.");
                }
                MultipleObjectsBundle objects = InvokeFilters(parsingResult);
                return objects;
            }
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
        public new class Parameterizer : AbstractDatabaseConnection.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigParser(config, typeof(IParser), typeof(NumberVectorLabelParser));
                ConfigFilters(config);
            }


            protected override object MakeInstance()
            {
                return new InputStreamDatabaseConnection(filters, parser);
            }

        }
    }
}
