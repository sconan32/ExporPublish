using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Datasets
{
    public class DAttriInfo:AttrInfo
    {
        List<string> values;

        public DAttriInfo(string name)
            :base(name,AttrType.Discrete)
        {
            values = new List<string>();
        }
        public override void InitAttrValue(AttrValue val)
        {
            val.DValue = int.MaxValue;
        }
        public void SetDiscreteValue(ref AttrValue av, int value)
        {
            av.DValue = value;
        }
        public override double CalcDistance(AttrValue lhs, AttrValue rhs)
        {
            if (IsUnknown(lhs) && IsUnknown(rhs))
            {
                return 0;
            }
            if (IsUnknown(lhs) ^ IsUnknown(rhs))
            {
                return 1;
            }
            if (lhs.DValue == rhs.DValue)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public bool IsUnknown(AttrValue av)
        {
            return av.DValue == double.MaxValue;
        }
        public int AddValue(string s, bool allowDuplicate)
        {
            int ind = values.IndexOf(s);
            if (ind < 0)
            {
                values.Add(s);
                return values.Count - 1;
            }
            else
            {
                if (allowDuplicate)
                {
                    return ind;
                }
                else
                {
                    throw new Exception("Value " + s + " Already Exists");
                    //return -1;
                }
            }
        }
        public int NumValueCount
        {
            get
            {
                return values.Count;
            }
        }
        public int GetIdFromValue(string value)
        {
            return values.IndexOf(value);
        }
        public string GetValueFromId(int index)
        {
            return values[index];
        }
        public override double GetContinuousValue(AttrValue av)
        {
            throw new NotImplementedException();
        }
        public override void SetContinuousValue(ref AttrValue av, double value)
        {
            throw new NotImplementedException();
        }
    }
}
