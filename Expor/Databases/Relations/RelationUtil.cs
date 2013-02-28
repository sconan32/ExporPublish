using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.Relations
{

    /**
     * Utility functions for handling database relation.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses Relation oneway
     */
    public sealed class RelationUtil
    {
        /**
         * Fake constructor: do not instantiate.
         */
        private RelationUtil()
        {
            // Do not instantiate!
        }


        /// <summary>
        /// Get the vector field type information from a relation.
        /// </summary>
        /// <param name="relation">relation</param>
        /// <returns>Vector field type information</returns>
        public static VectorFieldTypeInformation<INumberVector> AssumeVectorField(IRelation relation)
        {
            try
            {
                return ((VectorFieldTypeInformation<INumberVector>)relation.GetDataTypeInformation());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Expected a vector field, got type information: " + relation.GetDataTypeInformation().ToString(), e);
            }
        }

        /**
         * Get the number vector factory of a database relation.
         * 
         * @param relation relation
         * @param <V> Vector type
         * @param <N> Number type
         * @return Vector field type information
         */
        public static INumberVector GetNumberVectorFactory(IRelation relation)
        {
            VectorFieldTypeInformation<INumberVector> type =
                AssumeVectorField(relation);

            INumberVector factory = (INumberVector)type.GetFactory();
            return factory;
        }

        /// <summary>
        /// Get the dimensionality of a database relation.
        /// </summary>
        /// <param name="relation">relation</param>
        /// <returns> Database dimensionality</returns>
        public static int Dimensionality(IRelation relation)
        {
            try
            {
                return ((dynamic)relation.GetDataTypeInformation()).Dimensionality();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /**
         * <em>Copy</em> a relation into a double matrix.
         * 
         * This is <em>not recommended</em> unless you need to modify the data
         * temporarily.
         * 
         * @param relation Relation
         * @param ids IDs, with well-defined order (i.e. array)
         * @return Data matrix
         */
        public static double[,] RelationAsMatrix(IRelation relation, IArrayDbIds ids)
        {
            int rowdim = ids.Count;
            int coldim = Dimensionality(relation);
            double[,] mat = new double[rowdim, coldim];
            int r = 0;
            //for (DBIDArrayIter iter = ids.iter(); iter.valid(); iter.advance(), r++) {
            foreach (var id in ids)
            {
                INumberVector vec = relation.VectorAt(id);

                for (int c = 0; c < coldim; c++)
                {
                    mat[r, c] = vec[c];
                }
                r++;
            }
            Debug.Assert(r == rowdim);
            return mat;
        }

        /**
         * Get the column name or produce a generic label "Column XY".
         * 
         * @param rel Relation
         * @param col Column
         * @param <V> Vector type
         * @return Label
         */
        public static String GetColumnLabel(IRelation rel, int col)
        {
            String lbl = AssumeVectorField(rel).GetLabel(col);
            if (lbl != null)
            {
                return lbl;
            }
            else
            {
                return "Column " + col;
            }
        }
       
    }

}
