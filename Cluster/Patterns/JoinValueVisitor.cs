using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;

namespace Socona.Clustering.Patterns
{
    public class JoinValueVisitor:INodeVisitor
    {
        public SortedSet<KeyValuePair<KeyValuePair<int, int>, double>> JoinValues 
        { get; protected set; }
        public JoinValueVisitor()
        {
            JoinValues = new SortedSet<KeyValuePair<KeyValuePair<int, int>, double>>(new NNMapDoubleComparer());
        }

        public void Visit(LeafNode node)
        {
            //Do Nothing
        }

        public void Visit(InternalNode node)
        {
            if (node.GetChildrenCount() != 2)
            {
                throw new Exception("JoinValueVisitor Can Only Handles Nodes With 2 Children");
            }
            JoinValues.Add(new KeyValuePair<KeyValuePair<int,int>,double>(new KeyValuePair<int,int>(node[0].Id,node[1].Id),
                node.JoinValue));
            node[0].Accept(this);
            node[1].Accept(this);

        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tmp in JoinValues)
            {
                sb.Append(tmp.Key.Key + 1);
                sb.Append(",");
                sb.Append(tmp.Key.Value + 1);
                sb.Append(",");
                sb.Append(tmp.Value);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
