using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options
{
    public class InternalParameterizationErrors : ParameterException
    {

        /**
         * The errors that occurred.
         */
        private ICollection<Exception> internalErrors;

        /**
         * Constructor.
         * 
         * @param message Error message
         * @param internalErrors internal errors
         */
        public InternalParameterizationErrors(String message, ICollection<Exception> internalErrors)
            : base(message)
        {
            this.internalErrors = internalErrors;
        }

        /**
         * Constructor.
         * 
         * @param message Error message
         * @param internalError internal error
         */
        public InternalParameterizationErrors(String message, Exception internalError)
            : base(message)
        {
            List<Exception> errors = new List<Exception>(1);
            errors.Add(internalError);
            this.internalErrors = errors;
        }

        /**
         * @return the internalErrors
         */
        protected ICollection<Exception> getInternalErrors()
        {
            return internalErrors;
        }
    }
}
