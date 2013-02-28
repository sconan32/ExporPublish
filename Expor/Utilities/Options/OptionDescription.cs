using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options
{
    public sealed class OptionDescription : ConstantObject<OptionDescription>
    {
        /**
         * Flag to obtain help-message.
         * <p>
         * Key: {@code -h}
         * </p>
         */
        public static readonly OptionDescription HELP =
            new OptionDescription("h",
                "Request a help-message, either for the main-routine or for any specified algorithm. " +
                "Causes immediate stop of the program.");

        /**
         * Flag to obtain help-message.
         * <p>
         * Key: {@code -help}
         * </p>
         */
        public static readonly OptionDescription HELP_LONG = new OptionDescription("help",
            "Request a help-message, either for the main-routine or for any specified algorithm. " +
            "Causes immediate stop of the program.");

        /**
         * OptionDescription for {@link de.lmu.ifi.dbs.elki.workflow.AlgorithmStep}
         */
        public static readonly OptionDescription ALGORITHM = new OptionDescription("algorithm",
            "Algorithm to run.");

        /**
         * Optional Parameter to specify a class to obtain a description for.
         * <p>
         * Key: {@code -description}
         * </p>
         */
        public static readonly OptionDescription DESCRIPTION = new OptionDescription("description",
            "Class to obtain a description of. " + "Causes immediate stop of the program.");

        /**
         * Optional Parameter to specify a class to enable debugging for.
         * <p>
         * Key: {@code -enableDebug}
         * </p>
         */
        public static readonly OptionDescription DEBUG = new OptionDescription("enableDebug",
            "Parameter to enable debugging for particular packages.");

        /**
         * OptionDescription for {@link de.lmu.ifi.dbs.elki.workflow.InputStep}
         */
        public static readonly OptionDescription DATABASE = new OptionDescription("db",
            "Database class.");

        /**
         * OptionDescription for {@link de.lmu.ifi.dbs.elki.workflow.InputStep}
         */
        // TODO: move to database class?
        public static readonly OptionDescription DATABASE_CONNECTION = new OptionDescription("dbc",
            "Database connection class.");

        /**
         * OptionDescription for {@link de.lmu.ifi.dbs.elki.workflow.EvaluationStep}
         */
        public static readonly OptionDescription EVALUATOR = new OptionDescription("evaluator",
            "Class to evaluate the results with.");

        /**
         * OptionDescription for {@link de.lmu.ifi.dbs.elki.workflow.OutputStep}
         */
        public static readonly OptionDescription RESULT_HANDLER = new OptionDescription("resulthandler",
            "Result handler class.");

        /**
         * OptionDescription for the application output file/folder 
         */
        public static readonly OptionDescription OUTPUT = new OptionDescription("out",
            "Directory name (or name of an existing file) to write the obtained results in. " +
            "If this parameter is omitted, per default the output will sequentially be given to STDOUT.");

        /**
         * Flag to allow verbose messages while running the application.
         * <p>
         * Key: {@code -verbose}
         * </p>
         */
        public static readonly OptionDescription VERBOSE_FLAG = new OptionDescription("verbose",
            "Enable verbose messages.");

        /**
         * Flag to allow verbose messages while running the application.
         * <p>
         * Key: {@code -time}
         * </p>
         */
        public static readonly OptionDescription TIME_FLAG = new OptionDescription("time",
            "Enable logging of runtime data. Do not combine with more verbose logging, since verbose logging can significantly impact performance.");

        /**
         * The description of the OptionDescription.
         */
        private String description;

        /**
         * Provides a new OptionDescription of the given name and description.
         * <p/>
         * All OptionDescriptions are unique w.r.t. their name. An OptionDescription provides
         * additionally a description of the option.
         * 
         * @param name the name of the option
         * @param description the description of the option
         */
        public OptionDescription(String name, String description)
            : base(name)
        {
            this.description = description;
        }

        /**
         * Returns the description of this OptionDescription.
         * 
         * @return the description of this OptionDescription
         */
        public String Description
        {
            get { return description; }
            set { description = value; }
        }


        /**
         * Gets or creates the OptionDescription for the given class and given name. The
         * OptionDescription usually is named as the classes name (lowercase) as name-prefix
         * and the given name as suffix of the complete name, separated by a dot. For
         * example, the parameter {@code epsilon} for the class
         * {@link de.lmu.ifi.dbs.elki.algorithm.clustering.DBSCAN} will be named
         * {@code dbscan.epsilon}.
         * 
         * @param name the name
         * @param description the description is also set if the named OptionDescription does
         *        exist already
         * @return the OptionDescription for the given name
         */
        public static OptionDescription GetOrCreate(String name, String description)
        {
            OptionDescription optionID = Get(name);
            if (optionID == null)
            {
                optionID = new OptionDescription(name, description);
            }
            else
            {
                optionID.Description = description;
            }
            return optionID;
        }

        /**
         * Returns the OptionDescription for the given name if it exists, null otherwise.
         * 
         * @param name name of the desired OptionDescription
         * @return the OptionDescription for the given name
         */
        public static OptionDescription Get(String name)
        {
            return OptionDescription.Lookup(typeof(OptionDescription), name);
        }

        /**
         * Returns the name of this OptionDescription.
         * 
         * @return the name
         * @see java.lang.Object#toString()
         */

        public override String ToString()
        {
            return Name;
        }

    }
}
