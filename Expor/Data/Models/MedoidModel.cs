using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{

    public class MedoidModel : BaseModel, ITextWriteable
    {
        /**
         * Cluster medoid
         */
        private IDbId medoid;

        /**
         * Constructor with medoid
         * 
         * @param medoid Cluster medoid
         */
        public MedoidModel(IDbId medoid)
            : base()
        {

            this.medoid = medoid;
        }


        public IDbId Medoid
        {
            get { return medoid; }
            set { medoid = value; }
        }

        /**
         * Implementation of {@link TextWriteable} interface.
         */

        public override  void WriteToText(TextWriterStream sout, String label)
        {
            if (label != null)
            {
                sout.CommentPrintLine(label);
            }
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + GetType().ToString());
            sout.CommentPrintLine("Cluster Medoid: " + medoid.ToString());
        }
    }
}