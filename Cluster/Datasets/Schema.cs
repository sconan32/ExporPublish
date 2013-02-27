using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
namespace Socona.Clustering.Datasets
{
    public class Schema:Container<AttrInfo>
    {
        public DAttriInfo LabelInfo { get; set; }
        public DAttriInfo IdInfo { get; set; }

        public void SetLabel(Record r, string val)
        {
            int label = LabelInfo.AddValue(val, true);
            r.Label = label;
        }
        public void SetId(Record r, string val)
        {
            int id = IdInfo.AddValue(val, false);
            r.Id = id;
        }
    }
}
