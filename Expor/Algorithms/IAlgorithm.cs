using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Results;

namespace Socona.Expor.Algorithms
{
    public interface IAlgorithm
    {
        /**
   * Runs the algorithm.
   * 
   * @param database the database to run the algorithm on
   * @return the Result computed by this algorithm
   */
        IResult Run(IDatabase database);

        /**
         * Get the input type restriction used for negotiating the data query.
         * 
         * @return Type restriction
         */
         ITypeInformation[] GetInputTypeRestriction();
      
    }
}
