using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{
    
public interface IDoubleDataStore : IDataStore<double> {
  /**
   * Getter, but using objects.
   * 
   * @deprecated Use {@link #doubleValue} instead, to avoid boxing/unboxing cost.
   */

//Double GetDouble(IDbIdRef id);

}
}
