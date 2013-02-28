using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{
    public interface IDbId : IDbIdRef, IComparable<IDbIdRef>, IComparable, IArrayDbIds
    { }
}
