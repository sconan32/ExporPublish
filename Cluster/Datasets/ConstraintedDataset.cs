using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Constraints;

namespace Socona.Clustering.Datasets
{
    public class ConstraintedDataset:Dataset
    {
        public ConstraintedDataset(Schema schema)
            : base(schema)
        {
            MustLinks = new MustLinkSet();
            CannotLinks = new CannotLinkSet();
        }

        public MustLinkSet MustLinks { get; protected set; }
        public CannotLinkSet CannotLinks { get; protected set; }
        public override string ToDescriptionString()
        {
            string tmp= base.ToDescriptionString();
            StringBuilder sb = new StringBuilder();
            sb.Append("MustLink Constraints #");
            sb.Append(MustLinks.Count);
            sb.Append(Environment.NewLine +"CannotLink Constraints #");
            sb.Append(CannotLinks.Count);
            sb.Append(Environment.NewLine);
            return tmp + sb.ToString();
        }

    }
}
