using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class OneMustBeSetGlobalConstraint : IGlobalParameterConstraint
    {
        /**
         * List of parameters to be checked.
         */
        private List<IParameter> parameters;

        /**
         * Creates a One-Must-Be-Set global parameter constraint. That is, at least
         * one parameter value of the given list of parameters has to be set.
         * 
         * @param params list of parameters
         */
        public OneMustBeSetGlobalConstraint(List<IParameter> param)
        {
            parameters = param;
        }

        /**
         * Checks if at least one parameter value of the list of parameters specified
         * is set. If not, a parameter exception is thrown.
         * 
         */

        public void Test()
        {
            foreach (IParameter p in parameters)
            {
                if (p.IsDefined())
                {
                    return;
                }
            }
            throw new WrongParameterValueException("Global Parameter Constraint Error.\n" +
                "At least one of the parameters " + OptionUtil.OptionsNamesToString(parameters) + 
                " has to be set.");
        }


        public String Description
        {
            get
            {
                return "At least one of the parameters " + OptionUtil.OptionsNamesToString(parameters) +
                " has to be set.";
            }
        }
    }
}
