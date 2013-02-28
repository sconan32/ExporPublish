using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;


namespace Socona.Expor.Utilities.Options.Parameters
{

    public abstract class ValueTypeParameter<T> : Parameter<T>
    where T :struct, IComparable<T>, IEquatable<T>
    {
        bool hasDefaultValue = false;
        bool valueset = false;
        /**
         * Constructs a number parameter with the given optionID, constraint, and
         * optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter
         * @param defaultValue the default value for this parameter
         */
        public ValueTypeParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, T defaultValue) :
            base(optionID, constraints,( defaultValue))
        {
            hasDefaultValue = true;
        }

        /**
         * Constructs a number parameter with the given optionID, constraint, and
         * optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraint of this parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public ValueTypeParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :
            base(optionID, constraints, optional)
        {
        }

        /**
         * Constructs a number parameter with the given optionID, and constraint.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be empty if there
         *        are no constraints
         */
        public ValueTypeParameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
        }

        /**
         * Constructs a number parameter with the given optionID, constraint, and
         * optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         * @param defaultValue the default value for this parameter
         */
        public ValueTypeParameter(OptionDescription optionID, IParameterConstraint constraint, T defaultValue) :
            base(optionID, constraint,( defaultValue))
        {
            hasDefaultValue = true;
        }

        /**
         * Constructs a number parameter with the given optionID, constraint, and
         * optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public ValueTypeParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
        }

        /**
         * Constructs a number parameter with the given optionID, and constraint.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         */
        public ValueTypeParameter(OptionDescription optionID, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
        }

        /**
         * Constructs a number parameter with the given optionID and default Value.
         * 
         * @param optionID the unique id of this parameter
         * @param defaultValue the default value for this parameter
         */
        public ValueTypeParameter(OptionDescription optionID, T defaultValue) :
            base(optionID,(defaultValue))
        {
            hasDefaultValue = true;
        }

        /**
         * Constructs a number parameter with the given optionID and optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public ValueTypeParameter(OptionDescription optionID, bool optional) :
            base(optionID, optional)
        {
        }

        /**
         * Constructs a number parameter with the given optionID.
         * 
         * @param optionID the unique id of this parameter
         */
        public ValueTypeParameter(OptionDescription optionID) :
            base(optionID)
        {
        }
        public override bool HasDefaultValue()
        {
            return hasDefaultValue;
        }
        public override bool IsDefined()
        {
            return valueset;
        }
        protected override void SetValueInternal(T val)
        {
            base.SetValueInternal(val);
            valueset = true;
        }
        protected override bool Validate(T obj)
        {
            return base.Validate(obj);
        }

       // protected abstract T ParseValueType(object obj);
    }

}
