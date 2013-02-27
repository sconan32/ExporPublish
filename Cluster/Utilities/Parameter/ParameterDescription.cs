using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.Parameter
{
    public class ParameterDescription : ConstantObject<ParameterDescription>
    {

        /**
   * Flag to obtain help-message.
   * <p>
   * Key: {@code -h}
   * </p>
   */
        public static const ParameterDescription HELP = new ParameterDescription("h", "Request a help-message, either for the main-routine or for any specified algorithm. " + "Causes immediate stop of the program.");

        /**
         * Flag to obtain help-message.
         * <p>
         * Key: {@code -help}
         * </p>
         */
        public static const ParameterDescription HELP_LONG = new ParameterDescription("help", "Request a help-message, either for the main-routine or for any specified algorithm. " + "Causes immediate stop of the program.");

        /**
         * ParameterDescription for {@link de.lmu.ifi.dbs.elki.workflow.AlgorithmStep}
         */
        public static const ParameterDescription ALGORITHM = new ParameterDescription("algorithm", "Algorithm to run.");

        /**
         * Optional Parameter to specify a class to obtain a description for.
         * <p>
         * Key: {@code -description}
         * </p>
         */
        public static const ParameterDescription DESCRIPTION = new ParameterDescription("description", "Class to obtain a description of. " + "Causes immediate stop of the program.");

        /**
         * Optional Parameter to specify a class to enable debugging for.
         * <p>
         * Key: {@code -enableDebug}
         * </p>
         */
        public static const ParameterDescription DEBUG = new ParameterDescription("enableDebug", "Parameter to enable debugging for particular packages.");

        /**
         * ParameterDescription for {@link de.lmu.ifi.dbs.elki.workflow.InputStep}
         */
        public static const ParameterDescription DATABASE = new ParameterDescription("db", "Database class.");

        /**
         * ParameterDescription for {@link de.lmu.ifi.dbs.elki.workflow.InputStep}
         */
        // TODO: move to database class?
        public static const ParameterDescription DATABASE_CONNECTION = new ParameterDescription("dbc", "Database connection class.");

        /**
         * OptionID for {@link de.lmu.ifi.dbs.elki.workflow.EvaluationStep}
         */
        public static const ParameterDescription EVALUATOR = new ParameterDescription("evaluator", "Class to evaluate the results with.");

        /**
         * ParameterDescription for {@link de.lmu.ifi.dbs.elki.workflow.OutputStep}
         */
        public static const ParameterDescription RESULT_HANDLER = new ParameterDescription("resulthandler", "Result handler class.");

        /**
         * ParameterDescription for the application output file/folder 
         */
        public static const ParameterDescription OUTPUT = new ParameterDescription("out", "Directory name (or name of an existing file) to write the obtained results in. " + "If this parameter is omitted, per default the output will sequentially be given to STDOUT.");

        /**
         * Flag to allow verbose messages while running the application.
         * <p>
         * Key: {@code -verbose}
         * </p>
         */
        public static const ParameterDescription VERBOSE_FLAG = new ParameterDescription("verbose", "Enable verbose messages.");

        /**
         * Flag to allow verbose messages while running the application.
         * <p>
         * Key: {@code -time}
         * </p>
         */
        public static const ParameterDescription TIME_FLAG = new ParameterDescription("time", "Enable logging of runtime data. Do not combine with more verbose logging, since verbose logging can significantly impact performance.");

        /**
         * The description of the ParameterDescription.
         */
        private String description;

        /**
         * Provides a new ParameterDescription of the given name and description.
         * <p/>
         * All ParameterDescriptions are unique w.r.t. their name. An ParameterDescription provides
         * additionally a description of the option.
         * 
         * @param name the name of the option
         * @param description the description of the option
         */
        private ParameterDescription(String name, String description)
            : base(name)
        {
            this.description = description;
        }

        /**
         * Returns the description of this ParameterDescription.
         * 
         * @return the description of this ParameterDescription
         */
        /**
         * Sets the description of this ParameterDescription.
         * 
         * @param description the description to be set
         */

        public String Description
        {
            get
            {
                return description;
            }
            set { description = value; }
        }

        /**
         * Gets or creates the ParameterDescription for the given class and given name. The
         * ParameterDescription usually is named as the classes name (lowercase) as name-prefix
         * and the given name as suffix of the complete name, separated by a dot. For
         * example, the parameter {@code epsilon} for the class
         * {@link de.lmu.ifi.dbs.elki.algorithm.clustering.DBSCAN} will be named
         * {@code dbscan.epsilon}.
         * 
         * @param name the name
         * @param description the description is also set if the named ParameterDescription does
         *        exist already
         * @return the ParameterDescription for the given name
         */
        public static ParameterDescription GetOrCreate(String name, String description)
        {
            ParameterDescription pd = Get(name);
            if (pd == null)
            {
                pd = new ParameterDescription(name, description);
            }
            else
            {
                pd.Description = description;
            }
            return pd;
        }

        /**
         * Returns the ParameterDescription for the given name if it exists, null otherwise.
         * 
         * @param name name of the desired ParameterDescription
         * @return the ParameterDescription for the given name
         */
        public static ParameterDescription Get(String name)
        {
            return ParameterDescription.Lookup(typeof(ParameterDescription), name);
        }

        /**
         * Returns the name of this ParameterDescription.
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
