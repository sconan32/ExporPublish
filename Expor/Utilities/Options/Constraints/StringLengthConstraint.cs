using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class StringLengthConstraint : IParameterConstraint
    {
        /**
         * Minimum length
         */
        int minlength;

        /**
         * Maximum length
         */
        int maxlength;

        /**
         * Constructor with minimum and maximum length.
         * 
         * @param minlength Minimum length, may be 0 for no limit
         * @param maxlength Maximum length, may be -1 for no limit
         */
        public StringLengthConstraint(int minlength, int maxlength) :
            base()
        {
            this.minlength = minlength;
            this.maxlength = maxlength;
        }

        /**
         * Checks if the given string value of the string parameter is within the
         * length restrictions. If not, a parameter exception is thrown.
         */

        public void Test(String t)
        {
            if (t.Length < minlength)
            {
                throw new WrongParameterValueException("Parameter Constraint Error.\n" +
                    "Parameter value length must be at least " + minlength + ".");
            }
            if (maxlength > 0 && t.Length > maxlength)
            {
                throw new WrongParameterValueException("Parameter Constraint Error.\n" +
                    "Parameter value length must be at most " + maxlength + ".");
            }
        }


        public String GetDescription(String parameterName)
        {
            if (maxlength > 0)
            {
                return parameterName + " has length " + minlength + " to " + maxlength + ".";
            }
            else
            {
                return parameterName + " has length of at least " + minlength + ".";
            }
        }

        public void Test(object t)
        {
            throw new NotImplementedException();
        }
    }

}
