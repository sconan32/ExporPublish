using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Distances
{
    public class EuclideanDistance:MinkowskiDistance
    {
        public EuclideanDistance()
            : base(2.0)
        {
            Name = "Euclidean Distance";
        }
        public override double CalcDistance(Datasets.Record lhs, Datasets.Record rhs)
        {
            return base.CalcDistance(lhs, rhs);
        }
    }
}
