using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Datasets
{
    public abstract class AttrInfo:IEquatable<AttrInfo>
    {
        

        public string Name { get; set; }
        public AttrType Type { get;protected set; }


        public abstract double CalcDistance(AttrValue lhs, AttrValue rhs);

        public static bool operator==(AttrInfo lhs, AttrInfo rhs)
        {
            return ((object)lhs == null && (object)rhs==null)||
                ((object)lhs!=null && lhs.Equals(rhs));
        }
        public static bool operator !=(AttrInfo lhs, AttrInfo rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            return obj!=null && this.GetHashCode()==obj.GetHashCode();
        }
        public bool Equals(AttrInfo other)
        {
            return other!=null && this.GetHashCode() == other.GetHashCode();
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Type.GetHashCode();
        }
        public AttrInfo(string name, AttrType type)
        {
            Name = name;
            Type = type;
        }
        public abstract void InitAttrValue(AttrValue val);
        public abstract double GetContinuousValue(AttrValue av);
        public abstract void SetContinuousValue(ref AttrValue av, double value);
    }
    public enum AttrType
    {
        Unknown,
        Continuous,
        Discrete
    }
}
