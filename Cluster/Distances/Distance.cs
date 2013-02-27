using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Distances
{
    public abstract class Distance
    {
        public string Name { get; protected set; }

        public Distance(string name)
        {
            this.Name = name;
        }
        public abstract double CalcDistance(Record lhs, Record rhs);
    
    }
}
