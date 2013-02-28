using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Exceptions
{
    public class UnableToComplyException : Exception
    {
        /**
         * Exception to state inability to execute an operation.
         * 
         */
        public UnableToComplyException()
            : base()
        {

        }

        /**
         * Exception to state inability to execute an operation.
         * 
         * @param message a message to describe cause of exception
         */
        public UnableToComplyException(String message)
            : base(message)
        {

        }

        /**
         * Exception to state inability to execute an operation.
         * 
         * @param message a message to describe cause of exception
         * @param cause cause of exception
         */
        public UnableToComplyException(String message, Exception cause)
            : base(message, cause)
        {

        }


    }
}
