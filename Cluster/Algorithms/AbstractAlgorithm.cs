using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Socona.Clustering.Algorithms
{
    public abstract class AbstractAlgorithm:IAlgorithm
    {
        protected Datasets.Dataset dataset;
        public Results Results { get; protected set; }
        public Arguments Arguments { get; protected set; }


        protected virtual void SetupArguments()
        {
            dataset=Arguments.Dataset;
            Debug.Assert(dataset != null, "Dataset is NULL");
        }
        protected abstract void PerformClustering();
        protected abstract void FetchResults();
       
        public void Clusterize()
        {
            SetupArguments();
            PerformClustering();
            Reset();
            FetchResults();
        }
        public void Reset()
        {
            Results.Reset();
        }
        public AbstractAlgorithm()
        {
            Arguments = new Arguments();
            Results = new Results();
        }

        public void Run()
        {
            Clusterize();
        }
    }
}
