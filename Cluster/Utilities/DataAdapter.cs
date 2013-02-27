using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Utilities
{
    public abstract class DataAdapter
    {
        public  abstract void Fill(ref Dataset ds);
    }
}
