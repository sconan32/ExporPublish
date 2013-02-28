using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Scaling
{

    public class IdentityScaling : IStaticScalingFunction
    {

        public double GetScaled(double value)
        {
            return value;
        }


        public double GetMin()
        {
            return Double.NegativeInfinity;
        }


        public double GetMax()
        {
            return Double.PositiveInfinity;
        }
    }

}
