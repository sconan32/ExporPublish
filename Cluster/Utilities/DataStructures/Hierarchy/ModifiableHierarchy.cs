using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.DataStructures.Hierarchy
{
    
/**
 * Modifiable Hierarchy.
 * 
 * @author Erich Schubert
 * 
 * @param <O> Object type
 */
public interface IModifiableHierarchy<O> : IHierarchy<O> {
  /**
   * Add a parent-child relationship.
   * 
   * @param parent Parent
   * @param child Child
   */
  // TODO: return true when new?
  public void Add(O parent, O child);

  /**
   * Remove a parent-child relationship.
   * 
   * @param parent Parent
   * @param child Child
   */
  // TODO: return true when found?
  public void Remove(O parent, O child);
}

}
