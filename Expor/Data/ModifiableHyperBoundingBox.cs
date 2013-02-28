using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;

namespace Socona.Expor.Data
{

    public class ModifiableHyperBoundingBox : HyperBoundingBox
    {
        /**
         * Serial version
         */
       // private static long serialVersionUID = 1;

        /**
         * Constructor
         */
        public ModifiableHyperBoundingBox()
            : base()
        {
        }

        /**
         * Uses the references to the fields in <code>hbb</code> as <code>min</code>,
         * <code>max</code> fields. Thus, this constructor indirectly provides a way
         * to modify the fields of a {@link HyperBoundingBox}.
         * 
         * FIXME: that isn't really nice and should be handled with care.
         * 
         * @param hbb existing hyperboundingbox
         */
        public ModifiableHyperBoundingBox(ISpatialComparable hbb) :
            base(SpatialUtil.GetMin(hbb), SpatialUtil.GetMax(hbb))
        {
        }

        /**
         * Creates a ModifiableHyperBoundingBox for the given hyper points.
         * 
         * @param min - the coordinates of the minimum hyper point
         * @param max - the coordinates of the maximum hyper point
         */
        public ModifiableHyperBoundingBox(double[] min, double[] max)
        {
            if (min.Length != max.Length)
            {
                throw new ArgumentException("min/max need same dimensionality");
            }
            this.min = min;
            this.max = max;
        }

        /**
         * Set the maximum bound in dimension <code>dimension</code> to value
         * <code>value</code>.
         * 
         * @param dimension the dimension for which the coordinate should be set,
         *        where 1 &le; dimension &le; <code>this.Count</code>
         * @param value the coordinate to set as upper bound for dimension
         *        <code>dimension</code>
         */
        public void setMax(int dimension, double value)
        {
            max[dimension - 1] = value;
        }

        /**
         * Set the minimum bound in dimension <code>dimension</code> to value
         * <code>value</code>.
         * 
         * @param dimension the dimension for which the lower bound should be set,
         *        where 1 &le; dimension &le; <code>this.Count</code>
         * @param value the coordinate to set as lower bound for dimension
         *        <code>dimension</code>
         */
        public void setMin(int dimension, double value)
        {
            max[dimension - 1] = value;
        }

        /**
         * Returns a reference to the minimum hyper point.
         * 
         * @return the minimum hyper point
         */
        public double[] GetMinRef()
        {
            return min;
        }

        /**
         * Returns the reference to the maximum hyper point.
         * 
         * @return the maximum hyper point
         */
        public double[] GetMaxRef()
        {
            return max;
        }

        /**
         * Extend the bounding box by some other spatial object
         * 
         * @param obj Spatial object to extend with
         * @return true when the MBR changed.
         */
        public bool extend(ISpatialComparable obj)
        {
            int dim = min.Length;
            Debug.Assert((obj.Count == dim));
            bool extended = false;
            for (int i = 0; i < dim; i++)
            {
                double omin = obj.GetMin(i + 1);
                double omax = obj.GetMax(i + 1);
                if (omin < min[i])
                {
                    min[i] = omin;
                    extended = true;
                }
                if (omax > max[i])
                {
                    max[i] = omax;
                    extended = true;
                }
            }
            return extended;
        }
    }
}
