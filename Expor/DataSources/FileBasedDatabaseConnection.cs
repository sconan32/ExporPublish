using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Filters;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.DataSources
{

    public class FileBasedDatabaseConnection : InputStreamDatabaseConnection
    {
        /**
         * Parameter that specifies the name of the input file to be parsed.
         * <p>
         * Key: {@code -dbc.in}
         * </p>
         */
        public static OptionDescription INPUT_ID = OptionDescription.GetOrCreate("dbc.in",
            "The name of the input file to be parsed.");

        /**
         * Constructor.
         * 
         * @param filters Filters, can be null
         * @param parser the parser to provide a database
         * @param in the input stream to parse from.
         */
        public FileBasedDatabaseConnection(IList<IObjectFilter> filters, IParser parser, Stream ins)
            : base
                (filters, parser)
        {
            this.ins = ins;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : InputStreamDatabaseConnection.Parameterizer
        {
            protected Stream inputStream;


            protected override void MakeOptions(IParameterization config)
            {
                // Add the input file first, for usability reasons.
                FileParameter inputParam = new FileParameter(INPUT_ID, FileParameter.FileType.INPUT_FILE);
                if (config.Grab((IParameter)inputParam))
                {
                    try
                    {
                        inputStream = inputParam.GetValue().OpenRead() ;
                        inputStream = FileUtil.TryGzipInput(inputStream);
                    }
                    catch (IOException e)
                    {
                        config.ReportError(new WrongParameterValueException(
                            inputParam, inputParam.GetValue().FullName, e));
                        inputStream = null;
                    }
                }
                base.MakeOptions(config);
            }


            protected override object MakeInstance()
            {
                return new FileBasedDatabaseConnection(filters, parser, inputStream);
            }
        }
    }
}
