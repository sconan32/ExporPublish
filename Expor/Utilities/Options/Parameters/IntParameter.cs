using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class IntParameter : ValueTypeParameter<Int32>
    {
        /**
         * Constructs an integer parameter with the given optionID, parameter
         * constraint, and default value.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraints the constraint for this integer parameter
         * @param defaultValue the default value
         */
        public IntParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, Int32 defaultValue) :
            base(optionID, constraints, defaultValue)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID, parameter
         * constraint, and optional flag.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraints the constraint for this integer parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraints the constraint for this integer parameter
         */
        public IntParameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID, parameter
         * constraint, and default value.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraint the constraint for this integer parameter
         * @param defaultValue the default value
         */
        public IntParameter(OptionDescription optionID, IParameterConstraint constraint, Int32 defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID, parameter
         * constraint, and optional flag.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraint the constraint for this integer parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID optionID the unique id of the option
         * @param constraint the constraint for this integer parameter
         */
        public IntParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID.
         * 
         * @param optionID optionID the unique id of the option
         * @param defaultValue the default value
         */
        public IntParameter(OptionDescription optionID, Int32 defaultValue) :
            base(optionID, defaultValue)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID.
         * 
         * @param optionID optionID the unique id of the option
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs an integer parameter with the given optionID.
         * 
         * @param optionID optionID the unique id of the option
         */
        public IntParameter(OptionDescription optionID) :
            base(optionID)
        {
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            return GetValue().ToString();
        }

        /** {@inheritDoc} */

        protected override Int32 ParseValue(Object obj)
        {
            if (obj is Int32)
            {
                return (Int32)obj;
            }
            try
            {
                return Int32.Parse(obj.ToString());
            }
            catch (NullReferenceException )
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires an integer value, read: " + obj + "!\n");
            }
            catch (FormatException )
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires an integer value, read: " + obj + "!\n");
            }
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;int&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<int>";
        }
    }

}
