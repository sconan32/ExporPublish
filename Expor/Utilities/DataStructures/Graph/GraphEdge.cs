using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    class GraphEdge : IGraphEdge
    {

        protected IGraphVertex from;
        protected IGraphVertex to;

        public IGraphVertex From { get { return from; } }
        public IGraphVertex To { get { return to; } }

        public GraphEdge()
        {
        }

        public GraphEdge(IGraphVertex from, IGraphVertex to)
        {
            this.from = from;
            this.to = to;
        }



        public virtual object Clone()
        {
            return new GraphEdge(this.from, this.to);
        }
    }
}
