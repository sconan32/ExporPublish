using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class DoubleListParameter : ListParameter<Double>
    {
        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraints Constraints
         * @param defaultValue Default value
         */
        public DoubleListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, IList<Double> defaultValue) :
            base(optionID, constraints, defaultValue)
        {
        }

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraints Constraints
         * @param optional Optional flag
         */
        public DoubleListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraints Constraints
         */
        /*
         * public DoubleListParameter(OptionDescription optionID,
         * List<ParameterConstraint<List<Double>>> constraints) { base(optionID,
         * constraints); }
         */

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraint Constraint
         * @param defaultValue Default value
         */
        public DoubleListParameter(OptionDescription optionID, IParameterConstraint constraint, IList<Double> defaultValue) :
            base(optionID, constraint, defaultValue)
        {
        }

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraint Constraint
         * @param optional Optional flag
         */
        public DoubleListParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         * @param constraint Constraint
         */
        public DoubleListParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs a list parameter with the given optionID and optional flag.
         * 
         * @param optionID Option ID
         * @param optional Optional flag
         */
        public DoubleListParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs a list parameter with the given optionID.
         * 
         * @param optionID Option ID
         */
        public DoubleListParameter(OptionDescription optionID) :
            base(optionID)
        {
        }


        public override String GetValueAsString()
        {
            return FormatUtil.Format(GetValue().ToArray(), LIST_SEP, FormatUtil.NF);
        }



        protected override IList<Double> ParseValue(Object obj)
        {
            try
            {
                List<Double> l = (List<double>)(obj);
                // do extra validation:
                foreach (double o in l)
                {
                    //if (!(o is Double))
                    //{
                    //    throw new WrongParameterValueException("Wrong parameter format for parameter \"" + GetName() + "\". Given list contains objects of different type!");
                    //}
                }
                // TODO: can we use reflection to Get extra checks?
                // TODO: Should we copy the list?
                return (List<Double>)l;
            }
            catch (InvalidCastException )
            {
                // continue with others
            }
            if (obj is String)
            {
                String[] values = SPLIT.Split((String)obj);
                List<Double> doubleValue = new List<Double>(values.Length);
                foreach (String val in values)
                {
                    doubleValue.Add(Double.Parse(val));
                }
                return doubleValue;
            }
            throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() +
                "\" requires a list of Double values!");
        }

        /**
         * Sets the default value of this parameter.
         * 
         * @param allListDefaultValue default value for all list elements of this
         *        parameter
         */
        // Unused?
        /*public void setDefaultValue(double allListDefaultValue) {
          for(int i = 0; i < defaultValue.Count; i++) {
            defaultValue.set(i, allListDefaultValue);
          }
        }*/

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;double_1,...,double_n&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<double_1,...,double_n>";
        }
    }

}
