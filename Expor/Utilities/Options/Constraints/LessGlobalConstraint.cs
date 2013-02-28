using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{
    public class LessGlobalConstraint<T> : IGlobalParameterConstraint
    where T :struct, IComparable<T>, IEquatable<T>
    {
        /**
         * First number parameter.
         */
        private ValueTypeParameter<T> first;

        /**
         * Second number parameter.
         */
        private ValueTypeParameter<T> second;

        /**
         * Creates a Less-Than global parameter constraint. That is the value of the
         * first number parameter given has to be less than the value of the second
         * number parameter given.
         * 
         * @param first first number parameter
         * @param second second number parameter
         */
        public LessGlobalConstraint(ValueTypeParameter<T> first, ValueTypeParameter<T> second)
        {
            this.first = first;
            this.second = second;
        }

        /**
         * Checks if the value of the first number parameter is less than the value of
         * the second number parameter. If not, a parameter exception is thrown.
         * 
         */

        public void Test()
        {
            if (first.IsDefined() && second.IsDefined())
            {
                if (first.GetValue().CompareTo(second.GetValue()) >= 0)
                {
                    throw new WrongParameterValueException(
                        "Global Parameter Constraint Error: \n" +
                        "The value of parameter \"" + first.GetName() +
                        "\" has to be less than the" + "value of parameter \"" + second.GetName() +
                        "\"" + "(Current values: " + first.GetName() +
                        ": " + first.GetValue() + ", " + second.GetName() + ": " + second.GetValue() + ")\n");
                }
            }
        }


        public String Description
        {
            get { return first.GetName() + " < " + second.GetName(); }
        }
    }
}
