using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public class GraphVertex<T> : IGraphVertex
    {
        protected static int ordinalSerial = 0;
        int ordinal;
        bool isVisited;

        T entry;
        public GraphVertex()
        {
            ordinal = ordinalSerial++;
        }
        public T Entry { get { return entry; } set { entry = value; } }
        public int Ordinal { get { return ordinal; } }
        public bool IsVisited { get { return isVisited; } set { isVisited = value; } }

        public bool ContainsSame(IGraphVertex obj)
        {
            if (obj is GraphVertex<T>)
            {
               return  entry.Equals((obj as GraphVertex<T>).entry);
            }
            return false;
        }
    }
}
