using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{
   
/**
 * Set-oriented implementation of a modifiable DBID collection.
 * 
 * @author Erich Schubert
 */
public interface IHashSetModifiableDbIds : IHashSetDbIds, IModifiableDbIds {
  /**
   * Retain all elements that also are in the second set.
   * 
   * @param set second set
   * @return true when modified
   */
 bool RetainAll(IDbIds set);
}

}
