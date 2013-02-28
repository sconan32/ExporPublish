using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public interface IGraphEdge:ICloneable
    {
         IGraphVertex From { get; }
         IGraphVertex To { get; }
    }
}
