using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Socona.Clustering.Constraints
{
    public class CannotLinkSet:ConstraintSet
    {
        public override int GetViolations(Datasets.Record r, Clusters.Cluster[] cls,int k)
        {
            int vcnt = 0;
            IEnumerable<PairConstraint> query = dataset.Where(con => con.First == r);
            foreach (var v in query)
            {
                if (cls[k].Contains(v.Second))
                {
                    vcnt++;
                }

            }
            //Debug.Assert(vcnt == 0, "DEBUG ONLY CL " + k + " " + r.Id.DValue);
            return vcnt;
        }
    }
}
