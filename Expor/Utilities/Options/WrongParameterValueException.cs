using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class WrongParameterValueException : ParameterException
    {
        /**
         * Generated serial version UID.
         */
        // private static final long serialVersionUID = 2155964376772417402L;

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param parameter the parameter that has a wrong value
         * @param read the value of the parameter read by the option handler
         */
        public WrongParameterValueException(IParameter parameter, String read) :
            this("Wrong value of parameter \"" + parameter.GetName() + "\".\n" + "Read: " + read + ".\n" + "Expected: " + parameter.GetFullDescription())
        {
        }

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param parameter the parameter that has a wrong value
         * @param read the value of the parameter read by the option handler
         * @param cause the cause
         */
        public WrongParameterValueException(IParameter parameter, String read, Exception cause) :
            this("Wrong value of parameter \"" + parameter.GetName() + "\".\n" + "Read: " + read + ".\n" + "Expected: " + parameter.GetFullDescription(), cause)
        {
        }

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param parameter the parameter that has a wrong value
         * @param read the value of the parameter read by the option handler
         * @param reason detailed error description
         * @param cause the cause
         */
        public WrongParameterValueException(IParameter parameter, String read, String reason, Exception cause) :
            this("Wrong value of parameter " + parameter.GetName() + ".\n" + "Read: " + read + ".\n" + "Expected: " + parameter.GetFullDescription() + "\n" + reason, cause)
        {
        }

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param parameter the parameter that has a wrong value
         * @param read the value of the parameter read by the option handler
         * @param reason detailed error description
         */
        public WrongParameterValueException(IParameter parameter, String read, String reason) :
            this("Wrong value of parameter " + parameter.GetName() + ".\n" + "Read: " + read + ".\n" + "Expected: " + parameter.GetFullDescription() + "\n" + reason)
        {
        }

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param message detail message
         */
        public WrongParameterValueException(String message) :
            base(message)
        {
        }

        /**
         * Thrown by a Parameterizable object in case of wrong parameter format.
         * 
         * @param message detail message
         * @param e cause
         */
        public WrongParameterValueException(String message, Exception e) : base(message, e) { }
    }
}

