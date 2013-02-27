using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
using Socona.Clustering.Datasets;
using System.IO;

namespace Socona.Clustering.Clusters
{
    public class PClustering:Container<Cluster>
    {
        bool calculated;
        int numclust;
        int numclustGiven;

        List<int> clustSize;
        List<string> clustLabel;
        List<int> CM;
        List<int> CMGiven;
        NNMap<int> map;
        public PClustering()
        {
            clustSize = new List<int>();
            clustLabel = new List<string>();
            map = new NNMap<int>();
        }
        void CrossTab()
        {
            int c1, c2;
            for (int i = 0; i < CM.Count; i++)
            {
                c1 = CM[i];
                c2 = CMGiven[i];
                if (map.ContainsKey(new KeyValuePair<int, int>(c1, c2)))
                {
                    map[c1, c2] += 1;
                }
                else
                {
                    map[c1, c2] = 1;
                }
            }
        }
        public void Calculate()
        {
            if (calculated)
            {
                return;
            }
            CreateClusterId();
            numclust = data.Count;
            Cluster c;
            Record r;
            CM = new List<int>(data[0][0].Schema.IdInfo.NumValueCount);
            for (int i = 0; i < data[0][0].Schema.IdInfo.NumValueCount; i++)
            {
                CM.Add(0);
            }
            for (int i = 0; i < numclust; i++)
            {
                c = data[i];
                clustSize.Add(c.Count);
                for (int j = 0; j < c.Count; j++)
                {
                    r = c[j];
                    CM[(int)r.Id.DValue] = c.Id;
                }
            }
            DAttriInfo info = data[0][0].Schema.LabelInfo;
            if (info == null)
            {
                calculated = true;
                return;
            }

            numclustGiven = info.NumValueCount;
            for (int i = 0; i < numclustGiven; i++)
            {
                clustLabel.Add(info.GetValueFromId(i));
            }
            CMGiven = new List<int>(CM.Capacity);
            for (int i = 0; i < CM.Capacity; i++)
            {
                CMGiven.Add(0);
            }
            for (int i = 0; i < numclust; i++)
            {
                c = data[i];
                for (int j = 0; j < c.Count; j++)
                {
                    r = c[j];
                    CMGiven[(int)r.Id.DValue] = (int)r.Label.DValue;
                }
            }
            CrossTab();
            calculated = true;

        }
        public void RemoveEmptyClusters()
        {
            List<Cluster> temp = new List<Cluster>();
            foreach (Cluster cl in data)
            {
                temp.Add(cl);
            }
            data.Clear();
            foreach (Cluster cl in temp)
            {
                if (cl.Count > 0)
                {
                    data.Add(cl);
                }
            }
        }
        public void CreateClusterId()
        {
            RemoveEmptyClusters();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Id = i;
            }
        }
        public string ToCompleteString()
        {
            StringBuilder sb = new StringBuilder();
            Calculate();

            sb.Append("CLUSTER SUMMARY"+Environment.NewLine);
            sb.Append("---" + Environment.NewLine);
            sb.Append("Number of Clusters:" + numclust+Environment.NewLine);
            for (int i = 0; i < numclust; i++)
            {
                sb.Append("Size of Cluster " + i + ": " + clustSize[i]+Environment.NewLine);

            }
            sb.Append(Environment.NewLine);

            if (numclustGiven > 0)
            {
                sb.Append("Number of Given Clusters: " + numclustGiven + Environment.NewLine);
                sb.Append("Cross Tabulation:" + Environment.NewLine);
                List<int> w = new List<int>();
                

                w.Add(6);
                StringBuilder fm = new StringBuilder();
                object [] strs=new object[numclustGiven+1];
                fm.Append("{0,-");
                fm.Append(w[0]);
                fm.Append("}");
                strs[0]="CL-ID";
                for(int i=0;i<numclustGiven;i++)
                {
                    w.Add(clustLabel[i].Length+3);
                    fm.Append("{");
                    fm.Append(i + 1);
                    fm.Append(",");
                    fm.Append(w[i]+2);
                    fm.Append("}");
                    strs[i+1]=clustLabel[i];
                }
                string fms = fm.ToString();
                sb.Append(string.Format(fms, strs)+Environment.NewLine);

                for (int i = 0; i < numclust; i++)
                {
                    strs[0] = i;
                    for (int j = 0; j < numclustGiven; j++)
                    {
                        if (map.ContainsKey(new KeyValuePair<int, int>(i, j)))
                        {
                            strs[j + 1] = map[i, j];
                        }
                        else
                        {
                            strs[j + 1] = 0;
                        }
                    }
                    sb.Append(string.Format(fms, strs) + Environment.NewLine);
                }
                

            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public void Save(string fileName)
        {
            FileStream fs = File.OpenWrite(fileName);
            StreamWriter sr = new StreamWriter(fs);

            sr.Write(ToCompleteString());

            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine+"ClusterMembership"+Environment.NewLine);
            sb.Append("Record ID, Cluster Index, Cluster Index Given"+Environment.NewLine);
            for (int i = 0; i < CM.Count; i++)
            {
                sb.Append((i + 1).ToString() + ", " + CM[i]);
                if (numclustGiven == int.MaxValue)
                {
                    sb.Append(", NA"+Environment.NewLine);
                    continue;
                }
                sb.Append(", " + CMGiven[i] + Environment.NewLine);
            }
            sr.Write(sb.ToString());
            sr.Close();
            fs.Close();


        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
