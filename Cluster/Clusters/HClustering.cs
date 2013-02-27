using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Patterns;
using System.Diagnostics;

namespace Socona.Clustering.Clusters
{
    public class HClustering
    {
        public HClustering()
        { }
        public HClustering(INode root)
        {
            Root = root;
        }
        public INode JoinWith(HClustering hc, double joinValue)
        {
            InternalNode p = new InternalNode(joinValue);
            INode cn1 = Root;
            INode cn2 = hc.Root;
            cn1.Parent = p;
            cn2.Parent = p;
            p.Add(cn1);
            p.Add(cn2);
            
            return p;

        }
        public PClustering GetPC(int maxClust) 
        {
            Debug.Assert(maxClust > 0, "Invalid Param MaxClust");

            int cutLevel = Root.Level - maxClust + 2;

            PClustering pc = new PClustering();
            PCVisitor pcv = new PCVisitor(pc, cutLevel);
            Root.Accept(pcv);
            return pc;
        }
        public void Save(string filename, int p)
        {
            JoinValueVisitor jvv=new JoinValueVisitor ();
            Root.Accept(jvv);

            SortedSet<KeyValuePair<KeyValuePair<int, int>, double>> joinValues = 
                jvv.JoinValues ;
            //Stack<KeyValuePair<KeyValuePair<int, int>, double>> stack=
            //    new Stack<KeyValuePair<KeyValuePair<int,int>,double>> ();
            
            double ljv, hjv;
            int llevel, hlevel;
            //foreach(var v in joinValues)
            //{
            //    stack.Push(v);
            //}
            var lastval = joinValues.Max;
            hjv = lastval.Value;
            hlevel = Root.Level;
            if (p == 0)
            {
                var firstval = joinValues.Min;
                ljv = firstval.Value;
                llevel = 0;
            }
            else
            {
                var it = joinValues.GetEnumerator();
                for (int i = 0; i < joinValues.Count - p + 1; i++)
                {
                    it.MoveNext();
                }
                ljv = it.Current.Value;
                llevel = Root.Level - p + 1;
            }
            DendrogramVisitor dgv = new DendrogramVisitor(hjv, llevel, hlevel);
            Root.Accept(dgv);
            dgv.Save(filename);

        }
        public void Save(string filename)
        {
            Save(filename, 100);
        }

        public INode Root { get; set; }
    }
}
