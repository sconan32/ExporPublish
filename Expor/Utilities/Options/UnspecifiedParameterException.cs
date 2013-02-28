using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Socona.Expor.Utilities.Options.Parameters
{

    public class UnspecifiedParameterException : WrongParameterValueException
    {
        /**
         * Serial UID
         */
       // private static long serialVersionUID = -7142809547201980898L;

        /**
         * Constructor with missing Parameter
         * @param parameter Missing parameter
         */
        public UnspecifiedParameterException(IParameter parameter)
            : base("No value given for parameter \"" + parameter.GetName() + "\":\n" +
            "Expected: " + parameter.GetFullDescription())
        {
        }

        /**
         * Constructor with missing Parameter and cause
         * @param parameter Missing parameter
         * @param cause Cause
         */
        public UnspecifiedParameterException(IParameter parameter, Exception cause)
            : base("No value given for parameter \"" + parameter.GetName() + "\":\n" + 
            "Expected: " + parameter.GetFullDescription(), cause)
        {
        }

        /**
         * Constructor with error message.
         * @param message Message
         */
        public UnspecifiedParameterException(String message)
            : base(message)
        {
        }
    }
}
