using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;


namespace Socona.Expor.Data.Models
{

    public class DimensionModel : BaseModel, ITextWriteable
    {
        /**
         * Number of dimensions
         */
        private int dimension;

        /**
         * Constructor
         * @param dimension number of dimensions
         */
        public DimensionModel(int dimension) :
            base()
        {
            this.dimension = dimension;
        }

        /**
         * Get cluster dimensionality
         * 
         * @return dimensionality
         */
        public int GetDimension()
        {
            return dimension;
        }

        /**
         * Set cluster dimensionality
         *  
         * @param dimension new dimensionality
         */
        public void SetDimension(int dimension)
        {
            this.dimension = dimension;
        }

        /**
         * Implementation of {@link TextWriteable} interface
         */

        public override void WriteToText(TextWriterStream sout, String label)
        {
            base.WriteToText(sout, label);
            sout.CommentPrintLine("Dimension: " + dimension);
        }

    }

}
