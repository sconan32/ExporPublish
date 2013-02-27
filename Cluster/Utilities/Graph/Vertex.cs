using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.Graph
{
    public class Vertex<VT,ET>
    {
        
        public VT Data { get; set; }

        public List<Edge<ET>> Edges { get; protected set; }
    }
}
