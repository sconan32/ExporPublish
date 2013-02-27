using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;

namespace Socona.Clustering.Patterns
{
    public class InternalNode:Container<INode>,INode
    {
        public double JoinValue { get; set; }

        public InternalNode(int id, INode p)
            :this(0,id,p)        { }
        public InternalNode() : this(0, null) { }
        public InternalNode(double joinValue, int id, INode p)
            
        {
            Id = id;
            Parent = p;
            Level = 0;
            JoinValue = joinValue;
        }
        public InternalNode(double joinValue)
            : this(joinValue, 0, null) 
        { }
        public void Accept(INodeVisitor v)
        {
            v.Visit(this);
        }
        public  int GetRecordsCount()
        {
            int sum=0;
            for (int i = 0; i < data.Count; i++)
            {
                sum += data[i].GetChildrenCount();
            }
            return sum;
        }

        public int Id
        {
            get;
           
            set;
            
        }

        public int Level
        {
            get;
           
            set;
           
        }

        public INode Parent
        {
            get;
           
            set;
            
        }

        public int GetChildrenCount()
        {
            return data.Count;
        }
    }
}
