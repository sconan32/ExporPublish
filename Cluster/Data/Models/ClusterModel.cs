using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Data.Model
{
    public sealed class ClusterModel : ModelBase
    {
        /**
         * Static cluster model that can be shared for all clusters (since the object
         * doesn't include meta information.
         */
        public readonly static ClusterModel CLUSTER = new ClusterModel();


        public override String ToString()
        {
            return "ClusterModel";
        }
    }
}
