using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Resluts.TextIO;

namespace Socona.Clustering.Data.Model
{
    public class ModelBase : IModel
    {

        public void WriteToText(TextWriterStream sout, String label)
        {
            if (label != null)
            {
                sout.CommentPrintLine(label);
            }
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + typeof(ModelBase).Name);
        }
    }
}
