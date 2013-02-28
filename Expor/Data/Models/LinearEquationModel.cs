using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{

    public class LinearEquationModel : BaseModel, ITextWriteable
    {
        /**
         * Equation system
         */
        private LinearEquationSystem les;

        /**
         * Constructor
         * @param les equation system
         */
        public LinearEquationModel(LinearEquationSystem les) :
            base()
        {
            this.les = les;
        }

        /**
         * Get assigned Linear Equation System
         * 
         * @return linear equation system
         */
        public LinearEquationSystem GetLes()
        {
            return les;
        }

        /**
         * Assign new Linear Equation System.
         * 
         * @param les new linear equation system
         */
        public void SetLes(LinearEquationSystem les)
        {
            this.les = les;
        }

        /**
         * Implementation of {@link TextWriteable} interface
         */

        public override void WriteToText(TextWriterStream sout, String label)
        {
            base.WriteToText(sout, label);
            sout.CommentPrintLine(les.EquationsToString(6));
        }

    }

}
