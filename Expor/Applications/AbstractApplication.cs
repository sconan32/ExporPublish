using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;

namespace Socona.Expor.Applications
{

    public abstract class AbstractApplication : IParameterizable
    {
        /**
         * We need a static logger in this class, for code used in "main" methods.
         */
        protected static Logging STATIC_LOGGER = Logging.GetLogger(typeof(AbstractApplication));

        /**
         * The newline string according to system.
         */
        private static String NEWLINE = "\n";

        /**
         * Information for citation and version.
         */
        public static String INFORMATION =
            "ExPOR 1.0" + NEWLINE + NEWLINE +
            "PUBLISHED" + NEWLINE +
            "2013, Q. Zhao, WISDOM Lab, SSDUT" + NEWLINE + NEWLINE +
            "ORIGINAL JAVA VERSION:" + NEWLINE +
            "E. Achtert, S. Goldhofer, H.-P. Kriegel, E. Schubert, A. Zimek:" + NEWLINE +
            "Evaluation of Clusterings – Metrics and Visual Support." + NEWLINE;
         

        /**
         * Parameter that specifies the name of the output file.
         * <p>
         * Key: {@code -app.out}
         * </p>
         */
        public static OptionDescription OUTPUT_ID = OptionDescription.GetOrCreate("app.out", "");

        /**
         * Parameter that specifies the name of the input file.
         * <p>
         * Key: {@code -app.in}
         * </p>
         */
        public static OptionDescription INPUT_ID = OptionDescription.GetOrCreate("app.in", "");

        /**
         * Value of verbose flag.
         */
       // private bool verbose;

        /**
         * Constructor.
         * 
         * @param verbose Verbose flag.
         */
        public AbstractApplication(bool verbose)
        {
            if (verbose)
            {
                // Note: do not unset verbose if not --verbose - someone else might
                // have set it intentionally. So don't setVerbose(verbose)!
                //LoggingConfiguration.setVerbosParameterFlage(true);
            }
        }

        /**
         * Returns whether verbose messages should be printed while executing the
         * application.
         * 
         * @return whether verbose messages should be printed while executing the
         *         application
         */
        //public bool IsVerbose()
        //{
        //    return verbose;
        //}

        /**
         * Generic command line invocation.
         * 
         * Refactored to have a central place for outermost exception handling.
         * 
         * @param cls Application class to run.
         * @param args the arguments to run this application with
         */
        public static void RunCliApp(Type cls, String[] args)
        {
            BoolParameter HELP_FLAG = new BoolParameter(OptionDescription.HELP);
            BoolParameter HELP_LONG_FLAG = new BoolParameter(OptionDescription.HELP_LONG);
            ClassParameter DESCRIPTION_PARAM = new ClassParameter(OptionDescription.DESCRIPTION, typeof(Object), true);
            StringParameter DEBUG_PARAM = new StringParameter(OptionDescription.DEBUG, true);

            SerializedParameterization param = new SerializedParameterization(args);
            try
            {
                param.Grab(HELP_FLAG);
                param.Grab(HELP_LONG_FLAG);
                param.Grab(DESCRIPTION_PARAM);
                param.Grab(DEBUG_PARAM);
                if (DESCRIPTION_PARAM.IsDefined())
                {
                    param.ClearErrors();
                    PrintDescription(DESCRIPTION_PARAM.GetValue());
                    return;
                }
                // Fail silently on errors.
                if (param.GetErrors().Count > 0)
                {
                    param.LogAndClearReportedErrors();
                    return;
                }
                if (DEBUG_PARAM.IsDefined())
                {
                    //LoggingUtil.parseDebugParameter(DEBUG_PARAM);
                }
            }
            catch (Exception e)
            {
                PrintErrorMessage(e);
                return;
            }
              try
            {
                TrackParameters config = new TrackParameters(param);
                AbstractApplication task = ClassGenericsUtil.TryInstantiate<AbstractApplication>(typeof(AbstractApplication), cls, config);

                if ((HELP_FLAG.IsDefined() && HELP_FLAG.GetValue()) || (HELP_LONG_FLAG.IsDefined() && HELP_LONG_FLAG.GetValue()))
                {
                    //LoggingConfiguration.setVerbose(true);
                    STATIC_LOGGER.Verbose(Usage(config.GetAllParameters()));
                }
                else
                {
                    param.LogUnusedParameters();
                    if (param.GetErrors().Count > 0)
                    {
                        //LoggingConfiguration.setVerbose(true);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("***ERROR(S) OCCURRED***\n");
                        sb.Append("The following configuration errors prevented execution:\n");
                        
                       
                        foreach (ParameterException e in param.GetErrors())
                        {
                            sb.Append(e.Message).Append("\n");
                            STATIC_LOGGER.Error(e.Message);
                        }
                       // STATIC_LOGGER.Verbose("\n");
                        sb.Append("Stopping execution because of configuration errors.\n");
                        STATIC_LOGGER.Error(sb.ToString());
                    }
                    else
                    {
                        task.Run();
                    }
                }
            }
            catch (Exception e)
            {
                PrintErrorMessage(e);
            }
            //close log service;
              Logging.Shutdown();
        }

