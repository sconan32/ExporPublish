using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Distances
{
    public class MinkowskiDistance:Distance
    {
        double power;
        public MinkowskiDistance()
            :base("Minkowski Distance")
        {
            this.power=2.0;
        }
        public MinkowskiDistance(double p)
            : base("Minkowski Distance")
        {
            Debug.Assert(p >= 1, "Invalid Parameter");
            power = p;
        }
        public override double CalcDistance(Datasets.Record lhs, Datasets.Record rhs)
        {
            Schema schema = lhs.Schema;
            Debug.Assert(schema == rhs.Schema, " Schema Does Not Match");

            double res = 0;
            for (int i = 0; i < schema.Count; i++)
            {
                res += Math.Pow(
                    Math.Abs(schema[i].CalcDistance(lhs[i], rhs[i])), power);
                           }
            return Math.Pow(res, 1 / power);
        }
    }
}
