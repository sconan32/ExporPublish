using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Results;

namespace Socona.Expor.Algorithms.Clustering.Trivial
{

    public class ByLabelOrAllInOneClustering : ByLabelClustering
    {
        /**
         * Constructor.
         */
        public ByLabelOrAllInOneClustering()
            : base()
        {

        }

        public override IResult Run(IDatabase database)
        {
            // Prefer a true class label
            try
            {
                IRelation relation = database.GetRelation(TypeUtil.CLASSLABEL);
                return Run(relation);
            }
            catch (NoSupportedDataTypeException)
            {
                // Ignore.
            }
            try
            {
                IRelation relation = database.GetRelation(TypeUtil.GUESSED_LABEL);
                return Run(relation);
            }
            catch (NoSupportedDataTypeException)
            {
                // Ignore.
            }
            IDbIds ids = database.GetRelation(TypeUtil.ANY).GetDbIds();
            ClusterList result = new ClusterList("All-in-one trivial Clustering", "allinone-clustering");
            Cluster c = new Cluster(ids, ClusterModel.CLUSTER);
            result.AddCluster(c);
            return result;
        }
    }

}
