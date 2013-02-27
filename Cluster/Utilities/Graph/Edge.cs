using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.Graph
{
    public class Edge<T>
    {
        public T Weight { get; set; }
        public Edge Next { get; set; }
    }
}
