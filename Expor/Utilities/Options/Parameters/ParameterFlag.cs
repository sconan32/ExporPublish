using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class BoolParameter : ValueTypeParameter<Boolean>
    {
        /**
         * Constant indicating that the flag is set.
         */
        public static String SET = "true";

        /**
         * Constant indicating that the flag is not set.
         */
        public static String NOT_SET = "false";

        /**
         * Constructs a flag object with the given optionID.
         * <p/>
         * If the flag is not set its value is &quot;false&quot;.
         * 
         * @param optionID the unique id of the option
         */
        public BoolParameter(OptionDescription optionID) :
            base(optionID)
        {
            SetOptional(true);
            SetDefaultValue(false);
        }


        protected override Boolean ParseValue(Object obj)
        {
            if (SET.Equals(obj))
            {
                return true;
            }
            if (NOT_SET.Equals(obj))
            {
                return false;
            }
            if (obj is Boolean)
            {
                return (Boolean)obj;
            }
            if (obj != null && SET.Equals(obj.ToString()))
            {
                return true;
            }
            if (obj != null && NOT_SET.Equals(obj.ToString()))
            {
                return false;
            }
            throw new WrongParameterValueException(
                "Wrong value for flag \"" + GetName() + "\". Allowed values:\n" + SET + " or " + NOT_SET);
        }

        /**
         * A flag has no syntax, since it doesn't take extra options
         */

        public override String GetSyntax()
        {
            return "<|" + SET + "|" + NOT_SET + ">";
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            return GetValue()==true ? SET : NOT_SET;
        }

        /**
         * {@inheritDoc}
         */

        protected override Boolean Validate(Boolean obj)
        {
            //if (obj == null)
            //{
            //    throw new WrongParameterValueException(
            //        "Boolean option '" + GetName() + "' got 'null' value.");
            //}
            return true;
        }

        /**
         * Convenience function using a native bool, that doesn't require error
         * handling.
         * 
         * @param val bool value
         */
        public void SetValue(Boolean val)
        {
            try
            {
                base.SetValue(val);
            }
            catch (ParameterException e)
            {
                // We're pretty sure that any Boolean is okay, so this should never be
                // reached.
                throw new AbortException("Flag did not accept bool value!", e);
            }
        }

        /**
         * Shorthand for {@code isDefined() && GetValue() == true}
         * 
         * @return true when defined and true.
         */
        public Boolean IsTrue()
        {
            return IsDefined() && GetValue();
        }

        /**
         * Shorthand for {@code isDefined() && GetValue() == false}
         * 
         * @return true when defined and true.
         */
        public Boolean IsFalse()
        {
            return IsDefined() && !GetValue();
        }
    }
}
