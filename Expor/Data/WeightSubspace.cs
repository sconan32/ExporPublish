using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;

namespace Socona.Expor.Data
{
    public class WeightSubspace
    {
        IList<double> weights;
        int dimCount;

        public WeightSubspace(int dimcount)
            : this(dimcount, new double[dimcount])
        {

        }
        public WeightSubspace(IList<double> weights)
            : this(weights.Count, weights)
        {


        }
        public WeightSubspace(int dimcount, IList<double> weights)
        {
            this.dimCount = dimcount;
            this.weights = weights;
            while (weights.Count < dimcount)
            {
                weights.Add(0);
            }
        }


        public IList<double> Weights
        {
            get { return weights; }
            set { weights = value; }
        }
        public int Count { get { return dimCount; } }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < weights.Count; i++)
            {
                sb.Append(FormatUtil.Format(weights[i]));
                if (i < weights.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}
