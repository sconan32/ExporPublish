using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Maths.Scales
{

    public class Scales
    {
        /**
         * Compute a linear scale for each dimension.
         * 
         * @param <O> vector type
         * @param db Database
         * @return Scales, indexed starting with 0 (like Vector, not database objects!)
         */
        public static LinearScale[] CalcScales(IRelation db)
        {
            if (db == null)
            {
                throw new AbortException("No database was given to Scales.calcScales.");
            }
            int dim = DatabaseUtil.Dimensionality(db);
            DoubleMinMax[] minmax = DoubleMinMax.NewArray(dim);
            LinearScale[] scales = new LinearScale[dim];

            // analyze data
            foreach (IDbId id in db.GetDbIds())
            {
                // for(DBIDIter iditer = db.iterDBIDs(); iditer.valid(); iditer.advance()) {
                INumberVector v = db[id] as INumberVector;
                for (int d = 0; d < dim; d++)
                {
                    minmax[d].Put(v[d + 1]);
                }
            }

            // generate scales
            for (int d = 0; d < dim; d++)
            {
                scales[d] = new LinearScale(minmax[d].GetMin(), minmax[d].GetMax());
            }
            return scales;
        }
    }

}
