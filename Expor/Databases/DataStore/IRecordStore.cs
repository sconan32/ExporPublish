using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.DataStore
{
   
public interface IRecordStore {
  /**
   * Get a {@link DataStore} instance for a particular record column.
   * 
   * @param <T> Data type
   * @param col Column number
   * @param datatype data class
   * @return writable storage
   */
    IDataStore<T> GetStorage<T>(int col,Type datatype);
}

}
