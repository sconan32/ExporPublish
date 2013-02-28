using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Maths.Statistics.Distributions
{
    public class ChiSquared
    {
        /**
         * Return the quantile function for this distribution
         * 
         * Reference:
         * <p>
         * Algorithm AS 91: The percentage points of the $\chi$^2 distribution<br />
         * D.J. Best, D. E. Roberts<br />
         * Journal of the Royal Statistical Society. Series C (Applied Statistics)
         * </p>
         * 
         * @param x Quantile
         * @param dof Degrees of freedom
         * @return quantile position
         */
        [Reference(Title = "Algorithm AS 91: The percentage points of the $\\chi^2$ distribution", Authors = "D.J. Best, D. E. Roberts",
            BookTitle = "Journal of the Royal Statistical Society. Series C (Applied Statistics)")]
        public static double Quantile(double x, double dof)
        {
            return Gamma.Quantile(x, .5 * dof, .5);
        }

    }
}
