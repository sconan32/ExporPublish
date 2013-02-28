using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Utilities.Options.Constraints;

namespace Socona.Expor.Utilities.Options.Parameters
{
   
public abstract class ListParameter<T> : Parameter<IList<T>> {
  /**
   * A pattern defining a &quot,&quot.
   */
  public static readonly Regex SPLIT = new Regex (",");

  /**
   * List separator character - &quot;:&quot;
   */
  public static readonly String LIST_SEP = ",";

  /**
   * A pattern defining a &quot:&quot.
   */
  public static readonly Regex VECTOR_SPLIT = new Regex (":");
  
  /**
   * Vector separator character - &quot;:&quot;
   */
  public static readonly String VECTOR_SEP = ":";

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraints the constraints of this parameter, may be null
   * @param defaultValue the default value of this parameter (may be null)
   */
  public ListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, IList<T> defaultValue) :
    base(optionID, constraints, defaultValue){
  }

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraints the constraints of this parameter, may be null
   * @param optional specifies if this parameter is an optional parameter
   */
  public ListParameter(OptionDescription optionID, IList<IParameterConstraint> constraints, bool optional) :base(optionID, constraints, optional)
  {
  }

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraints the constraint of this parameter
   */
  // NOTE: we cannot have this, because it has the same erasure as optionID, defaults!
  // Use optional=false!
  /*public ListParameter(OptionDescription optionID, List<ParameterConstraint<List<T>>> constraints) {
    super(optionID, constraints);
  }*/

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter, may be null
   * @param defaultValue the default value of this parameter (may be null)
   */
  public ListParameter(OptionDescription optionID, IParameterConstraint constraint, IList<T> defaultValue) 
    :base(optionID, constraint, defaultValue){
  }

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter, may be null
   * @param optional specifies if this parameter is an optional parameter
   */
  public ListParameter(OptionDescription optionID, IParameterConstraint constraint, bool optional) :base(optionID, constraint, optional)
  {
  }

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   * @param constraint the constraint of this parameter
   */
  public ListParameter(OptionDescription optionID, IParameterConstraint constraint) :base(optionID, constraint)
  {
  }

  /**
   * Constructs a list parameter with the given optionID:
   * 
   * @param optionID the unique id of this parameter
   * @param defaultValue the default value of this parameter (may be null)
   */
  // NOTE: we cannot have this, because it has the same erasure as optionID, defaults!
  // Use full constructor, constraints = null!
  /*public ListParameter(OptionDescription optionID, List<T> defaultValue) {
    super(optionID, defaultValue);
  }*/

  /**
   * Constructs a list parameter with the given optionID and optional flag.
   * 
   * @param optionID the unique id of this parameter
   * @param optional Optional flag
   */
  public ListParameter(OptionDescription optionID, bool optional) :base(optionID, optional){
  }

  /**
   * Constructs a list parameter with the given optionID.
   * 
   * @param optionID the unique id of this parameter
   */
  public ListParameter(OptionDescription optionID) :base(optionID){
  }

  /**
   * Returns the size of this list parameter.
   * 
   * @return the size of this list parameter.
   */
  public int GetListSize() {
    if(GetValue() == null && IsOptional()) {
      return 0;
    }

    return GetValue().Count;
  }

  /**
   * Returns a string representation of this list parameter. The elements of
   * this list parameters are given in &quot;[ ]&quot;, comma separated.
   */
  // TODO: keep? remove?
  protected  String AsString() {
    if(GetValue() == null) {
      return "";
    }
    StringBuilder buffer = new StringBuilder();
    buffer.Append("[");

    for(int i = 0; i < GetValue().Count; i++) {
      buffer.Append(GetValue()[i].ToString());
      if(i != GetValue().Count - 1) {
        buffer.Append(",");
      }
    }
    buffer.Append("]");
    return buffer.ToString();
  }
}

}
