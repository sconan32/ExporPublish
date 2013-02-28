using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Constraints
{
    
/**
 * Class storing a number of very common constraints.
 * 
 * @author Erich Schubert
 */
public sealed class CommonConstraints {
  /**
   * Integer constraint: >= -1
   */
  public static  IParameterConstraint GREATER_EQUAL_MINUSONE_INT = new GreaterEqualConstraint<int>(-1);

  /**
   * Not negative.
   */
  public static  IParameterConstraint GREATER_EQUAL_ZERO_INT = new GreaterEqualConstraint<int>(0);

  /**
   * Larger than zero.
   */
  public static  IParameterConstraint GREATER_EQUAL_ONE_INT = new GreaterEqualConstraint<int>(1);

  /**
   * Larger than one.
   */
  public static  IParameterConstraint  GREATER_THAN_ONE_INT = new GreaterConstraint<int>(1);

  /**
   * Not negative.
   */
  public static  IParameterConstraint GREATER_EQUAL_ZERO_DOUBLE = new GreaterEqualConstraint<double>(0.0);

  /**
   * Larger than zero.
   */
  public static  IParameterConstraint GREATER_THAN_ZERO_DOUBLE = new GreaterConstraint<double>(0.0);

  /**
   * Constraint: less than .5
   */
  public static  IParameterConstraint LESS_THAN_HALF_DOUBLE = new LessConstraint<double>(.5);

  /**
   * At least 1.
   */
  public static  IParameterConstraint GREATER_EQUAL_ONE_DOUBLE = new GreaterEqualConstraint<double>(1.0);

  /**
   * Larger than one.
   */
  public static  IParameterConstraint GREATER_THAN_ONE_DOUBLE = new GreaterConstraint<double>(1.0);

  /**
   * Less than one.
   */
  //public static  IParameterConstraint LESS_THAN_ONE_DOUBLE = new LessConstraint<double>(1.0);

  /**
   * Less or equal than one.
   */
  public static  IParameterConstraint LESS_EQUAL_ONE_DOUBLE = new LessEqualConstraint<double>(1.0);

  /**
   * Constraint for the whole list.
   */
  //public static  IParameterConstraint GREATER_EQUAL_ZERO_INT_LIST = new ListEachConstraint<int>(GREATER_EQUAL_ZERO_INT);

  /**
   * List constraint: >= 1
   */
 // public static  IParameterConstraint GREATER_EQUAL_ONE_INT_LIST = new ListEachConstraint<int>(GREATER_EQUAL_ONE_INT);
}

}
