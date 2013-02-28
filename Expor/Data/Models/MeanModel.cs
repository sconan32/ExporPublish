using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;


namespace Socona.Expor.Data.Models
{

    public class MeanModel<V> : BaseModel, ITextWriteable
        where V:IDataVector
    {
        /**
         * Cluster mean
         */
        private V mean;

        /**
         * Constructor with mean
         * 
         * @param mean Cluster mean
         */
        public MeanModel(V mean)
            : base()
        {

            this.mean = mean;
        }

        /**
         * @return mean
         */
        public V GetMean()
        {
            return mean;
        }

        /**
         * @param mean Mean vector
         */
        public void SetMean(V mean)
        {
            this.mean = mean;
        }

        /**
         * Implementation of {@link TextWriteable} interface.
         */

        public override void WriteToText(TextWriterStream sout, String label)
        {
            if (label != null)
            {
                sout.CommentPrintLine(label);
            }
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + GetType().Name);
            sout.CommentPrintLine("Cluster Mean: " + mean.ToString());
        }
    }

}
