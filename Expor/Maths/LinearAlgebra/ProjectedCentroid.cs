using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Extenstions;
namespace Socona.Expor.Maths.LinearAlgebra
{

    public class ProjectedCentroid : Centroid
    {
        /**
         * The selected dimensions.
         */
        private BitArray dims;

        /**
         * Constructor for updating use.
         * 
         * @param dims Dimensions to use (indexed with 0)
         * @param dim Full dimensionality
         */
        public ProjectedCentroid(BitArray dims, int dim)
            : base(dim)
        {

            this.dims = dims;
            Debug.Assert(dims.Length <= dim);
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */

        public override void Put(double[] val)
        {
            Debug.Assert(val.Length == this.Count);
            wsum += 1.0;
            for (int i = dims.NextSetBitIndex(0); i >= 0; i = dims.NextSetBitIndex(i + 1))
            {
                double delta = val[i] - this[i];
                this[i] += delta / wsum;
            }
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */

        public override void Put(double[] val, double weight)
        {
            Debug.Assert(val.Length == this.Count);
            double nwsum = weight + wsum;
            for (int i = dims.NextSetBitIndex(0); i >= 0; i = dims.NextSetBitIndex(i + 1))
            {
                double delta = val[i] - this[i];
                double rval = delta * weight / nwsum;
                this[i] += rval;
            }
            wsum = nwsum;
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */

        public override void Put(INumberVector val)
        {
            Debug.Assert(val.Count == this.Count);
            wsum += 1.0;
            for (int i = dims.NextSetBitIndex(0); i >= 0; i = dims.NextSetBitIndex(i + 1))
            {
                double delta = val[i + 1] - this[i];
                this[i] += delta / wsum;
            }
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */

        public void put(INumberVector val, double weight)
        {
            Debug.Assert(val.Count == this.Count);
            double nwsum = weight + wsum;
            for (int i = dims.NextSetBitIndex(0); i >= 0; i = dims.NextSetBitIndex(i + 1))
            {
                double delta = val[i + 1] - this[i];
                double rval = delta * weight / nwsum;
                this[i] += rval;
            }
            wsum = nwsum;
        }

        /**
         * Static Constructor from a relation.
         * 
         * @param dims Dimensions to use (indexed with 0)
         * @param relation Relation to process
         */
        public static ProjectedCentroid Make<V>(BitArray dims, IRelation relation)
        where V : INumberVector
        {
            ProjectedCentroid c = new ProjectedCentroid(dims, DatabaseUtil.Dimensionality(relation));
            Debug.Assert(dims.Length <= DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in relation.GetDbIds())
            {
                // for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                c.Put((double[])relation[id]);
            }
            return c;
        }

        /**
         * Static Constructor from a relation.
         * 
         * @param dims Dimensions to use (indexed with 0)
         * @param relation Relation to process
         * @param ids IDs to process
         */
        public static ProjectedCentroid Make<V>(BitArray dims, IRelation relation, IDbIds ids)
        where V : INumberVector
        {
            ProjectedCentroid c = new ProjectedCentroid(dims, DatabaseUtil.Dimensionality(relation));
            Debug.Assert(dims.Length <= DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in relation.GetDbIds())
            {
                //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                c.Put((double[])relation[id]);
            }
            return c;
        }
    }
}
