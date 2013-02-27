using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Socona.Clustering.Datasets
{
    [StructLayout(LayoutKind.Explicit)]
    public struct  AttrValue
    {
        [FieldOffset(0)]
        public double CValue;
        [FieldOffset(0)]
        public int DValue;

        public static implicit operator AttrValue(double d)
        {
            AttrValue attr=new AttrValue();
            attr.CValue = d;
            return attr;
        }
        public static implicit operator AttrValue(int i)
        {
            AttrValue attr = new AttrValue();
            attr.DValue = i;
            return attr;
        }
        public override string ToString()
        {
            return "C=" + CValue + " D=" + DValue;
        }
    }
    //public struct CAttrValue
    //{
    //    public double value;
    //}
    //public struct DAttrValue
    //{
    //    public int value;
    //}
}
