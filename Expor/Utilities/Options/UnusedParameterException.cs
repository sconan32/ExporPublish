using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options
{
    public class UnusedParameterException : ParameterException
    {

        /**
         * Thrown by OptionHandler in case of request of an unused parameter.
         * 
         * @param message the detail message
         */
        public UnusedParameterException(String message)
            : base(message)
        {
        }
    }
}
