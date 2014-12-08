﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Maths;

namespace Socona.Expor.Utilities.Scaling
{

    public class LinearScaling : IStaticScalingFunction
    {
        /**
         * Scaling factor
         */
        private double factor;

        /**
         * Shift
         */
        private double shift;

        /**
         * Constructor with defaults resulting in identity.
         */
        public LinearScaling() :
            this(1.0, 0.0)
        {
        }

        /**
         * Constructor with scaling only.
         * 
         * @param factor Scaling factor
         */
        public LinearScaling(double factor) :
            this(factor, 0.0)
        {
        }

        /**
         * Full constructor.
         * 
         * @param factor Scaling factor
         * @param shift Shift value
         */
        public LinearScaling(double factor, double shift)
        {
            this.factor = factor;
            this.shift = shift;
        }

        /**
         * Constructor from a double minmax.
         *
         * @param minmax Minimum and Maximum
         */
        public LinearScaling(DoubleMinMax minmax)
        {
            this.factor = 1.0 / (minmax.GetMax() - minmax.GetMin());
            this.shift = -minmax.GetMin() / this.factor;
        }


        public double GetScaled(double d)
        {
            return factor * d + shift;
        }


        public double GetMin()
        {
            return Double.NegativeInfinity;
        }


        public double GetMax()
        {
            return Double.PositiveInfinity;
        }

        /**
         * Make a linear scaling from a given minimum and maximum. The minimum will be
         * mapped to zero, the maximum to one.
         * 
         * @param min Minimum
         * @param max Maximum
         * @return New linear scaling.
         */
        public static LinearScaling FromMinMax(double min, double max)
        {
            double zoom = 1.0 / (max - min);
            return new LinearScaling(zoom, -min / zoom);
        }
    }
}
