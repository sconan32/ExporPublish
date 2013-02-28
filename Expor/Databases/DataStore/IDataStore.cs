using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Results;

namespace Socona.Expor.Databases.DataStore
{
    public interface IDataStore<T> : IResult
    {
        /**
   * Retrieves an object from the storage.
   * 
   * @param id Database ID.
   * @return Object or {@code null}
   */
        //  object Get(IDbIdRef id);
        T this[IDbIdRef id]
        {
            get;
             set;
        }
    }
}
