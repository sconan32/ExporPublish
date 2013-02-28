using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures.Graph;

namespace Socona.Expor.Constraints.Pairwise
{
    class CannotLinkGraphEdge : WeightedEdge<int>
    {
        private List<CannotLink> cls = new List<CannotLink>();

        public List<CannotLink> CannotLinks
        { get { return cls.Distinct().ToList(); } }
        public CannotLinkGraphEdge(CannotLink cl, IGraphVertex from, IGraphVertex to, int weight) :
            base(from, to, weight)
        {
            this.cls.Add(cl);
        }
        public CannotLinkGraphEdge(List<CannotLink> cls, IGraphVertex from, IGraphVertex to, int weight)
            : base(from, to, weight)
        {
            CannotLink[] tc = new CannotLink[cls.Count];
            cls.CopyTo(tc);
            this.cls = new List<CannotLink>(tc);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(").Append(this.Weight).Append("*");
            sb.Append(cls[0].ToString()).Append(")");
            return sb.ToString();
        }
    }
}
