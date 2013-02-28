﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class GreaterConstraint<T> : AbstractNumberConstraint<T> where T:IComparable
    {
        /**
         * Creates a Greater-Than-Number parameter constraint.
         * <p/>
         * That is, the value of the number
         * parameter has to be greater than the given constraint value.
         *
         * @param constraintValue the constraint value
         */
        public GreaterConstraint(T constraintValue) :
            base(constraintValue)
        {
        }

        /**
         * Checks if the number value given by the number parameter is greater than
         * the constraint value. If not, a parameter exception is thrown.
         *
         */

        public override void Test(T t)
        {
            if (t.CompareTo(constraintValue) <= 0)
            {
                throw new WrongParameterValueException("Parameter Constraint Error:\n"
                    + "The parameter value specified has to be greater than "
                    + constraintValue.ToString() +
                    ". (current value: " + t + ")\n");
            }
        }


        public override String GetDescription(String parameterName)
        {
            return parameterName + " > " + constraintValue;
        }
    }
}
