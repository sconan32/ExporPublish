using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities;

namespace Socona.Expor.Maths.LinearAlgebra
{
    public class Centroid : Vector
    {
        /**
         * The current weight
         */
        protected double wsum;

        /**
         * Constructor.
         * 
         * @param dim Dimensionality
         */
        public Centroid(int dim)
            : base(dim)
        {

            this.wsum = 0;
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public virtual void Put(double[] val)
        {
            Debug.Assert(val.Length == this.Count);
            wsum += 1.0;
            for (int i = 0; i < this.Count; i++)
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
        public virtual void Put(double[] val, double weight)
        {
            Debug.Assert(val.Length == this.Count);
            double nwsum = weight + wsum;
            for (int i = 0; i < this.Count; i++)
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
        public virtual void Put(Vector val)
        {
            Put(val.GetArrayRef());
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */
        public virtual void Put(Vector val, double weight)
        {
            Put(val.GetArrayRef(), weight);
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public virtual void Put(INumberVector val)
        {
            Debug.Assert(val.Count == this.Count);
            wsum += 1.0;
            for (int i = 0; i < this.Count; i++)
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
        public virtual void Put(INumberVector val, double weight)
        {
            Debug.Assert(val.Count == this.Count);
            double nwsum = weight + wsum;
            for (int i = 0; i < this.Count; i++)
            {
                double delta = val[i] - this[i];
                double rval = delta * weight / nwsum;
                this[i] += rval;
            }
            wsum = nwsum;
        }

        /**
         * Get the data as vector
         * 
         * @return the data
         */
        public F ToVector<F>(IRelation relation) where F : INumberVector
        {
            return (F)DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory().NewNumberVector(this);
        }

        /**
         * Static Constructor from an existing matrix columns.
         * 
         * @param mat Matrix to use the columns from.
         */
        public static Centroid Make(Matrix mat)
        {
            Centroid c = new Centroid(mat.RowCount);
            int n = mat.ColumnCount;
            for (int i = 0; i < n; i++)
            {
                // TODO: avoid constructing the vector objects?
                c.Put(mat.Column(i));
            }
            return c;
        }

        /**
         * Static constructor from an existing relation.
         * 
         * @param relation Relation to use
         * @return Centroid of relation
         */
        public static Centroid Make<V>(IRelation relation)
        where V : INumberVector
        {
            Centroid c = new Centroid(DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in relation.GetDbIds())
            {
                // for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                c.Put((INumberVector)relation[id]);
            }
            return c;
        }

        /**
         * Static constructor from an existing relation.
         * 
         * @param relation Relation to use
         * @param ids IDs to use
         */
        public static Centroid Make<V>(IRelation relation, IDbIds ids)
        where V : INumberVector
        {
            Centroid c = new Centroid(DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in ids)
            {
                //for(DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                c.Put((INumberVector)relation[id]);
            }
            return c;
        }
    }
}
