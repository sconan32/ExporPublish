using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths.Scales;

namespace Socona.Expor.Results
{

    public class ScalesResult : BasicResult
    {
        /**
         * The scales in use.
         */
        private LinearScale[] scales;

        /**
         * Constructor.
         * 
         * @param relation Relation to use
         */
        public ScalesResult(IRelation relation)
        :
            this(Scales.CalcScales(relation)){
        }

        /**
         * Constructor.
         * 
         * @param scales Relation scales to use
         */
        public ScalesResult(LinearScale[] scales):
        base("scales", "scales"){
            this.scales = scales;
        }

        /**
         * Get the scale for dimension dim (starting at 1!).
         * 
         * @param dim Dimension
         * @return Scale
         */
        public LinearScale GetScale(int dim)
        {
            return scales[dim - 1];
        }

        /**
         * Set the scale for dimension dim (starting at 1!).
         * 
         * Note: you still need to trigger an event. This is not done automatically,
         * as you might want to set more than one scale!
         * 
         * @param dim Dimension
         * @param scale New scale
         */
        public void SetScale(int dim, LinearScale scale)
        {
            scales[dim - 1] = scale;
        }

        /**
         * Get all scales. Note: you must not modify the array.
         * 
         * @return Scales array.
         */
        public LinearScale[] GetScales()
        {
            return scales;
        }
    }
}
