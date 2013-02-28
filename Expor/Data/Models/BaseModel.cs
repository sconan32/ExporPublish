using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{
    public abstract class BaseModel : IModel
    {
        /**
         * Implement writeToText as per {@link de.lmu.ifi.dbs.elki.result.textwriter.TextWriteable} interface.
         * However BaseModel is not given the interface directly, since
         * it is meant as signal to make Models printable. 
         * 
         * @param out Output steam
         * @param label Optional label to prefix
         */
        // actually @Override, for TextWriteable.
        public virtual void WriteToText(TextWriterStream sout, String label)
        {
            if (label != null)
            {
                sout.CommentPrintLine(label);
            }
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + typeof(BaseModel).ToString());
        }
    }
}
