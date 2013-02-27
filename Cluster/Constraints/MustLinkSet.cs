using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;
using Socona.Clustering.Clusters;
using System.Diagnostics;

namespace Socona.Clustering.Constraints
{
    public class MustLinkSet:ConstraintSet
    {

         
        
        public override  int GetViolations(Record r, Cluster[] cls,int k)
        {
            int vcnt=0;
            IEnumerable<PairConstraint> query = dataset.Where(con => con.First == r);
            foreach (var v in query)
            {
                if (!cls[k].Contains(v.Second))
                {
                    foreach (Cluster c in cls)
                    {
                        if (c == cls[k])
                            continue;

                        if (c.Contains(v.Second))
                        {
                            vcnt++; 
                            break;
                        }
                    }
                }

            }
            //Debug.Assert(vcnt == 0, "DEBUG ONLY ML "+k+" "+r.Id.DValue);
            return vcnt;
           
        }
        public void MakeClosure()
        {
        }
    }
}
