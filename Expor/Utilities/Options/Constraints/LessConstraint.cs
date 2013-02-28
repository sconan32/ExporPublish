using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{

    /**
     * Represents a Less-Than-Number parameter constraint. The value of the number
     * parameter ({@link NumberParameter}) tested has to be less than the specified
     * constraint value.
     * 
     * @author Steffi Wanka
     */
    public class LessConstraint<T> : AbstractNumberConstraint<T>
        where T : IComparable<T>
    {
        /**
         * Creates a Less-Than-Number parameter constraint.
         * <p/>
         * That is, the value of the number parameter tested has to be less than the
         * constraint value given.
         * 
         * @param constraintValue the constraint value
         */
        public LessConstraint(T constraintValue) :
            base(constraintValue)
        {
        }

        /**
         * Creates a Less-Than-Number parameter constraint.
         * <p/>
         * That is, the value of the number parameter tested has to be less than the
         * constraint value given.
         * 
         * @param constraintValue the constraint value
         */
        //public LessConstraint(int constraintValue) :
        //  base(Integer.valueOf(constraintValue)){
        //}

        ///**
        // * Creates a Less-Than-Number parameter constraint.
        // * <p/>
        // * That is, the value of the number parameter tested has to be less than the
        // * constraint value given.
        // * 
        // * @param constraintValue the constraint value
        // */
        //public LessConstraint(double constraintValue) :
        //  super(Double.valueOf(constraintValue));
        //}

        /**
         * Checks if the number value given by the number parameter is less than the
         * constraint value. If not, a parameter exception is thrown.
         * 
         */

        public override void Test(T t)
        {
            if (t.CompareTo(constraintValue) >= 0)
            {
                throw new WrongParameterValueException("Parameter Constraint Error: \n" + "The parameter value specified has to be less than " +
                    constraintValue.ToString() + ". (current value: " + t + ")\n");
            }
        }


        public override String GetDescription(String parameterName)
        {
            return parameterName + " < " + constraintValue;
        }

    }

}
