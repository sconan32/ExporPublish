using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
namespace Socona.Clustering.Datasets
{
    public class Record:Container<AttrValue>,ICloneable
    {
        public Schema Schema { get; protected set; }
        public AttrValue Label { get; set; }
        public AttrValue Id { get; set; }
        public Record(Schema schema)
        {
            this.Schema = schema;
            data = new List<AttrValue>(schema.Count);
            AttrValue av=new AttrValue ();
            foreach (AttrInfo ai in schema)
            {
                ai.InitAttrValue(av);
                data.Add(av);
            }
        }

        public object Clone()
        {
            Record r = new Record(Schema);
            r.Label = Label;
            r.Id = Id;
            r.data = new List<AttrValue>();
            //AttrValue av = new AttrValue();
            foreach (AttrValue av in data)
            {
               
                r.data.Add(av);
            }
            return r;
        }
        public override string ToString()
        {
            return "ID: " + Id.DValue + " Label: " + Label.DValue;
        }
    }
}
