using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class IntListParameter : ListParameter<Int32>
    {
        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be null
         * @param defaultValue the default value
         */
        public IntListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, IList<Int32> defaultValue) :
            base(optionID, constraints, defaultValue)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be null
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be null
         */
        /*public IntListParameter(OptionDescription optionID, IList<IParameterConstraint<IList<Int32>>> constraints) {
          base(optionID, constraints);
        } */

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter, may be null
         * @param defaultValue the default value
         */
        public IntListParameter(OptionDescription optionID, IParameterConstraint constraint, IList<Int32> defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter, may be null
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntListParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter, may be null
         */
        public IntListParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param defaultValue the default value
         */
        /*public IntListParameter(OptionDescription optionID, IList<Int32> defaultValue) {
          base(optionID, defaultValue);
        }*/

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public IntListParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs an integer list parameter
         * 
         * @param optionID the unique id of this parameter
         */
        public IntListParameter(OptionDescription optionID) :
            base(optionID)
        {
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            StringBuilder buf = new StringBuilder();
            IList<Int32> val = GetValue();
            for (int i = 0; i < val.Count; i++)
            {

                buf.Append(val[i]);
                if (i != val.Count - 1)
                {
                    buf.Append(LIST_SEP);
                }
            }
            return buf.ToString();
        }

        /** {@inheritDoc} */


        protected override IList<Int32> ParseValue(Object obj)
        {
            try
            {
                IList<int> l = (IList<int>)obj;
                // do extra validation:
                //foreach (int o in l)
                //{
                //    if (!(o is Int32))
                //    {
                //        throw new WrongParameterValueException<IntListParameter>("Wrong parameter format for parameter \"" + GetName() + "\". Given list contains objects of different type!");
                //    }
                //}
                // TODO: can we use reflection to get extra checks?
                // TODO: Should we copy the list?
                return (IList<Int32>)l;
            }
            catch (InvalidCastException )
            {
                // continue with others
            }
            if (obj is String)
            {
                String[] values = SPLIT.Split((String)obj);
                List<Int32> intValue = new List<Int32>(values.Length);
                foreach (String val in values)
                {
                    intValue.Add(Int32.Parse(val));
                }
                return intValue;
            }
            throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a list of Int32 values!");
        }

        /**
         * Sets the default value of this parameter.
         * 
         * @param allListDefaultValue default value for all list elements of this
         *        parameter
         */
        // unused?
        /*public void setDefaultValue(int allListDefaultValue) {
          for(int i = 0; i < defaultValue.size(); i++) {
            defaultValue.set(i, allListDefaultValue);
          }
        }*/

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;int_1,...,int_n&gt;&quot;
         */

        public String getSyntax()
        {
            return "<int_1,...,int_n>";
        }
    }

}

