using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Constraints
{
    public interface IConstraint
    {
        IDbId CId { get; }
    }
}
