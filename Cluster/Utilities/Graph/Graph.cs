using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.Graph
{
    class Graph<VT,ET>
    {
        List<Vertex<VT,ET>> vtList = new List<Vertex<VT,ET>>();


        public void AddVertex(Vertex<VT,ET> vertex)
        {
            vtList.Add(vertex);
        }
        public void AddEdge(Vertex<VT,ET> from, Vertex<VT,ET> to)
        {
            
        }
        public void AddEdge(Vertex<VT,ET> from, Vertex<VT,ET> to, ET weight)
        {
            if (vtList.Contains(from) && vtList.Contains(to))
            {
                Edge<ET> e = new Edge<ET>();
                e.Weight = weight;
                
            }
        }
        public void AddEdge(Vertex<VT,ET> from, Edge<ET> edge)
        {
        }

    }
}
