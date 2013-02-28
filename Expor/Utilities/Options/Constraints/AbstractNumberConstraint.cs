using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public abstract class AbstractNumberConstraint<P> : IParameterConstraint
    {
        /**
         * The constraint value.
         */
        protected P constraintValue;

        /**
         * Creates an abstract number constraint.
         *
         * @param constraintValue the constraint value
         */
        public AbstractNumberConstraint(P constraintValue)
        {
            this.constraintValue = constraintValue;
        }

        public abstract void Test(P t);


        public abstract string GetDescription(string parameterName);


        public virtual void Test(object t)
        {
            Test((P)t);
        }
    }
}
