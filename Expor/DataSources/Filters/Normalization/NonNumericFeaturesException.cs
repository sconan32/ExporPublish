using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.DataSources.Filters.Normalization
{

    public class NonNumericFeaturesException : Exception
    {
        /**
         * Generated serial version UID.
         */
       // private static long serialVersionUID = 284302959521511627L;

        /**
         * An exception to signal the encounter of non numeric features where numeric
         * features have been expected.
         * 
         * @see Exception
         */
        public NonNumericFeaturesException() :
            base()
        {
        }

        /**
         * An exception to signal the encounter of non numeric features where numeric
         * features have been expected.
         * 
         * @param message Message
         * @see Exception
         */
        public NonNumericFeaturesException(String message) :
            base(message)
        {
        }

        /**
         * An exception to signal the encounter of non numeric features where numeric
         * features have been expected.
         * 
         * @param cause Throwable cause
         * @see Exception
         */
        public NonNumericFeaturesException(Exception cause) :
            base("", cause)
        {
        }

        /**
         * An exception to signal the encounter of non numeric features where numeric
         * features have been expected.
         * 
         * @param message Message
         * @param cause Throwable Cause
         * @see Exception
         */
        public NonNumericFeaturesException(String message, Exception cause) :
            base(message, cause)
        {
        }
    }
}
