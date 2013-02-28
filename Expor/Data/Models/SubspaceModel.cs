using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{

    public class SubspaceModel<V> : MeanModel<V>, ITextWriteable
        where V : IDataVector
    {
        /**
         * The subspace of the cluster.
         */
        private Subspace<V> subspace;

        /**
         * Creates a new SubspaceModel for the specified subspace with the given
         * cluster mean.
         * 
         * @param subspace the subspace of the cluster
         * @param mean the cluster mean
         */
        public SubspaceModel(Subspace<V> subspace, V mean) :
            base(mean)
        {
            this.subspace = subspace;
        }

        /**
         * Returns the subspace of this SubspaceModel.
         * 
         * @return the subspace
         */
        public Subspace<V> Subspace
        {
            get { return subspace; }
        }

        /**
         * Returns the BitArray that represents the dimensions of the subspace of this
         * SubspaceModel.
         * 
         * @return the dimensions of the subspace
         */
        public BitArray GetDimensions()
        {
            return subspace.GetDimensions();
        }

        /**
         * Implementation of {@link TextWriteable} interface.
         */

        public override void WriteToText(TextWriterStream sout, String label)
        {
            base.WriteToText(sout, label);
            sout.CommentPrintLine("Subspace: " + subspace.ToString());
        }
    }

}
