using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Patterns
{
    public interface INode
    {
        int Id { get; set; }
        int Level { get; set; }
        INode Parent { get; set; }

        //protected Node(Node p, int id)
        //{
        //    Parent = p;
        //    Id = id;
        //}

         void Accept(INodeVisitor v);
        int GetChildrenCount();
         int GetRecordsCount();



        
    }
}
