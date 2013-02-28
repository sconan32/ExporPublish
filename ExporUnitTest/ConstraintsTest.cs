using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Socona.Expor.Constraints.Pairwise;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Int32DbIds;

namespace ExporUnitTest
{
    [TestClass]
    public class ConstraintsTest
    {
        [TestMethod]
        public void TestCreateConstraintsGraph()
        {
            List<MustLink> mls = new List<MustLink>
            {
                new MustLink(new Int32DbId(32),new Int32DbId(66)),
                new MustLink(new Int32DbId(66),new Int32DbId(86)),
                new MustLink(new Int32DbId(86),new Int32DbId(44)),
                new MustLink(new Int32DbId(77),new Int32DbId(66)),
                new MustLink(new Int32DbId(09),new Int32DbId(66)),
                new MustLink(new Int32DbId(12),new Int32DbId(22)),
                new MustLink(new Int32DbId(16),new Int32DbId(06)),
                new MustLink(new Int32DbId(01),new Int32DbId(08)),
            };
            List<CannotLink> cls = new List<CannotLink> 
            {
                new CannotLink(new Int32DbId(32),new Int32DbId(55)),
                new CannotLink(new Int32DbId(28),new Int32DbId(87)),
                new CannotLink(new Int32DbId(90),new Int32DbId(27)),
                new CannotLink(new Int32DbId(05),new Int32DbId(17)),
                new CannotLink(new Int32DbId(65),new Int32DbId(57)),
                new CannotLink(new Int32DbId(65),new Int32DbId(97)),
                new CannotLink(new Int32DbId(45),new Int32DbId(77)),
                new CannotLink(new Int32DbId(35),new Int32DbId(22)),
                new CannotLink(new Int32DbId(25),new Int32DbId(24)),
                new CannotLink(new Int32DbId(15),new Int32DbId(71)),
                new CannotLink(new Int32DbId(10),new Int32DbId(66)),
                new CannotLink(new Int32DbId(08),new Int32DbId(87)),
                new CannotLink(new Int32DbId(06),new Int32DbId(24)),
            };
            ConsGraph graph = new ConsGraph(mls, cls);
            Assert.IsNotNull(graph);

        }
    }
}
