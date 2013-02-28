using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{
   
public class DoubleParameter :ValueTypeParameter<Double> {
  /**
   * Constructs a double parameter with the given optionID, parameter
   * constraints, and default value.
   * 
   * @param optionID the unique optionID
   * @param cons a list of parameter constraints for this double parameter
   * @param defaultValue the default value for this double parameter
   */
  public DoubleParameter(OptionDescription optionID, IList<IParameterConstraint> cons, Double defaultValue)
      :base(optionID, cons, defaultValue){
  }

  /**
   * Constructs a double parameter with the given optionID, parameter
   * constraints, and optional flag.
   * 
   * @param optionID the unique optionID
   * @param cons a list of parameter constraints for this double parameter
   * @param optional specifies whether this parameter is an optional parameter
   */
  public DoubleParameter(OptionDescription optionID, IList<IParameterConstraint> cons, bool optional) :
    this(optionID, cons){
    SetOptional(optional);
  }

  /**
   * Constructs a double parameter with the given optionID, and parameter
   * constraints.
   * 
   * @param optionID the unique optionID
   * @param constraints a list of parameter constraints for this double
   *        parameter
   */
  public DoubleParameter(OptionDescription optionID, IList<IParameterConstraint> constraints) :
   base(optionID, constraints){
  }

  /**
   * Constructs a double parameter with the given optionID, parameter
   * constraint, and default value.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter
   * @param defaultValue the default value for this parameter
   */
  public DoubleParameter(OptionDescription optionID, IParameterConstraint constraint, Double defaultValue) :
    base(optionID, constraint, defaultValue){
  }

  /**
   * Constructs a double parameter with the given optionID, parameter
   * constraint, and optional flag.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter
   * @param optional specifies whether this parameter is an optional parameter
   */
  public DoubleParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :
    base(optionID, constraint, optional){
  }

  /**
   * Constructs a double parameter with the given optionID, and parameter
   * constraint.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter
   */
  public DoubleParameter(OptionDescription optionID, IParameterConstraint constraint) :
    base(optionID, constraint){
  }

  /**
   * Constructs a double parameter with the given optionID and default value.
   * 
   * @param optionID the unique optionID
   * @param defaultValue the default value for this double parameter
   */
  public DoubleParameter(OptionDescription optionID, Double defaultValue) :base(optionID, defaultValue){
  }

  /**
   * Constructs a double parameter with the given optionID and optional flag.
   * 
   * @param optionID the unique id of this parameter
   * @param optional specifies whether this parameter is an optional parameter
   */
  public DoubleParameter(OptionDescription optionID, bool optional) :base(optionID, optional){
  }

  /**
   * Constructs a double parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   */
  public DoubleParameter(OptionDescription optionID) :base(optionID){
  }

  /** {@inheritDoc} */
  
  public override String GetValueAsString() {
    return GetValue().ToString();
  }

  /** {@inheritDoc} */
 
  protected override Double ParseValue(Object obj)  {
    if(obj is Double) {
      return (Double) obj;
    }
    try {
      return Double.Parse(obj.ToString());
    }
    catch(NullReferenceException ) {
      throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a double value, read: " + obj + "!\n");
    }
    catch(FormatException ) {
      throw new WrongParameterValueException("Wrong parameter format! Parameter \"" + GetName() + "\" requires a double value, read: " + obj + "!\n");
    }
  }

  /**
   * Indicates whether some other object is "equal to" this one.
   * 
   * @param obj the reference object with which to compare.
   * @return <code>true</code> if this double parameter has the same value as
   *         the specified object, <code>false</code> otherwise.
   */
  // TODO: comparing the parameters doesn't make sense. REMOVE.
  /*@Override
  public boolean equals(Object obj) {
    if(obj == this) {
      return true;
    }
    if(!(obj instanceof DoubleParameter)) {
      return false;
    }
    DoubleParameter oth = (DoubleParameter) obj;
    if(this.getValue() == null) {
      return (oth.getValue() == null);
    }
    return this.getValue().equals(oth.getValue());
  }*/

  /**
   * Returns a string representation of the parameter's type.
   * 
   * @return &quot;&lt;double&gt;&quot;
   */

  public override String GetSyntax() {
    return "<double>";
  }
}

}