        /**
         * Returns a usage message, explaining all known options
         * 
         * @param options Options to show in usage.
         * @return a usage message explaining all known options
         */
        public static String Usage(ICollection<IPair<Object, IParameter>> options)
        {
            StringBuilder usage = new StringBuilder();
            usage.Append(INFORMATION);

            // Collect options
            usage.Append(NEWLINE).Append("Parameters:").Append(NEWLINE);
            OptionUtil.FormatForConsole(usage, FormatUtil.GetConsoleWidth(), "   ", options);

            // FIXME: re-add constraints!
            return usage.ToString();
        }

        /**
         * Print an error message for the given error.
         * 
         * @param e Error Exception.
         */
        protected static void PrintErrorMessage(Exception e)
        {
            if (e is AbortException)
            {
                // ensure we actually show the message:
                // LoggingConfiguration.setVerbose(true);
                STATIC_LOGGER.Verbose(e.Message);
            }
            else if (e is UnspecifiedParameterException)
            {
                STATIC_LOGGER.Error(e.Message);
            }
            else if (e is ParameterException)
            {
                STATIC_LOGGER.Error(e.Message);
            }
            else
            {
                STATIC_LOGGER.Error(e);
            }
        }

        /**
         * Print the description for the given parameter
         */
        private static void PrintDescription(Type descriptionClass)
        {
            if (descriptionClass != null)
            {
                // LoggingConfiguration.setVerbose(true);
                // STATIC_LOGGER.verbose(OptionUtil.describeParameterizable(new StringBuilder(), descriptionClass, FormatUtil.GetConsoleWidth(), "    ").toString());
            }
        }

        /**
         * Runs the application.
         * 
         * @throws de.lmu.ifi.dbs.elki.utilities.exceptions.UnableToComplyException if
         *         an error occurs during running the application
         */
        public abstract void Run();

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            /**
             * Verbose flag
             */
            protected bool verbose = false;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigVerbose(config);
                // Note: we do not run the other methods by default.
                // Only verbose will always be present!
            }

            /**
             * Get the verbose parameter.
             * 
             * @param config Parameterization
             */
            protected void ConfigVerbose(IParameterization config)
            {
                BoolParameter verboseF = new BoolParameter(OptionDescription.VERBOSE_FLAG);
                if (config.Grab(verboseF))
                {
                    verbose = verboseF.GetValue();
                }
            }

            /**
             * Get the output file parameter.
             * 
             * @param config Options
             * @return Output file
             */
            protected FileInfo GetParameterOutputFile(IParameterization config)
            {
                return GetParameterOutputFile(config, "Output filename.");
            }

            /**
             * Get the output file parameter.
             * 
             * @param config Options
             * @param description Short description
             * @return Output file
             */
            protected FileInfo GetParameterOutputFile(IParameterization config, String description)
            {
                FileParameter outputP = new FileParameter(OUTPUT_ID, FileParameter.FileType.OUTPUT_FILE);
                outputP.ShortDescription = description;
                if (config.Grab(outputP))
                {
                    return outputP.GetValue();
                }
                return null;
            }

            /**
             * Get the input file parameter.
             * 
             * @param config Options
             * @return Input file
             */
            protected FileInfo GetParameterInputFile(IParameterization config)
            {
                return GetParameterInputFile(config, "Input filename.");
            }

            /**
             * Get the input file parameter
             * 
             * @param config Options
             * @param description Description
             * @return Input file
             */
            protected FileInfo GetParameterInputFile(IParameterization config, String description)
            {
                FileParameter inputP = new FileParameter(INPUT_ID, FileParameter.FileType.INPUT_FILE);
                inputP.ShortDescription = description;
                if (config.Grab(inputP))
                {
                    return inputP.GetValue();
                }
                return null;
            }


            protected override abstract object MakeInstance();
        }
    }
}
