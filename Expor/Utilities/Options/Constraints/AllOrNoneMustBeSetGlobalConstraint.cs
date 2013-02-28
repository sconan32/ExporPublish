using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    /**
     * Global parameter constraint specifying that either all elements of a list of
     * parameters ({@link Parameter}) must be set, or none of them.
     * 
     * @author Steffi Wanka
     */
    public class AllOrNoneMustBeSetGlobalConstraint<T> : IGlobalParameterConstraint
    {
        /**
         * List of parameters to be checked
         */
        private Parameter<T>[] parameterList;

        /**
         * Constructs a global parameter constraint for testing if either all elements
         * of a list of parameters are set or none of them.
         * 
         * @param parameters list of parameters to be checked
         */
        public AllOrNoneMustBeSetGlobalConstraint(params Parameter<T>[] parameters)
        {
            this.parameterList = parameters;
        }

        /**
         * Checks if either all elements of a list of parameters are set, or none of
         * them. If not, a parameter exception is thrown.
         */

        public void Test()
        {

            List<String> set = new List<string>();
            List<String> notSet = new List<string>();

            foreach (Parameter<T> p in parameterList)
            {
                if (p.IsDefined())
                {
                    set.Add(p.GetName());
                }
                else
                {
                    notSet.Add(p.GetName());
                }
            }
            if (set.Count > 0 && notSet.Count > 0)
            {
                throw new WrongParameterValueException("Global Constraint Error.\n" + "Either all of the parameters " +
                    OptionUtil.OptionsNamesToString<Parameter<T>>(parameterList.ToList()) + " must be set or none of them. " + "Parameter(s) currently set: " +
                    set.ToString() + ", parameters currently " + "not set: " + notSet.ToString());
            }
        }


        public String Description
        {
            get
            {
                return "Either all of the parameters " +
                    OptionUtil.OptionsNamesToString<Parameter<T>>(parameterList.ToList()) + " must be set or none of them. ";
            }
        }
    }

}
