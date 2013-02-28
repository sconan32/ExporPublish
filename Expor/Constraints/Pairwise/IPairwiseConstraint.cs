using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Constraints.Pairwise
{
    public interface IPairwiseConstraint : IConstraint
    {
        IDbId First { get; }
        IDbId Second { get; }
    }
}
