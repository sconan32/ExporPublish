using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Resluts
{
    
     public    class ResultEventArgs:EventArgs
    {
        public IResult Parent { get; set; }
        public IResult Child { get; set; }
        public IResult Current { get; set; }
    }
    public delegate void ResultEventHandler(object sender,ResultEventArgs args);
}
