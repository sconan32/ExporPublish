using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class EnumParameter : Parameter<Enum>
    {

        /**
         * Reference to the actual enum type, for T.valueOf().
         */
        protected Type enumClass;

        /**
         * Constructs an enum parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param defaultValue the default value of the parameter
         */
        public EnumParameter(OptionDescription optionID, Type enumClass, Enum defaultValue) :
            base(optionID, defaultValue)
        {
            this.enumClass = enumClass;
        }

        /**
         * default value.
         * 
         * @param optionID the unique id of the parameter
         * @param optional ParameterFlag to signal an optional parameter.
         */
        public EnumParameter(OptionDescription optionID, Type enumClass, bool optional) :
            base(optionID, optional)
        {
            this.enumClass = enumClass;
        }

        /**
         * Constructs an enum parameter with the given optionID, constraints and
         * default value.
         * 
         * @param optionID the unique id of the parameter
         */
        public EnumParameter(OptionDescription optionID, Type enumClass) :
            base(optionID)
        {
            this.enumClass = enumClass;
        }


        public override String GetSyntax()
        {
            return "<" + JoinEnumNames(" | ") + ">";
        }


        protected override Enum ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter \"" + GetName() + "\": Null value given!");
            }
            if (obj is String)
            {
                try
                {
                    return (Enum)Enum.Parse(enumClass, (String)obj);
                }
                catch (ArgumentException )
                {
                    throw new WrongParameterValueException("Enum parameter " + GetName() + " is invalid (must be one of [" +
                        JoinEnumNames(", ") + "].");
                }
            }
            throw new WrongParameterValueException("Enum parameter " + GetName() + " is not given as a string.");
        }


        public override String GetValueAsString()
        {
            return GetValue().ToString();
        }

        /**
         * Get a list of possible values for this enum parameter.
         * 
         * @return list of strings representing possible enum values.
         */
        public ICollection<String> GetPossibleValues()
        {
            // Convert to string array
            Array enums = Enum.GetValues(enumClass);
            List<String> values = new List<String>(enums.Length);
            foreach (Enum t in enums)
            {
                values.Add(t.ToString());
            }
            return values;
        }

        /**
         * Utility method for merging possible values into a string for informational
         * messages.
         * 
         * @param separator char sequence to use as a separator for enum values.
         * @return <code>{VAL1}{separator}{VAL2}{separator}...</code>
         */
        private String JoinEnumNames(String separator)
        {
            Array enumTypes = Enum.GetValues(enumClass);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < enumTypes.Length; ++i)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(enumTypes.GetValue(i).ToString());
            }
            return sb.ToString();
        }

    }

}
