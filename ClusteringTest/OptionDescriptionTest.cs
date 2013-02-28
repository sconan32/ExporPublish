using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Socona.Expor.Utilities.Options;

namespace ClusteringTest
{
    [TestClass]
    public class OptionDescriptionTest
    {
        [TestMethod]
        public void TestGetOrCreate()
        {
            OptionDescription.GetOrCreate("lpnorm.p", "the degree of the L-P-Norm (positive number)");
        }
    }
}
