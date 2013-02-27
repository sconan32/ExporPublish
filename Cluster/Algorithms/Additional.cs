using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Algorithms
{
    public class Additional
    {
        public Additional()
        {
            Values = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Values
        {
            get;
            protected set;
        }
        public object Get(string name)
        {
            if (
                Values.ContainsKey(name))
            {
                return Values[name];
            }
            throw new Exception(name + "Not Found");
        }
        public void Insert(string name, object val)
        {
            Values[name] = val;
        }
    }
    public class Arguments:Additional
    {
        public Datasets.Dataset Dataset { get; set; }
        public Distances.Distance Distance { get; set; }
    }
    public class Results:Additional
    {

        public Results()
        {
            CM = new List<int>();
        }
        public void Reset()
        {
            CM.Clear();
            Values.Clear();
        }
        public List<int> CM { get; set; }
    }
}
