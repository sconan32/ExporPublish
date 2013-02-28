using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Exceptions
{

    public class APIViolationException : AbortException
    {
        /**
         * Serial version
         */
        //private static long serialVersionUID = 1L;

        /**
         * Constructor.
         * 
         * @param message Error message
         */
        public APIViolationException(String message)
            : base(message)
        {
        }

        /**
         * Constructor.
         * 
         * @param message Error message
         * @param cause Reason
         */
        public APIViolationException(String message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
