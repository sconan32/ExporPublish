using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Distances.DistanceValues
{

    public class BitDistance : NumberDistanceValue<Bit>
    {
        /**
         * The static factory instance
         */
        public static BitDistance FACTORY = new BitDistance();

        /**
         * The distance value
         */
        private bool value;

        /**
         * Generated serial version UID
         */
        // private static long serialVersionUID = 6514853467081717551L;

        /**
         * Empty constructor for serialization purposes.
         */
        public BitDistance() :
            base()
        {
        }

        /**
         * Constructs a new BitDistance object that represents the bit argument.
         * 
         * @param bit the value to be represented by the BitDistance.
         */
        public BitDistance(bool bit) :
            base()
        {
            this.value = bit;
        }

        /**
         * Constructs a new BitDistance object that represents the bit argument.
         * 
         * @param bit the value to be represented by the BitDistance.
         */
        public BitDistance(Bit bit) :
            base()
        {
            this.value = bit;
        }


        public BitDistance FromDouble(double val)
        {
            return new BitDistance(val > 0);
        }


        public BitDistance Plus(BitDistance distance)
        {
            return new BitDistance(this.BitValue() || distance.BitValue());
        }


        public BitDistance Minus(BitDistance distance)
        {
            return new BitDistance(this.BitValue() ^ distance.BitValue());
        }

        /**
         * Returns the value of this BitDistance as a bool.
         * 
         * @return the value as a bool
         */
        public bool BitValue()
        {
            return this.value;
        }

        public override double DoubleValue()
        {
            return value ? 1.0 : 0.0;
        }


        public override long LongValue()
        {
            return value ? 1 : 0;
        }


        public override int IntValue()
        {
            return value ? 1 : 0;
        }

        public int CompareTo(BitDistance other)
        {
            return this.IntValue() - other.IntValue();
        }


        public override IDistanceValue ParseString(String val)
        {
            if (TestInputPattern(val))
            {
                return new BitDistance(Bit.ValueOf(val).BitValue());
            }
            else
            {
                throw new ArgumentException("Given pattern \"" + val +
                    "\" does not match required pattern \"" + RequiredInputPattern + "\"");
            }
        }

        public override IDistanceValue Infinity
        {
            get { return new BitDistance(true); }
        }


        public override IDistanceValue Empty
        {
            get { return new BitDistance(false); }
        }


        public override IDistanceValue Undefined
        {
            get { throw new InvalidOperationException(ExceptionMessages.UNSUPPORTED_UNDEFINED_DISTANCE); }
        }


        public override Regex GetPattern()
        {
            return Bit.BIT_PATTERN;
        }


        public override bool IsInfinity
        {
            get { return false; }
        }


        public override bool IsEmpty
        {
            get { return (value == false); }
        }


        public override bool IsUndefined
        {
            get { return false; }
        }

        public override Bit Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }



        public override int CompareTo(IDistanceValue obj)
        {
            return this.value.CompareTo((obj as BitDistance).value);
        }


    }
}
