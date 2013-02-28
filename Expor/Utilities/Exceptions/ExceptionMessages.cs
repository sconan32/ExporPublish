using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Exceptions
{

    public sealed class ExceptionMessages
    {
        /**
         * Message when the user requested a help message.
         */
        public static String USER_REQUESTED_HELP = "Aborted: User requested help message.";
        /**
         * Messages in case a database is unexpectedly empty.
         */
        public static String DATABASE_EMPTY = "database empty: must contain elements";
        /**
         * Message when a new label was discovered in a database, that did not exist before.
         */
        public static String INCONSISTENT_STATE_NEW_LABEL = "inconsistent state of database - found new label";
        /**
         * Message when an empty clustering is encountered.
         */
        public static String CLUSTERING_EMPTY = "Clustering doesn't contain any cluster.";
        /**
         * Message when a distance doesn't support undefined values.
         */
        public static String UNSUPPORTED_UNDEFINED_DISTANCE = "Undefinded distance not supported!";
        /**
         * Generic "unsupported" message
         */
        public static String UNSUPPORTED = "Unsupported.";
        /**
         * Generic "not yet supported" message
         */
        public static String UNSUPPORTED_NOT_YET = "Not yet supported.";
        /**
         * "remove unsupported" message for iterators
         */
        public static String UNSUPPORTED_REMOVE = "remove() unsupported";
        /**
         * File not found. 404.
         */
        public static String FILE_NOT_FOUND = "File not found";
        /**
         * File already exists, will not overwrite.
         */
        public static String FILE_EXISTS = "File already exists";
    }

}
