using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Log;

namespace Socona.Expor.Utilities.Options.Parameters
{

    /// <summary>
    /// 算法中的参数
    /// </summary>
    /// <typeparam name="T">参数的值类型</typeparam>

    public abstract class Parameter<T> : Socona.Expor.Utilities.Options.Parameters.IParameter
    {

        /**
         * The default value of the parameter (may be null).
         */
        protected T defaultValue = default(T);

        /**
         * Specifies if the default value of this parameter was taken as parameter
         * value.
         */
        private bool defaultValueTaken = false;

        /**
         * Specifies if this parameter is an optional parameter.
         */
        protected bool optionalParameter = false;

        /**
         * Holds parameter constraints for this parameter.
         */
        protected readonly IList<IParameterConstraint> constraints;

        /**
         * The option name.
         */
        protected readonly OptionDescription optionName;

        /**
         * The short description of the option. An extended description is provided by
         * the method {@link #getFullDescription()}
         */
        protected String shortDescription;

        /**
         * The value last passed to this option.
         */
        protected T givenValue = default(T);

        /**
         * The value of this option.
         */
        private T value;

        /**
         * Constructs a parameter with the given optionID, constraints, and default
         * value.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be empty if there
         *        are no constraints
         * @param defaultValue the default value of this parameter (may be null)
         */
        public Parameter(OptionDescription optionID, IList<IParameterConstraint> constraints, T defaultValue)
        {
            this.optionName = optionID;
            this.shortDescription = optionID.Description;
            this.optionalParameter = true;
            this.defaultValue = defaultValue;
            this.constraints = (constraints != null) ? constraints : new List<IParameterConstraint>();
        }

        /**
         * Constructs a parameter with the given optionID, constraints, and optional
         * flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be empty if there
         *        are no constraints
         * @param optional specifies if this parameter is an optional parameter
         */
        public Parameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional)
        {
            this.optionName = optionID;
            this.shortDescription = optionID.Description;
            this.optionalParameter = optional;
            this.defaultValue = default(T);
            this.constraints = (constraints != null) ? constraints : new List<IParameterConstraint>();
        }

        /**
         * Constructs a parameter with the given optionID, and constraints.
         * 
         * @param optionID the unique id of this parameter
         * @param constraints the constraints of this parameter, may be empty if there
         *        are no constraints
         */
        public Parameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
            this(optionID, constraints, false)
        { }

        /**
         * Constructs a parameter with the given optionID, constraint, and default
         * value.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         * @param defaultValue the default value of this parameter (may be null)
         */
        public Parameter(OptionDescription optionID, IParameterConstraint constraint, T defaultValue) :
            this(optionID, MakeConstraintsList(constraint), defaultValue)
        {
            ;
        }

        /**
         * Constructs a parameter with the given optionID, constraint, and optional
         * flag.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public Parameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
            this(optionID, MakeConstraintsList(constraint), optional)
        {
        }

        /**
         * Constructs a parameter with the given optionID, and constraint.
         * 
         * @param optionID the unique id of this parameter
         * @param constraint the constraint of this parameter
         */
        public Parameter(OptionDescription optionID, IParameterConstraint constraint) :
            this(optionID, constraint, false)
        {
        }

        /**
         * Constructs a parameter with the given optionID and default value.
         * 
         * @param optionID the unique id of the option
         * @param defaultValue default value.
         */
        public Parameter(OptionDescription optionID, T defaultValue) :
            this(optionID, (List<IParameterConstraint>)null, defaultValue)
        {
        }

        /**
         * Constructs a parameter with the given optionID and optional flag.
         * 
         * @param optionID the unique id of the option
         * @param optional optional flag
         */
        public Parameter(OptionDescription optionID, bool optional) :
            this(optionID, (IList<IParameterConstraint>)null, optional)
        {
        }

        /**
         * Constructs a parameter with the given optionID.
         * 
         * @param optionID the unique id of the option
         */
        public Parameter(OptionDescription optionID) :
            this(optionID, (List<IParameterConstraint>)null, false)
        {
        }

        /**
         * Wrap a single constraint into a vector of constraints.
         * 
         * @param <S> Type
         * @param constraint Constraint, may be {@code null}
         * @return List containing the constraint (if not null)
         */
        private static List<IParameterConstraint> MakeConstraintsList(IParameterConstraint constraint)
        {
            List<IParameterConstraint> constraints = new List<IParameterConstraint>((constraint == null) ? 0 : 1);
            if (constraint != null)
            {
                constraints.Add(constraint);
            }
            return constraints;
        }

