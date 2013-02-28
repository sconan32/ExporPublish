using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public interface IParameterConstraint
    {
        /**
         * Checks if the value {@code t} of the parameter to be tested fulfills the parameter constraint.
         * If not, a parameter exception is thrown.
         *
         * @param t Value to be checked whether or not it fulfills the underlying
         *          parameter constraint.
         * @throws ParameterException if the parameter to be tested does not
         *                            fulfill the parameter constraint
         */
        void Test(object t);

        /**
         * Returns a description of this constraint.
         *
         * @param parameterName the name of the parameter this constraint is used for
         * @return a description of this constraint
         */
        String GetDescription(String parameterName);

    }

}
