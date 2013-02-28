using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Constraints
{
 
public class GreaterEqualConstraint<T> : AbstractNumberConstraint<T> where T:IComparable {
    /**
     * Creates a Greater-Equal parameter constraint.
     * <p/>
     * That is, the value of the number
     * parameter given has to be greater equal than the constraint value given.
     *
     * @param constraintValue the constraint value
     */
    public GreaterEqualConstraint(T constraintValue) :
        base(constraintValue){
    }

    /**
     * Checks if the number value given by the number parameter is
     * greater equal than the constraint
     * value. If not, a parameter exception is thrown.
     *
     */
   
    public override void Test(T t) {
        if (t.CompareTo(constraintValue)<0) {
            throw new WrongParameterValueException("Parameter Constraint Error: \n"
                + "The parameter value specified has to be greater equal than "
                + constraintValue.ToString() +
                ". (current value: " + t+ ")\n");
        }
    }

  
    public override String GetDescription(String parameterName) {
        return parameterName + " >= " + constraintValue;
    }

}

}
