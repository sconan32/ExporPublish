using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public interface IGraphVertex
    {

        int Ordinal { get;   }
         bool IsVisited { get; set; }
         bool ContainsSame(IGraphVertex obj);
     
    }
}
