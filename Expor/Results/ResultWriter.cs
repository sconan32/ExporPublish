using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Databases;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Results
{

    public class ResultWriter : IResultHandler
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(ResultWriter));

        /**
         * Flag to control GZIP compression.
         * <p>
         * Key: {@code -out.gzip}
         * </p>
         */
        public static OptionDescription GZIP_OUTPUT_ID = OptionDescription.GetOrCreate("out.gzip", "Enable gzip compression of output files.");

        /**
         * Flag to suppress overwrite warning.
         * <p>
         * Key: {@code -out.silentoverwrite}
         * </p>
         */
        public static OptionDescription OVERWRITE_OPTION_ID = OptionDescription.GetOrCreate("out.silentoverwrite", "Silently overwrite output files.");

        /**
         * Holds the file to print results to.
         */
        private FileInfo sout;

        /**
         * Whether or not to do gzip compression on output.
         */
        private bool gzip = false;

        /**
         * Whether or not to warn on overwrite
         */
        private bool warnoverwrite = true;

        /**
         * Constructor.
         * 
         * @param out
         * @param gzip
         * @param warnoverwrite
         */
        public ResultWriter(FileInfo sout, bool gzip, bool warnoverwrite)
        {

            this.sout = sout;
            this.gzip = gzip;
            this.warnoverwrite = warnoverwrite;
        }


        public void ProcessNewResult(IHierarchicalResult baseresult, IResult result)
        {
            ResultTextWriter writer = new ResultTextWriter();

            IStreamFactory output;
            try
            {
                if (this.sout == null)
                {
                    output = new SingleStreamOutput(gzip);
                }
                else if (sout.Exists)
                {
                    //if(1==1) {
                    //  if(warnoverwrite && sout.listFiles().length > 0) {
                    //    logger.warning("Output directory specified is not empty. Files will be overwritten and old files may be left over.");
                    //  }
                    //  output = new MultipleFilesOutput(sout, gzip);
                    //}
                    //else {
                    if (warnoverwrite)
                    {
                        logger.Warning("Output file exists and will be overwritten!");
                    }
                    output = new SingleStreamOutput(sout, gzip);
                    // }
                }
                else
                {
                    // If it doesn't exist yet, make a MultipleFilesOutput.
                    output = new MultipleFilesOutput(sout, gzip);
                }
            }
            catch (IOException e)
            {
                throw new Exception("Error opening output.", e);
            }
            try
            {
                IDatabase db = ResultUtil.FindDatabase(baseresult);
                writer.Output(db, result, output);
            }
            catch (IOException e)
            {
                throw new Exception("Input/Output error while writing result.", e);
            }
            catch (UnableToComplyException e)
            {
                throw new Exception("Unable to comply while writing result.", e);
            }
            output.CloseAllStreams();
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
             * Holds the file to print results to.
             */
            private FileInfo sout = null;

            /**
             * Whether or not to do gzip compression on output.
             */
            private bool gzip = false;

            /**
             * Whether or not to warn on overwrite
             */
            private bool warnoverwrite = true;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                FileParameter outputP = new FileParameter(OptionDescription.OUTPUT, FileParameter.FileType.OUTPUT_FILE, true);
                if (config.Grab(outputP))
                {
                    sout = outputP.GetValue();
                }

                BoolParameter gzipF = new BoolParameter(GZIP_OUTPUT_ID);
                if (config.Grab(gzipF))
                {
                    gzip = gzipF.GetValue();
                }

                BoolParameter overwriteF = new BoolParameter(OVERWRITE_OPTION_ID);
                overwriteF.SetDefaultValue(true);
                if (config.Grab(overwriteF))
                {
                    // note: inversed meaning
                    warnoverwrite = !overwriteF.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new ResultWriter(sout, gzip, warnoverwrite);
            }

           
        }
    }
}
