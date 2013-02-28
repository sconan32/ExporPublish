using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{

    public interface IWritableInt32DataStore : IInt32DataStore, IWritableDataStore<int>
    {
        /**
         * Setter, but using objects.
         * 
         * @deprecated Use {@link #putInt} instead, to avoid boxing/unboxing cost.
         */

       //Int32 Put(IDbIdRef id, Int32 value);



    }
}
