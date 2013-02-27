using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Constraints;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Algorithms
{
    public class CopKmeans:Kmean
    {
        MustLinkSet mls;
        CannotLinkSet cls;
        ConstraintedDataset cds;

        public CopKmeans()
            : base()
        {
            //cds = dataset as ConstraintedDataset;
            //mls = cds.MustLinks;
            //cls = cds.CannotLinks;
        }
        protected override void Initialize()
        {
            cds = dataset as ConstraintedDataset;
            mls = cds.MustLinks;
            cls = cds.CannotLinks;
            base.Initialize();
        }
        protected override void Iterate()
        {
            bool changed = true;
            UpdateCenter();

            numIter = 1;
            while (changed)
            {
                changed = false;
                int s = 0;
                double min, dist;
                for (int i = 0; i < dataset.Count; i++)
                {
                    min = double.MaxValue;
                    for (int k = 0; k < clusters.Count; k++)
                    {
                        dist = distance.CalcDistance(dataset[i], clusters[k].Center);
                        int mv = mls.GetViolations(dataset[i], clusters.ToArray(), k);
                        int cv = cls.GetViolations(dataset[i], clusters.ToArray(), k);

                        dist += mv * 10 + cv * 5;
                        
                        if (min > dist)
                        {
                            min = dist;
                            s = k;
                        }
                    }
                    if (min == double.MaxValue)
                    {
                        throw new Exception("Constaints Have Inflictions!");
                    }
                    if (CM[i] != s)
                    {
                        clusters[CM[i]].Remove(dataset[i]);
                        clusters[s].Add(dataset[i]);
                        CM[i] = s;
                        changed = true;
                    }
                }
                UpdateCenter();
                numIter++;
                if (numIter > maxIter)
                {
                    break;
                }
            }
        }
    }
}
