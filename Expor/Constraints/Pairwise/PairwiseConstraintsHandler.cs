using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;

namespace Socona.Expor.Constraints.Pairwise
{
    class PairwiseConstraintsHandler : IConstraintHandler
    {
        /**
         * Our logger
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(PairwiseConstraintsHandler));


        List<MustLink> mls = new List<MustLink>();
        List<CannotLink> cls = new List<CannotLink>();
        static int cidcnt = 0;
        public List<MustLink> MustLinks { get { return mls; } }
        public List<CannotLink> CannotLinks { get { return cls; } }
        public void HandleConstraints(IDbIds dataids, DataSources.Bundles.MultipleObjectsBundle conspack)
        {

            IList<object> pairs = conspack.GetColumn(0);
            IList<object> marks = conspack.GetColumn(1);
            for (int i = 0; i < conspack.DataLength(); i++)
            {
                Pair<int, int> thepair = pairs[i] as Pair<int, int>;
                string themark = (marks[i] as IList<string>)[0];
                Int32DbId testid1 = new Int32DbId(thepair.First);
                Int32DbId testid2 = new Int32DbId(thepair.Second);
                if (dataids.Contains(testid1) && dataids.Contains(testid2))
                {
                    if (themark.ToLower().StartsWith("m"))
                    {
                        MustLink ml = new MustLink(new Int32DbId(cidcnt++), testid1, testid2);
                        mls.Add(ml);
                    }
                    else
                    {
                        CannotLink cl = new CannotLink(new Int32DbId(cidcnt++), testid1, testid2);
                        cls.Add(cl);
                    }
                }
                else
                {
                    logger.Warning(string.Format("Invalid Constraints,No such dbid <{0}> and/or <{1}>.", testid1, testid2));
                }
            }
        }
        public class Parameterizer : AbstractParameterizer
        {
            protected override object MakeInstance()
            {
                return new PairwiseConstraintsHandler();
            }
        }
    }
}
