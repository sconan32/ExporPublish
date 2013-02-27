using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Algorithms
{
    class Kprototype:Kmean
    {
        protected override void SetupArguments()
        {
            base.SetupArguments();


        }
        protected override void UpdateCenter()
        {
            double tmp;
            Schema schema = dataset.Schema;

            for (int k = 0; k < clusters.Count; k++)
            {
                for (int j = 0; j < schema.Count; j++)
                {
                    if (schema[j] is CAttrInfo)
                    {
                        tmp = 0;
                        for (int i = 0; i < clusters[k].Count; i++)
                        {
                            Record rec = clusters[k][i];
                            tmp += rec[j].CValue ;
                        }
                        clusters[k].Center[j] = tmp / clusters[k].Count;

                    }
                    else
                    {
                        DAttriInfo da = schema[j] as DAttriInfo;
                        Dictionary<int, int> freq = new Dictionary<int, int>();
                        for (int i = 0; i < da.NumValueCount; i++)
                        {
                            freq.Add(i, 0);
                        }
                        for (int i = 0; i < clusters[k].Count; i++)
                        {
                            Record rec = clusters[k][i];
                            freq[rec[j].DValue] += 1;
                        }
                        int max = 0;
                        int s = 0;
                        for (int i = 0; i < da.NumValueCount; ++i)
                        {
                            if (max < freq[i])
                            {
                                max = freq[i];
                                s = i;
                            }

                        }
                        clusters[k].Center[j] = s;
                    }
                }
            }
            
        }
    }
}
