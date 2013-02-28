using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{
    public  class WeightedSubspaceModel:MeanModel<IDataVector>,ITextWriteable
    {
        WeightSubspace subspace;

        public WeightedSubspaceModel(WeightSubspace subspace, IDataVector mean)
            :base(mean)
        {
            this.subspace = subspace;
        }
        public WeightSubspace Subspace { get { return subspace; } }

        public IList<double> SubspaceWeights { get { return subspace.Weights; } }

        public override void WriteToText(TextWriterStream sout, string label)
        {
            base.WriteToText(sout, label);
           
            sout.CommentPrintLine("WeightedSubspace: " + subspace.ToString());
        }
    }
}
