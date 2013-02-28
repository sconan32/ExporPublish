using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Constraints
{
    public interface IGlobalParameterConstraint
    {

        /**
         * Checks if the respective parameters satisfy the parameter constraint. If not,
         * a parameter exception is thrown.
         * 
         * @throws ParameterException if the parameters don't satisfy the parameter constraint.
         */
        void Test();

        /**
         * Returns a description of this global constraint.
         *
         * @return a description of this global constraint
         */
        String Description { get; }

    }

}
