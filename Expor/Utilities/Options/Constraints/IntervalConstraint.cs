using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    public class IntervalConstraint<T> : IParameterConstraint where T:IComparable
    {
        /**
         * Available interval boundary types types:
         * {@link IntervalConstraint.IntervalBoundary#OPEN} denotes an open interval,
         * i.e. less than (or greater than) comparison
         * {@link IntervalConstraint.IntervalBoundary#CLOSE} denotes a closed
         * interval, i.e. an equal to or less than (or equal to or greater than)
         * comparison
         * 
         * @apiviz.exclude
         */
        public enum IntervalBoundary
        {
            /**
             * Open interval boundary
             */
            OPEN,
            /**
             * Closed interval boundary
             */
            CLOSE
        }

        /**
         * The low constraint value (left interval boundary).
         */
        private T lowConstraintValue;

        /**
         * The interval boundary for the low constraint value.
         * 
         * @see IntervalBoundary
         */
        private IntervalBoundary lowBoundary;

        /**
         * The high constraint value (right interval boundary).
         */
        private T highConstraintValue;

        /**
         * The interval boundary for the high constraint value.
         * 
         * @see IntervalBoundary
         */
        private IntervalBoundary highBoundary;

        /**
         * Creates an IntervalConstraint parameter constraint.
         * <p/>
         * That is, the value of the number parameter given has to be greater than (or
         * equal to, if specified) than the specified low constraint value and less
         * than (or equal to, if specified) than the specified high constraint value.
         * 
         * @param lowConstraintValue the low constraint value (left interval boundary)
         * @param lowBoundary the interval boundary for the low constraint value (see {@link IntervalBoundary})
         * @param highConstraintValue the high constraint value (right interval
         *        boundary)
         * @param highBoundary the interval boundary for the high constraint value
         *        (see {@link IntervalBoundary})
         */
        public IntervalConstraint(T lowConstraintValue, IntervalBoundary lowBoundary, T highConstraintValue, IntervalBoundary highBoundary)
        {
            if (lowConstraintValue .CompareTo(highConstraintValue)>=0)
            {
                throw new ArgumentException("Left interval boundary is greater than " + "or equal to right interval boundary!");
            }

            this.lowConstraintValue = lowConstraintValue;
            this.lowBoundary = lowBoundary;
            this.highConstraintValue = highConstraintValue;
            this.highBoundary = highBoundary;
        }

        /**
         * Checks if the number value given by the number parameter is greater equal
         * than the constraint value. If not, a parameter exception is thrown.
         * 
         */

        public  void Test(object o)
        {
            T t = (T)o;
            // lower value
            if (lowBoundary.Equals(IntervalBoundary.CLOSE))
            {
                if (t .CompareTo( lowConstraintValue)<0)
                {
                    throw new WrongParameterValueException("Parameter Constraint Error: \n" + 
                        "The parameter value specified has to be " + 
                        "equal to or greater than " + lowConstraintValue.ToString() + 
                        ". (current value: " + t.ToString() + ")\n");
                }
            }
            else if (lowBoundary.Equals(IntervalBoundary.OPEN))
            {
                if (t.CompareTo (lowConstraintValue)<=0)
                {
                    throw new WrongParameterValueException("Parameter Constraint Error: \n" + 
                        "The parameter value specified has to be " + 
                        "greater than " + lowConstraintValue.ToString() + 
                        ". (current value: " + t.ToString() + ")\n");
                }
            }

            // higher value
            if (highBoundary.Equals(IntervalBoundary.CLOSE))
            {
                if (t.CompareTo( highConstraintValue)>0)
                {
                    throw new WrongParameterValueException("Parameter Constraint Error: \n" + 
                        "The parameter value specified has to be " + 
                        "equal to or less than " + highConstraintValue.ToString() + 
                        ". (current value: " + t.ToString()+ ")\n");
                }
            }
            else if (highBoundary.Equals(IntervalBoundary.OPEN))
            {
                if (t .CompareTo( highConstraintValue)>=0)
                {
                    throw new WrongParameterValueException("Parameter Constraint Error: \n" +
                        "The parameter value specified has to be " + 
                        "less than " + highConstraintValue.ToString() + 
                        ". (current value: " + t.ToString() + ")\n");
                }
            }
        }


        public String GetDescription(String parameterName)
        {
            String description = parameterName + " in ";
            if (lowBoundary.Equals(IntervalBoundary.CLOSE))
            {
                description += "[";
            }
            else if (lowBoundary.Equals(IntervalBoundary.OPEN))
            {
                description += "(";
            }

            description += lowConstraintValue.ToString() + ", " + highConstraintValue;

            if (highBoundary.Equals(IntervalBoundary.CLOSE))
            {
                description += "]";
            }
            if (highBoundary.Equals(IntervalBoundary.OPEN))
            {
                description += ")";
            }
            return description;
        }
    }
}
