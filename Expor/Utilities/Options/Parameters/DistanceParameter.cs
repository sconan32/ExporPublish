using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class DistanceParameter : Parameter<IDistanceValue>
    {
        /**
         * Distance type
         */
        IDistanceValue dist;

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraints, and default value.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param cons a list of parameter constraints for this double parameter
         * @param defaultValue the default value for this double parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, List<IParameterConstraint> cons, IDistanceValue defaultValue) :
            base(optionID, cons, defaultValue)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraints, and default value.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param cons a list of parameter constraints for this double parameter
         * @param defaultValue the default value for this double parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, List<IParameterConstraint> cons, IDistanceValue defaultValue) :
            base(optionID, cons, defaultValue)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraints, and optional flag.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param cons a list of parameter constraints for this double parameter
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, List<IParameterConstraint> cons, bool optional) :
            this(optionID, dist, cons)
        {
            SetOptional(optional);
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraints, and optional flag.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param cons a list of parameter constraints for this double parameter
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, List<IParameterConstraint> cons, bool optional) :
            this(optionID, dist, cons)
        {
            SetOptional(optional);
        }

        /**
         * Constructs a double parameter with the given optionID, and parameter
         * constraints.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param constraints a list of parameter constraints for this double
         *        parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, List<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID, and parameter
         * constraints.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param constraints a list of parameter constraints for this double
         *        parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, List<IParameterConstraint> constraints) :
            base(optionID, constraints)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraint, and default value.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         * @param defaultValue the default value for this parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, IParameterConstraint constraint, IDistanceValue defaultValue) :
            base(optionID, constraint, defaultValue)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraint, and default value.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         * @param defaultValue the default value for this parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, IParameterConstraint constraint, IDistanceValue defaultValue) :
            base(optionID, constraint, defaultValue)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraint, and optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID, parameter
         * constraint, and optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, IParameterConstraint constraint, bool optional) :
            base(optionID, constraint, optional)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID, and parameter
         * constraint.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param constraint the constraint of this parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, IParameterConstraint constraint) :
            base(optionID, constraint)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID and default value.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param defaultValue the default value for this double parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, IDistanceValue defaultValue) :
            base(optionID, defaultValue)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID and default value.
         * 
         * @param optionID the unique optionID
         * @param dist distance factory
         * @param defaultValue the default value for this double parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, IDistanceValue defaultValue) :
            base(optionID, defaultValue)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID and optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist, bool optional) :
            base(optionID, optional)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID and optional flag.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         * @param optional specifies whether this parameter is an optional parameter
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist, bool optional) :
            base(optionID, optional)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }

        /**
         * Constructs a double parameter with the given optionID.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         */
        public DistanceParameter(OptionDescription optionID, IDistanceValue dist) :
            base(optionID)
        {
            this.dist = dist;
        }

        /**
         * Constructs a double parameter with the given optionID.
         * 
         * @param optionID the unique id of this parameter
         * @param dist distance factory
         */
        public DistanceParameter(OptionDescription optionID, IDistanceFunction dist) :
            base(optionID)
        {
            this.dist = (dist != null) ? dist.DistanceFactory : null;
        }


        public override String GetValueAsString()
        {
            return GetValue().ToString();
        }


        protected override IDistanceValue ParseValue(Object obj)
        {
            if (dist == null)
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a distance value, but the distance was not set!");
            }
            if (obj == null)
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a distance value, but a null value was given!");
            }
            if (dist.Empty.GetType().IsAssignableFrom(obj.GetType()))
            {
                return (IDistanceValue)obj;
            }
            try
            {
                return dist.ParseString(obj.ToString());
            }
            catch (ArgumentException)
            {
                throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a distance value, read: " + obj + "!\n");
            }
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;distance&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<distance>";
        }
    }
}