        /**
         * Sets the default value of this parameter.
         * 
         * @param defaultValue default value of this parameter
         */
        public void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = defaultValue;
            this.optionalParameter = true;
        }

        /**
         * Checks if this parameter has a default value.
         * 
         * @return true, if this parameter has a default value, false otherwise
         */
        public virtual bool HasDefaultValue()
        {
            return !(defaultValue == null);
        }

        /**
         * Sets the default value of this parameter as the actual value of this
         * parameter.
         */
        // TODO: can we do this more elegantly?
        public void UseDefaultValue()
        {

            SetValueInternal(defaultValue);
            defaultValueTaken = true;
        }

        /**
         * Handle default values for a parameter.
         * 
         * @return Return code: {@code true} if it has a default value, {@code false}
         *         if it is optional without a default value. Exception if it is a
         *         required parameter!
         * @throws UnspecifiedParameterException If the parameter requires a value
         */
        public bool TryDefaultValue()
        {
            // Assume default value instead.
            if (HasDefaultValue())
            {
                UseDefaultValue();
                return true;
            }
            else if (IsOptional())
            {
                // Optional is fine, but not successful
                return false;
            }
            else
            {
                throw new UnspecifiedParameterException(this);
            }
        }

        /**
         * Specifies if this parameter is an optional parameter.
         * 
         * @param opt true if this parameter is optional, false otherwise
         */
        public void SetOptional(bool opt)
        {
            this.optionalParameter = opt;
        }

        /**
         * Checks if this parameter is an optional parameter.
         * 
         * @return true if this parameter is optional, false otherwise
         */
        public bool IsOptional()
        {
            return this.optionalParameter;
        }

        /**
         * Checks if the default value of this parameter was taken as the actual
         * parameter value.
         * 
         * @return true, if the default value was taken as actual parameter value,
         *         false otherwise
         */
        public bool TookDefaultValue()
        {
            return defaultValueTaken;
        }

        /**
         * Returns true if the value of the option is defined, false otherwise.
         * 
         * @return true if the value of the option is defined, false otherwise.
         */
        public virtual bool IsDefined()
        {
            return (this.value != null);
        }

        /**
         * Returns the default value of the parameter.
         * <p/>
         * If the parameter has no default value, the method returns <b>null</b>.
         * 
         * @return the default value of the parameter, <b>null</b> if the parameter
         *         has no default value.
         */
        // TODO: change this to return a string value?
        public T GetDefaultValue()
        {
            return defaultValue;
        }

        /**
         * Whether this class has a list of default values.
         * 
         * @return whether the class has a description of valid values.
         */
        public virtual bool HasValuesDescription()
        {
            return false;
        }

        /**
         * Return a string explaining valid values.
         * 
         * @return String explaining valid values (e.g. a class list)
         */
        public virtual String GetValuesDescription()
        {
            return "";
        }

        /**
         * Returns the extended description of the option which includes the option's
         * type, the short description and the default value (if specified).
         * 
         * @return the option's description.
         */
        public String GetFullDescription()
        {
            StringBuilder description = new StringBuilder();
            // description.Append(getParameterType()).Append(" ");
            description.Append(shortDescription);
            description.Append(FormatUtil.NEWLINE);
            if (HasValuesDescription())
            {
                String valuesDescription = GetValuesDescription();
                description.Append(valuesDescription);
                if (!valuesDescription.EndsWith(FormatUtil.NEWLINE))
                {
                    description.Append(FormatUtil.NEWLINE);
                }
            }
            if (HasDefaultValue())
            {
                description.Append("Default: ");
                description.Append(GetDefaultValueAsString());
                description.Append(FormatUtil.NEWLINE);
            }
            if (constraints.Count > 0)
            {
                if (constraints.Count == 1)
                {
                    description.Append("Constraint: ");
                }
                else if (constraints.Count > 1)
                {
                    description.Append("Constraints: ");
                }
                for (int i = 0; i < constraints.Count; i++)
                {
                    IParameterConstraint constraint = constraints[i];
                    if (i > 0)
                    {
                        description.Append(", ");
                    }
                    description.Append(constraint.GetDescription(GetName()));
                    if (i == constraints.Count - 1)
                    {
                        description.Append(".");
                    }
                }
                description.Append(FormatUtil.NEWLINE);
            }
            return description.ToString();
        }

        /**
         * Validate a value after parsing (e.g. do constrain checks!)
         * 
         * @param obj Object to validate
         * @return true iff the object is valid for this parameter.
         * @throws ParameterException when the object is not valid.
         */
        protected virtual bool Validate(T obj)
        {
            try
            {
                foreach (IParameterConstraint cons in this.constraints)
                {
                    cons.Test(obj);
                }
            }
            catch (ParameterException e)
            {
                throw new WrongParameterValueException("Specified parameter value for parameter \"" + GetName() +
                    "\" breaches parameter constraint.\n" + e.Message);
            }
            return true;
        }

        /**
         * Return the OptionDescription of this option.
         * 
         * @return Option ID
         */
        public OptionDescription GetOptionDescription()
        {
            return optionName;
        }

        /**
         * Returns the name of the option.
         * 
         * @return the option's name.
         */
        public String GetName()
        {
            return optionName.Name;
        }

        /// <summary>
        /// get or set the short description of the option
        /// </summary>
        public string ShortDescription
        {
            get { return shortDescription; }
            set { shortDescription = value; }
        }


        /**
         * Sets the value of the option.
         * 
         * @param obj the option's value to be set
         * @throws ParameterException if the given value is not a valid value for this
         *         option.
         */
        public virtual void SetValue(Object obj)
        {
            T val = ParseValue(obj);
            if (Validate(val))
            {
                SetValueInternal(val);
            }
            else
            {
                throw new ArgumentException("Value for option \"" + GetName() + "\" did not validate: " + obj.ToString());
            }
        }

        /**
         * Internal setter for the value.
         * 
         * @param val Value
         */
        protected virtual void SetValueInternal(T val)
        {
            this.value = this.givenValue = val;
        }

        /**
         * Returns the value of the option.
         * 
         * You should use either {@link de.lmu.ifi.dbs.elki.utilities.optionhandling.parameterization.Parameterization#grab}
         * or {@link #isDefined} to test if getValue() will return a well-defined value.
         * 
         * @return the option's value.
         */
        public virtual T GetValue()
        {
            if (this.value == null)
            {
                Logging.GetLogger(this.GetType()).Warning(
                    "Programming error: Parameter#getValue() called for unset parameter \"" +
                    this.optionName.Name + "\"", new Exception());
            }
            return this.value;
        }

        /**
         * Get the last given value. May return {@code null}
         * 
         * @return Given value
         */
        public virtual Object GetGivenValue()
        {
            return this.givenValue;
        }

        /**
         * Checks if the given argument is valid for this option.
         * 
         * @param obj option value to be checked
         * @return true, if the given value is valid for this option
         * @throws ParameterException if the given value is not a valid value for this
         *         option.
         */
        public bool IsValid(Object obj)
        {
            T val = ParseValue(obj);
            return Validate(val);
        }

        /**
         * Returns a string representation of the parameter's type (e.g. an
         * {@link de.lmu.ifi.dbs.elki.utilities.optionhandling.parameters.IntParameter}
         * should return {@code <int>}).
         * 
         * @return a string representation of the parameter's type
         */
        public virtual String GetSyntax() { return null; }

        /**
         * Parse a given value into the destination type.
         * 
         * @param obj Object to parse (may be a string representation!)
         * @return Parsed object
         * @throws ParameterException when the object cannot be parsed.
         */
        protected abstract T ParseValue(Object obj);

        /**
         * Get the value as string. May return {@code null}
         * 
         * @return Value as string
         */
        public virtual String GetValueAsString() { return null; }

        /**
         * Get the default value as string.
         * 
         * @return default value
         */
        public virtual String GetDefaultValueAsString()
        {
            return GetDefaultValue().ToString();
        }

        /**
         * Add an additional constraint.
         * 
         * @param constraint Constraint to add.
         */
        public void AddConstraint(IParameterConstraint constraint)
        {
            constraints.Add(constraint);
        }


        object IParameter.GetDefaultValue()
        {
            return this.GetDefaultValue();
        }

        object IParameter.GetValue()
        {
            return this.GetValue();
        }

        public void SetDefaultValue(object defaultValue)
        {
            SetDefaultValue((T)defaultValue);
        }
        public override string ToString()
        {
            return "Parameter:" + optionName.Name + " Type:" + typeof(T).Name;
        }
    }
}
