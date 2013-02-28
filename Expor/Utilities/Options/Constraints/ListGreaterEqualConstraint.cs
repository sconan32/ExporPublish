using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class ListGreaterEqualConstraint<N> : AbstractNumberConstraint<IList<N>>
    where N : IComparable<N>
    {
        /**
         * Creates a Greater-Equal-Than-Number parameter constraint.
         * <p/>
         * That is, all values of the list parameter
         * tested have to be greater than or equal to the specified constraint value.
         *
         * @param constraintValue parameter constraint value
         */
        public ListGreaterEqualConstraint(N constraintValue) :
            base(new List<N> { constraintValue })
        {
        }

        /**
         * Checks if all number values of the specified list parameter
         * are greater than or equal to the constraint value.
         * If not, a parameter exception is thrown.
         *
         */

        public override void Test(IList<N> t)
        {
            foreach (N n in t)
            {
                if (n.CompareTo(constraintValue[0]) < 0)
                {
                    throw new WrongParameterValueException("Parameter Constraint Error: \n"
                        + "The parameter values specified have to be greater than or equal to " +
                        constraintValue.ToString()
                        + ". (current value: " + t + ")\n");
                }
            }
        }


        public override String GetDescription(String parameterName)
        {
            return "all elements of " + parameterName + " < " + constraintValue;
        }

    }

}
