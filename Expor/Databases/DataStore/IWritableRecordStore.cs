using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{
   
public interface IWritableRecordStore : IRecordStore {
  /**
   * Get a {@link WritableDataStore} instance for a particular record column.
   * 
   * @param <T> Data type
   * @param col Column number
   * @param datatype data class
   * @return writable storage
   */

  //IWritableDataStore GetStorage<T>(int col, Type datatype);
  
  /**
   * Remove an object from the store, all columns.
   * 
   * @param id object ID to remove
   * @return success code
   */
  bool Remove(IDbIdRef id);
}

}
