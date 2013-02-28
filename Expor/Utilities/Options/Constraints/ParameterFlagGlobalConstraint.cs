using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class ParameterFlagGlobalConstraint<S> : IGlobalParameterConstraint
    {
        /**
         * Parameter possibly to be checked.
         */
        private Parameter<S> param;

        /**
         * Flag the checking of the parameter constraints is dependent on.
         */
        private BoolParameter flag;

        /**
         * Indicates at which status of the flag the parameter is to be checked.
         */
        private bool flagConstraint;

        /**
         * List of parameter constraints.
         */
        private IList<IParameterConstraint> cons;

        /**
         * Constructs a global parameter constraint specifying that the testing of the
         * parameter given for keeping the parameter constraints given is dependent on
         * the status of the flag given.
         * 
         * @param p parameter possibly to be checked
         * @param c a list of parameter constraints, if the value is null, the parameter is just tested if it is set.
         * @param f flag controlling the checking of the parameter constraints
         * @param flagConstraint indicates at which status of the flag the parameter
         *        is to be checked
         */
        public ParameterFlagGlobalConstraint(Parameter<S> p, IList<IParameterConstraint> c,
            BoolParameter f, bool flagConstraint)
        {
            param = p;
            flag = f;
            this.flagConstraint = flagConstraint;
            cons = c;
        }

        /**
         * Checks the parameter for its parameter constraints dependent on the status
         * of the given flag. If a parameter constraint is breached a parameter
         * exception is thrown.
         * 
         */

        public  void Test()
        {
            // only check constraints of param if flag is set
            if (flagConstraint == flag.GetValue())
            {
                if (cons != null)
                {
                    foreach (IParameterConstraint c in cons)
                    {
                        c.Test(param.GetValue());
                    }
                }
                else
                {
                    if (!param.IsDefined())
                    {
                        throw new UnusedParameterException("Value of parameter " + param.GetName() + " is not optional.");
                    }
                }
            }
        }


        public  String Description
        {
            get
            {
                StringBuilder description = new StringBuilder();
                if (flagConstraint)
                {
                    description.Append("If ").Append(flag.GetName());
                    description.Append(" is set, the following constraints for parameter ");
                    description.Append(param.GetName()).Append(" have to be fullfilled: ");
                    if (cons != null)
                    {
                        for (int i = 0; i < cons.Count; i++)
                        {
                            IParameterConstraint c = cons[(i)];
                            if (i > 0)
                            {
                                description.Append(", ");
                            }
                            description.Append(c.GetDescription(param.GetName()));
                        }
                    }
                    else
                    {
                        description.Append(param.GetName() + " must be set.");
                    }
                }
                return description.ToString();
            }
        }
    }
}
