using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public class GraphBase : IGraph
    {

        protected bool initialized;
        protected List<IGraphVertex> nodes;

        public List<IGraphVertex> Nodes
        {
            get { return nodes; }
            //set { nodes = value; }
        }

        public GraphBase()
        {
            nodes = new List<IGraphVertex>();
        }
        public void Reset()
        {
            foreach (var node in nodes)
            {
                node.IsVisited = false;
            }
        }

        public IEnumerator<IGraphVertex> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
        }
    }
}
