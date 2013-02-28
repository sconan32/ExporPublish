using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Bundles;

namespace Socona.Expor.Constraints
{
    public interface IConstraintHandler
    {
        void HandleConstraints(IDbIds dataids, MultipleObjectsBundle conspack);
    }
}
