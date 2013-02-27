using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Distances;

namespace Socona.Clustering.Algorithms
{
    public abstract class AbstractDistanceBasedAlgorithm:AbstractAlgorithm
    {
        protected AbstractDistanceBasedAlgorithm(Distance distance)
        {

            this.Distance = distance;
        }

       public Distance Distance { get; set; }
    }
}
