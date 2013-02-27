using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Patterns
{
    public class LeafNode : INode
    {
        public Record Data { get; protected set; }
        public LeafNode(Record r, int id, INode p)
        {
            Id = id;
            Data = r;
            Parent = p;
            Level = 0;
        }
        public LeafNode(Record r)
            : this(r, 0, null)
        { }
        public  int GetChildrenCount()
        {
            return 0;
        }
        public  int GetRecordsCount()
        {
            return 1;
        }
        public  void Accept(INodeVisitor v)
        {
            v.Visit(this);
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
    }
}
