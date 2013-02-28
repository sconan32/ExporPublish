using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    class WeightedEdge<T> : GraphEdge
    {
        T weight;
        public T Weight { get { return weight; } set { weight = value; } }

        public WeightedEdge()
            : this(null, null, default(T))
        {
        }
        public WeightedEdge(IGraphVertex from, IGraphVertex to)
            : this(from, to, default(T))
        {

        }
        public WeightedEdge(IGraphVertex from, IGraphVertex to, T weight) :
            base(from, to)
        {
            this.weight = weight;
        }

        public override object Clone()
        {
            return new WeightedEdge<T>(this.From, this.To, this.weight);
        }

    }
}
