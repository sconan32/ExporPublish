using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.Options
{
   public interface Parameterization {
  
  public bool Grab(Parameter<?,?> opt);

 
  public bool setValueForOption(Parameter<?,?> opt);

 
  public IList<ParameterException> getErrors();

  public void ReportError(ParameterException e);

  /**
   * Check for unused parameters
   * 
   * @return {@code true} if at least one parameter was not consumed
   */
  public bool hasUnusedParameters();

  /**
   * Check a parameter constraint.
   * 
   * @param constraint Parameter constraint
   * @return test result
   */
  public bool CheckConstraint(GlobalParameterConstraint constraint);
  
  /**
   * Descend parameterization tree into sub-option.
   * 
   * Note: this is done automatically by a {@link ClassParameter#instantiateClass}.
   * You only need to call this when you want to expose the tree structure
   * without offering a class choice as parameter.
   * 
   * @param option Option subtree
   * @return Parameterization
   */
  public Parameterization descend(Object option);

  /**
   * Return true when there have been errors.
   * 
   * @return Success code
   */
  public bool HasErrors();

  /**
   * Try to instantiate a particular class.
   * 
   * @param <C> return type
   * @param r Restriction class
   * @param c Base class
   * @return class instance or null
   */
  public  C tryInstantiate(C r,Type c);


  /**
   * Try to instantiate a particular class.
   * 
   * @param <C> return type
   * @param c Base class
   * @return class instance or null
   */
  public  C tryInstantiate(C c);
}
}
