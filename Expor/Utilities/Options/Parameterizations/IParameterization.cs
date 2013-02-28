using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Parameterizations
{
    public interface IParameterization
    {

        bool Grab(IParameter opt);


        bool SetValueForOption(IParameter opt);


        IList<ParameterException> GetErrors();

        void ReportError(ParameterException e);

        /**
         * Check for unused parameters
         * 
         * @return {@code true} if at least one parameter was not consumed
         */
        bool HasUnusedParameters();

        /**
         * Check a parameter constraint.
         * 
         * @param constraint Parameter constraint
         * @return test result
         */
        bool CheckConstraint(IGlobalParameterConstraint constraint);

        /**
         * Descend parameterization tree into sub-option.
         * 
         * Note: this is done automatically by a {@link ClassParameter#instantiateClass}.
         * You only need to call this when you want to expose the tree structure
         * without offering a class choice as parameter.
         * 
         * @param option Option subtree
         * @return Parameterization
         */
        IParameterization Descend(Object option);

        /**
         * Return true when there have been errors.
         * 
         * @return Success code
         */
        bool HasErrors();

        /**
         * Try to instantiate a particular class.
         * 
         * @param <C> return type
         * @param r Restriction class
         * @param c Base class
         * @return class instance or null
         */
        C TryInstantiate<C>(Type r, Type c);


        /**
         * Try to instantiate a particular class.
         * 
         * @param <C> return type
         * @param c Base class
         * @return class instance or null
         */
        C TryInstantiate<C>(Type c);
    }
}
