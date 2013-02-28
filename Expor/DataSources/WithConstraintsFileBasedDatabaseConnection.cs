using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.DataSources.Filters;
using Socona.Expor.DataSources.Parsers;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.DataSources
{
    class WithConstraintsFileBasedDatabaseConnection : FileBasedDatabaseConnection
    {
        private static readonly Logging logger = Logging.GetLogger(typeof(WithConstraintsFileBasedDatabaseConnection));

        /// <summary>
        ///  Parameter that specifies the name of the mustlink input file to be parsed.
        ///  <para>Key:<code>-dbc.in</code></para>
        /// </summary>
        public static OptionDescription INPUT_CONS_ID = OptionDescription.GetOrCreate("dbc.in.cons",
            "The name of the mustlink input file to be parsed.");
     
        public static OptionDescription CONS_PARSER_ID = OptionDescription.GetOrCreate("dbc.consparser",
         "The name of the cannot input file to be parsed.");

        Stream consins;
        
        IParser consParser;
        public WithConstraintsFileBasedDatabaseConnection(IList<IObjectFilter> filters,IParser dataparser,
            Stream datains,Stream consins,IParser consParser)
            : base(filters, dataparser, datains)
        {
            this.consins = consins;
            
            this.consParser = consParser;  
        }

        public override MultipleObjectsBundle LoadData()
        {
            MultipleObjectsBundle bundle= base.LoadData();

            return bundle;
        }
        public MultipleObjectsBundle LoadConstraints()
        {
            // Run parser
            if (logger.IsDebugging)
            {
                logger.Debug("Invoking parsers.");
            }
            if (parser is IStreamingParser)
            {
                IStreamingParser streamParser = (IStreamingParser)consParser;
                streamParser.InitStream(consins);

                // normalize objects and transform labels
                if (logger.IsDebugging)
                {
                    logger.Debug("Reading Constrints");
                }
                //TODO: 是否应该对约束使用Filters？
                MultipleObjectsBundle objects = MultipleObjectsBundle.FromStream(InvokeFilters(streamParser));

                return objects;

               
            }
            else
            {
                MultipleObjectsBundle parsingResult = parser.Parse(consins);

                // normalize objects and transform labels
                if (logger.IsDebugging)
                {
                    logger.Debug("Invoking filters.");
                }
                MultipleObjectsBundle objects = InvokeFilters(parsingResult);
                return objects;
            }
        }
       

        public new class Parameterizer : FileBasedDatabaseConnection.Parameterizer
        {
            protected Stream mlStream;
          
            protected IParser consparser;

            protected override void MakeOptions(IParameterization config)
            {
                // Add the input file first, for usability reasons.
             
                base.MakeOptions(config);
                FileParameter mlParam = new FileParameter(INPUT_CONS_ID, FileParameter.FileType.INPUT_FILE);
                if (config.Grab((IParameter)mlParam))
                {
                    try
                    {
                        mlStream = mlParam.GetValue().OpenRead();
                        mlStream = FileUtil.TryGzipInput(mlStream);
                    }
                    catch (IOException e)
                    {
                        config.ReportError(new WrongParameterValueException(
                            mlParam, mlParam.GetValue().FullName, e));
                        mlStream = null;
                    }
                }
               
                ObjectParameter<IParser> consParserParam = new ObjectParameter<IParser>(
                    PARSER_ID, typeof(IParser),typeof( PairwiseConstraintsParser));
                if (config.Grab(consParserParam))
                {
                    consparser = consParserParam.InstantiateClass(config);
                }

            }


            protected override object MakeInstance()
            {
                return new WithConstraintsFileBasedDatabaseConnection(filters, parser, 
                    inputStream,mlStream,consparser);
            }
        }



    }
}
