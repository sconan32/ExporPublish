using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Datasets
{
    class CAttrInfo:AttrInfo
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public CAttrInfo(string name)
            :base(name, AttrType.Continuous)
        {
            Min = Max = 0;
        }
        public override void InitAttrValue(AttrValue val)
        {
            val.CValue = double.MaxValue;
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
            return lhs.CValue - rhs.CValue;
        }
        public void SetContinousValue(ref AttrValue av, double value)
        {
            av.CValue = value;
        }
        public bool IsUnknown(AttrValue av)
        {
            if (av.CValue == double.MaxValue)
            { return true; }
            return false;
        }
        public override double GetContinuousValue(AttrValue av)
        {
            return av.CValue;
        }
        public override void SetContinuousValue(ref AttrValue av, double value)
        {
            av.CValue = value;
        }
    }
}
