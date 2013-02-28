using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options
{
    public abstract class ParameterException : Exception
    {
        protected ParameterException(String message)
            : base(message)
        {
        }

        protected ParameterException(String message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
