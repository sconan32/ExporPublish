using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class LongParameter : ValueTypeParameter<long>
    {
        /**
         * Constructs a long parameter with the given optionID, parameter constraint
         * and default value.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraints the parameter constraints for this long parameter
         * @param defaultValue the default value
         */
        public LongParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, long defaultValue) :
            base(optionID, constraints, defaultValue)
        {
        }

        /**
         * Constructs a long parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraints the parameter constraints for this long parameter
         * @param optional optional flag
         */
        public LongParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs a long parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraints the parameter constraints for this long parameter
         */
        public LongParameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
        }

        /**
         * Constructs a long parameter with the given optionID, parameter constraint
         * and default value.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraint the parameter constraint for this long parameter
         * @param defaultValue the default value
         */
        public LongParameter(OptionDescription optionID, IParameterConstraint constraint, long defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a long parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraint the parameter constraint for this long parameter
         * @param optional optional flag
         */
        public LongParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs a long parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param constraint the parameter constraint for this long parameter
         */
        public LongParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs a long parameter with the given optionID and default value.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param defaultValue the default value
         */
        public LongParameter(OptionDescription optionID, long defaultValue) :
            base(optionID, defaultValue)
        {
        }

        /**
         * Constructs a long parameter with the given optionID.
         * 
         * @param optionID the unique OptionDescription for this parameter
         * @param optional optional flag
         */
        public LongParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs a long parameter with the given optionID.
         * 
         * @param optionID the unique OptionDescription for this parameter
         */
        public LongParameter(OptionDescription optionID) :
            base(optionID)
        {
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            return (GetValue().ToString());
        }

        /** {@inheritDoc} */

        protected override long ParseValue(Object obj)
        {
            if (obj is long)
            {
                return (long)obj;
            }
            if (obj is int)
            {
                return (long)obj;
            }
            try
            {
                return long.Parse(obj.ToString());
            }
            catch (NullReferenceException )
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a double value, read: " + obj + "!\n");
            }
            catch (FormatException )
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a double value, read: " + obj + "!\n");
            }
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;long&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<long>";
        }
    }

}
