using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Distances.DistanceValues
{

    public class Int32DistanceValue : NumberDistanceValue<int>
    {
        /**
         * The static factory instance
         */
        public static Int32DistanceValue FACTORY = new Int32DistanceValue();

        /**
         * The distance value
         */
        int value;

        /**
         * Created serial version UID.
         */
        // private static long serialVersionUID = 5583821082931825810L;

        /**
         * Empty constructor for serialization purposes.
         */
        public Int32DistanceValue()
            : base()
        {
        }

        /**
         * Constructor
         * 
         * @param value distance value
         */
        public Int32DistanceValue(int value)
            : base()
        {
            this.value = value;
        }




        public IDistanceValue Minus(IDistanceValue distance)
        {
            return new Int32DistanceValue(this.Value - ((Int32DistanceValue)distance).Value);
        }


        public IDistanceValue Plus(IDistanceValue distance)
        {
            return new Int32DistanceValue(this.Value + ((Int32DistanceValue)distance).Value);
        }


        public override Int32 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override int CompareTo(IDistanceValue other)
        {
            Int32DistanceValue iother = other as Int32DistanceValue;
            return (this.value < iother.value ? -1 : (this.value == iother.value ? 0 : 1));
        }


        public override IDistanceValue Empty
        {
            get { return new Int32DistanceValue(0); }
        }


        public override IDistanceValue Undefined
        {
            get { throw new InvalidOperationException(ExceptionMessages.UNSUPPORTED_UNDEFINED_DISTANCE); }
        }


        public override IDistanceValue Infinity
        {
            get { return new Int32DistanceValue(Int32.MaxValue); }
        }


        public override bool IsInfinity
        {
            get { return value == Int32.MaxValue; }
        }


        public override bool IsEmpty
        {
            get { return value == 0; }
        }


        public override bool IsUndefined
        {
            get { return false; }
        }


        public override IDistanceValue ParseString(String val)
        {
            if (TestInputPattern(val))
            {
                return new Int32DistanceValue(Int32.Parse(val));
            }
            else
            {
                throw new ArgumentException("Given pattern \"" + val +
                    "\" does not match required pattern \"" + RequiredInputPattern + "\"");
            }
        }

        public override Regex GetPattern()
        {
            return INTEGER_PATTERN;
        }
        public override long LongValue()
        {
            return value;
        }
        public override double DoubleValue()
        {
            return value;
        }




    }
}
