using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{

    /**
     * Adapter to use a feature vector as an array of features.
     * 
     * Use the static instance from {@link ArrayLikeUtil}!
     * 
     * @author Erich Schubert
     * 
     * @param <F> Feature type
     */
    public class FeatureVectorAdapter<F> : ArrayAdapterBase<F>
    {
        /**
         * Constructor.
         * 
         * Use the static instance from {@link ArrayLikeUtil}!
         */
        internal FeatureVectorAdapter()
            : base()
        {

        }


        public int Size(IDataVector array)
        {
            return array.Count;
        }


        public  F Get(IDataVector array, int off)
        {
            return (F)array.Get(off);
        }

        public override int Size(IEnumerable<F> array)
        {
            return array.Count();
        }

        public override  F Get(IEnumerable<F> array, int off)
        {
            return array.ElementAt(off);
        }
    }
}
