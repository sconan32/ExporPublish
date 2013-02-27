using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Clusters;

namespace Socona.Clustering.Patterns
{
    public class PCVisitor:INodeVisitor
    {
        private PClustering pc;
        private int cutLevel;

        public PCVisitor(PClustering pc, int cutLevel)
        {
            // TODO: Complete member initialization
            this.pc = pc;
            this.cutLevel = cutLevel;
        }

        public void Visit(LeafNode node)
        {
            Cluster c = new Clusters.Cluster();
            c.Add(node.Data);
            pc.Add(c);
        }

        public void Visit(InternalNode node)
        {
            if (node.Level >= cutLevel)
            {
                for (int i = 0; i < node.GetChildrenCount(); ++i)
                {
                    node[i].Accept(this);
                }
            }
            else
            {
                CVisitor cv=new CVisitor ();
                node.Accept(cv);
                pc.Add(cv.Cluster);
            }
        }
    }
    public class CVisitor : INodeVisitor
    {
        public Cluster Cluster { get; private set; }
        public CVisitor()
        {
            Cluster = new Clusters.Cluster();
        }
        public void Visit(LeafNode node)
        {
            Cluster.Add(node.Data);
        }

        public void Visit(InternalNode node)
        {
            int n = node.GetChildrenCount();
            for (int i = 0; i < n; i++)
            {
                node[i].Accept(this);
            }
        }
    }
}
