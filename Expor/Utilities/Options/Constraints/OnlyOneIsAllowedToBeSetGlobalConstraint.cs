using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class OnlyOneIsAllowedToBeSetGlobalConstraint : IGlobalParameterConstraint
    {
        /**
         * List of parameters to be checked.
         */
        private List<IParameter> parameters;

        /**
         * Constructs a global parameter constraint for testing if only one parameter
         * of a list of parameters is set.
         * 
         * @param params list of parameters to be checked
         */
        public OnlyOneIsAllowedToBeSetGlobalConstraint(List<IParameter> param)
        {
            parameters = param;
        }

        /**
         * Checks if only one parameter of a list of parameters is set. If not, a
         * parameter exception is thrown.
         * 
         */

        public void Test()
        {
            List<String> set = new List<String>();
            foreach (IParameter p in parameters)
            {
                if (p.IsDefined())
                {
                    // FIXME: Retire the use of this constraint for Flags!
                    if (p is BoolParameter)
                    {
                        if (((BoolParameter)p).GetValue())
                        {
                            set.Add(p.GetName());
                        }
                    }
                    else
                    {
                        set.Add(p.GetName());
                    }
                }
            }
            if (set.Count > 1)
            {
                throw new WrongParameterValueException("Global Parameter Constraint Error.\n" +
                    "Only one of the parameters " +
                    OptionUtil.OptionsNamesToString(parameters) + " is allowed to be set. " +
                    "Parameters currently set: " + set.ToString());
            }
        }


        public String Description
        {
            get
            {
                return "Only one of the parameters " + OptionUtil.OptionsNamesToString(parameters) +
                " is allowed to be set.";
            }
        }
    }
}
