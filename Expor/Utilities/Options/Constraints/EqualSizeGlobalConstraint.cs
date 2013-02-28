using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    /**
     * Global parameter constraint defining that a number of list parameters (
     * {@link de.lmu.ifi.dbs.elki.utilities.optionhandling.parameters.ListParameter}
     * ) must have equal list sizes.
     * 
     * @author Steffi Wanka
     */
    public class EqualSizeGlobalConstraint<T> : IGlobalParameterConstraint
    {
        /**
         * List parameters to be tested
         */
        private ListParameter<T>[] parameters;

        /**
         * Creates a global parameter constraint for testing if a number of list
         * parameters have equal list sizes.
         * 
         * @param params list parameters to be tested for equal list sizes
         */
        public EqualSizeGlobalConstraint(params ListParameter<T>[] param)
        {
            this.parameters = param;
        }

        /**
         * Checks if the list parameters have equal list sizes. If not, a parameter
         * exception is thrown.
         * 
         */

        public void Test()
        {
            bool first = false;
            int constraintSize = -1;

            foreach (ListParameter<T> listParam in parameters)
            {
                if (listParam.IsDefined())
                {
                    if (!first)
                    {
                        constraintSize = listParam.GetListSize();
                        first = true;
                    }
                    else if (constraintSize != listParam.GetListSize())
                    {
                        throw new WrongParameterValueException("Global constraint errror.\n" + "The list parameters " +
                            OptionUtil.OptionsNamesToString<ListParameter<T>>(parameters.ToList()) + " must have equal list sizes.");
                    }
                }
            }
        }


        public String Description
        {
            get
            {
                return "The list parameters " + OptionUtil.OptionsNamesToString<ListParameter<T>>(parameters.ToList()) + " must have equal list sizes.";
            }
        }
    }

}
