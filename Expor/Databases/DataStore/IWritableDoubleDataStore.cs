using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{
   
public interface IWritableDoubleDataStore :IDoubleDataStore, IWritableDataStore<double> {
  /**
   * Setter, but using objects.
   * 
   * @deprecated Use {@link #putDouble} instead, to avoid boxing/unboxing cost.
   */

 //Double Put(IDbIdRef id, Double value);

  /**
   * Associates the specified value with the specified id in this storage. If
   * the storage previously contained a value for the id, the previous value is
   * replaced by the specified value.
   * 
   * @param id Database ID.
   * @param value Value to store.
   * @return previous value
   */
}
}
