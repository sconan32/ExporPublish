using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options
{

    public class NoParameterValueException : ParameterException
    {
        /**
         * Serial version UID
         */
        //private static final long serialVersionUID = 8991076624591950629L;

        /**
         * Thrown by OptionHandler in case of incorrect parameter-array.
         *
         * @param message the detail message
         */
        public NoParameterValueException(String message) :
            base(message)
        {
        }

        /**
         * Thrown by OptionHandler in case of incorrect parameter-array.
         *
         * @param message the detail message
         * @param cause   the cause
         */
        public NoParameterValueException(String message, Exception cause) :
            base(message, cause)
        {
        }
    }

}
