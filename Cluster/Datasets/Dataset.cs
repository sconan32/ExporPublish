using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
namespace Socona.Clustering.Datasets
{
    public class Dataset:Container<Record>
    {
        public Schema Schema { get; protected set; }
        public int AttributesCount { get; protected set; }
        public Dataset(Schema schema)
        {
            this.Schema = schema;
            AttributesCount = schema.Count;
        }
        public virtual string ToDescriptionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Records #" + Count+Environment.NewLine);
            sb.Append("Attributes #" + AttributesCount + Environment.NewLine);
            int n = 0;
            for (int i = 0; i < AttributesCount; i++)
            {
                if (Schema[i] is CAttrInfo)
                {
                    ++n;
                }
            }
            sb.Append("Numerical Attributes #" + n + Environment.NewLine);
            sb.Append("Categorical Attributes #" + (AttributesCount - n) + Environment.NewLine);
            return sb.ToString();
        }
    }
}
