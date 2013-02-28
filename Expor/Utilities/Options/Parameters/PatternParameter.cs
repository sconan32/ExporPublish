using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class PatternParameter : Parameter<Regex>
    {
        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID,
            IList<IParameterConstraint> constraint, Regex defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID,
            IList<IParameterConstraint> constraint, String defaultValue) :
            base(optionID, constraint, new Regex(defaultValue))
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraints parameter constraint
         * @param optional Flag to signal an optional parameter.
         */
        public PatternParameter(OptionDescription optionID,
            IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraints parameter constraint
         */
        public PatternParameter(OptionDescription optionID, List<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID,
            IParameterConstraint constraint, Regex defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID,
            IParameterConstraint constraint, String defaultValue) :
            base(optionID, constraint, new Regex(defaultValue))
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         * @param optional Flag to signal an optional parameter.
         */
        public PatternParameter(OptionDescription optionID,
            IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param constraint parameter constraint
         */
        public PatternParameter(OptionDescription optionID,
            IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, and default value.
         * 
         * @param optionID the unique id of the parameter
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID, Regex defaultValue) :
            base(optionID, defaultValue)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID, and default value.
         * 
         * @param optionID the unique id of the parameter
         * @param defaultValue the default value of the parameter
         */
        public PatternParameter(OptionDescription optionID, String defaultValue) :
            base(optionID, new Regex(defaultValue))
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID.
         * 
         * @param optionID the unique id of the parameter
         * @param optional Flag to signal an optional parameter.
         */
        public PatternParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs a pattern parameter with the given optionID.
         * 
         * @param optionID the unique id of the parameter
         */
        public PatternParameter(OptionDescription optionID) :
            base(optionID)
        {
        }


        public override String GetValueAsString()
        {
            return GetValue().ToString();
        }


        protected override Regex ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter \"" + GetName() + "\": Null value given!");
            }
            if (obj is Regex)
            {
                return (Regex)obj;
            }
            if (obj is String)
            {
                try
                {
                    return new Regex((String)obj);
                }
                catch (Exception )
                {
                    throw new WrongParameterValueException("Given pattern \"" + obj + "\" for parameter \"" + GetName() + "\" is no valid regular expression!");
                }
            }
            throw new WrongParameterValueException("Given pattern \"" + obj + "\" for parameter \"" + GetName() + "\" is of unknown type!");
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;pattern&gt;&quot;
         */

        public String getSyntax()
        {
            return "<pattern>";
        }

       
    }

}
