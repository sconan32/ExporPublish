using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public class AdjacencyListGraph : GraphBase
    {


        protected AdjacencyList list;
        public AdjacencyListGraph()
        {
            this.list = new AdjacencyList(Nodes);
        }
        public void AddNode(IGraphVertex node)
        {
            foreach (var nd in Nodes)
            {
                if (nd.ContainsSame(node))
                {
                    return;
                }
            }
            list.AddNode(node);
            Nodes.Add(node);

        }
        public void DeleteNode(IGraphVertex node)
        {
            if (Nodes.Contains(node))
            {
                list.DeleteNode(node);
                Nodes.Remove(node);
            }
        }
        public void AddEdge(IGraphEdge edge)
        {
            list.AddEdge(edge);
        }

        public bool IsEdgeExist(IGraphEdge edge)
        {
            return list.IsEdgeExist(edge);
        }
        public bool IsEdgeExist(IGraphVertex from, IGraphVertex to)
        {
            IEnumerator<IGraphEdge> eiter = EnumEdgesFor(from);

            while (eiter.MoveNext())
            {
                if (eiter.Current.To == to)
                {
                    return true;
                }
            }
            return false;
        }
        public IGraphEdge GetEdge(IGraphVertex from, IGraphVertex to)
        {
            IEnumerator<IGraphEdge> eiter = EnumEdgesFor(from);

            while (eiter.MoveNext())
            {
                if (eiter.Current.To == to)
                {
                    return eiter.Current;
                }
            }
            return null;
        }
        public IList<IGraphEdge> GetEdgesFor(IGraphVertex node)
        {
            IEnumerator<IGraphEdge> eiter = EnumEdgesFor(node);
            List<IGraphEdge> list = new List<IGraphEdge>();
            while (eiter.MoveNext())
            {
                list.Add(eiter.Current);
            }
            return list;
        }
        public IEnumerator<IGraphEdge> EnumEdgesFor(IGraphVertex node)
        {
            return new AdjacencyList.EdgeEnumerator(list, node);
        }

    }
}
