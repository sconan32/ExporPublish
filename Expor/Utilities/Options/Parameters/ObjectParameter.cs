using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Utilities.Options.Parameters
{
   
public class ObjectParameter<C> : ClassParameter{
  /**
   * The instance to use.
   */
  private C instance;

  /**
   * Constructs a class parameter with the given optionID, restriction class,
   * and default value.
   * 
   * @param optionID the unique id of the option
   * @param restrictionClass the restriction class of this class parameter
   * @param defaultValue the default value of this class parameter
   */
  public ObjectParameter(OptionDescription optionID,Type restrictionClass, Type defaultValue):base(optionID, restrictionClass, defaultValue)
  {
  }

  /**
   * Constructs a class parameter with the given optionID, restriction class,
   * and default value.
   * 
   * @param <T> default value type, to solve generics problems.
   * @param optionID the unique id of the option
   * @param restrictionClass the restriction class of this class parameter
   * @param defaultValue the default instance of this class parameter
   */
  public  ObjectParameter(OptionDescription optionID, Type restrictionClass, C defaultValue) :
   base(optionID, restrictionClass){
    this.instance = defaultValue;
  }

  /**
   * Constructs a class parameter with the given optionID, restriction class,
   * and optional flag.
   * 
   * @param optionID the unique id of the option
   * @param restrictionClass the restriction class of this class parameter
   * @param optional specifies if this parameter is an optional parameter
   */
  public ObjectParameter(OptionDescription optionID, Type restrictionClass, bool optional) :
    base(optionID, restrictionClass, optional){
  }

  /**
   * Constructs a class parameter with the given optionID, and restriction
   * class.
   * 
   * @param optionID the unique id of the option
   * @param restrictionClass the restriction class of this class parameter
   */
  public ObjectParameter(OptionDescription optionID, Type restrictionClass) :base(optionID, restrictionClass){
    // It would be nice to be able to use Class<C> here, but this won't work
    // with nested Generics:
    // * ClassParameter<Foo<Bar>>(optionID, Foo.class) doesn't satisfy Class<C>
    // * ClassParameter<Foo<Bar>>(optionID, Foo<Bar>.class) isn't valid
    // * ClassParameter<Foo<Bar>>(optionID, (Class<Foo<Bar>>) Foo.class) is an
    // invalid cast.
    
  }

  /** {@inheritDoc} */

 
  protected override Type ParseValue(Object obj) {
    if(obj == null) {
      throw new UnspecifiedParameterException("Parameter Error.\n" + "No value for parameter \"" + GetName() + "\" " + "given.");
    }
    // does the given objects class fit?
    if(restrictionClass.IsInstanceOfType(obj)) {
      return obj.GetType();
    }
    return base.ParseValue(obj);
  }

  /** {@inheritDoc} */

  public override void SetValue(Object obj) {
    // This is a bit hackish. But when given an appropriate instance, keep it.
    if(restrictionClass.IsInstanceOfType(obj)) {
      instance = (C) obj;
    }
    base.SetValue(obj);
  }

  /**
   * Returns a string representation of the parameter's type.
   * 
   * @return &quot;&lt;class&gt;&quot;
   */

  public override String GetSyntax() {
    return "<class|object>";
  }

  /**
   * Returns a new instance for the value (i.e., the class name) of this class
   * parameter. The instance has the type of the restriction class of this class
   * parameter.
   * <p/>
   * If the Class for the class name is not found, the instantiation is tried
   * using the package of the restriction class as package of the class name.
   * 
   * @param config Parameterization
   * @return a new instance for the value of this class parameter
   */
  
  public  C InstantiateClass(IParameterization config) {
    if(instance != null) {
      return instance;
    }
    // NOTE: instance may be null here, when instantiateClass failed.
    return instance = base.InstantiateClass<C>(config);
  }

  /** {@inheritDoc} */
 
  public override Object GetGivenValue() {
    if(instance != null) {
      return instance;
    }
    else {
      return base.GetGivenValue();
    }
  }
}
}
