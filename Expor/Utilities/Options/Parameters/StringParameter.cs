using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class StringParameter : Parameter<String>
    {
        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public StringParameter(OptionDescription optionID,
            IList<IParameterConstraint> constraint, String defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraints parameter constraint
         * @param optional Flag to signal an optional parameter.
         */
        public StringParameter(OptionDescription optionID,
            IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraints parameter constraint
         */
        public StringParameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public StringParameter(OptionDescription optionID,
            IParameterConstraint constraint, String defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param optional Flag to signal an optional parameter.
         */
        public StringParameter(OptionDescription optionID,
            IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         */
        public StringParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs a string parameter with the given optionID, and default value.
         * 
         * @param optionID the unique id of the parameter
         * @param defaultValue the default value of the parameter
         */
        public StringParameter(OptionDescription optionID, String defaultValue) :
            base(optionID, defaultValue)
        {
        }

        /**
         * Constructs a string parameter with the given optionID.
         * 
         * @param optionID the unique id of the parameter
         * @param optional Flag to signal an optional parameter.
         */
        public StringParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs a string parameter with the given optionID.
         * 
         * @param optionID the unique id of the parameter
         */
        public StringParameter(OptionDescription optionID) :
            base(optionID)
        {
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            return GetValue();
        }

        /** {@inheritDoc} */

        protected override String ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter \"" + GetName() + "\": Null value given!");
            }
            if (obj is String)
            {
                return (String)obj;
            }
            // TODO: allow anything convertible by toString()?
            throw new WrongParameterValueException("String parameter " + GetName() + " is not a string.");
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;string&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<string>";
        }
    }

}
