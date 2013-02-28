using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;

namespace Socona.Expor.Indexes.Tree.Spatial
{

    public interface ISpatialEntry : IEntry, ISpatialComparable
    {
        // Emtpy - just combining the two interfaces above.
    }
}
