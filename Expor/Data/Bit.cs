using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Data
{

    /**
     * Provides a bit number. The bit is internally represented as boolean.
     * 
     * @author Arthur Zimek
     */
    public struct Bit : IComparable, IConvertible, IEquatable<Bit>, IFormattable, IComparable<Bit>
    {
        /**
         * Generated serial version UID.
         */
       // private static readonly long serialVersionUID = 390879869314931240L;

        /**
         * Pattern defining valid bit values. A valid bit value is either 0 or 1.
         */
        public static readonly Regex BIT_PATTERN = new Regex("^[01]$");

        /**
         * Method to construct a Bit for a given String expression.
         * 
         * @param bit a String expression defining a Bit
         * @return a Bit as defined by the given String expression
         * @throws NumberFormatException if the given String expression does not fit
         *         to the Pattern {@link #BIT_PATTERN BIT_PATTERN}
         */
        public static Bit ValueOf(String bit)
        {
            if (!BIT_PATTERN.IsMatch(bit))
            {
                throw new FormatException("Input \"" + bit + "\" does not fit required pattern: " + BIT_PATTERN.ToString());
            }
            return new Bit(int.Parse(bit));
        }
        public static implicit operator Bit(bool _bit)
        {
            return new Bit(){bit=_bit};
        }
        public static implicit operator bool(Bit _bit)
        {
            return _bit.bit;
        }
        /**
         * Internal representation of the bit value.
         */
        private bool bit;

        /**
         * Provides a new bit according to the specified boolean value.
         * 
         * @param bit the boolean value of this bit
         */
        public Bit(bool bit)
        {
            this.bit = bit;
        }

        /**
         * Provides a new bit according to the specified integer value. The bit value
         * is 1 for true and 0 for false.
         * 
         * @param bit 1 for true and 0 for false
         * @throws IllegalArgumentException if the specified value is neither 0 nor 1.
         */
        public Bit(int bit)
        {
            if (bit != 0 && bit != 1)
            {
                throw new ArgumentException("Required: 0 or 1 - found: " + bit);
            }
            this.bit = bit == 1;
        }


        /**
         * Returns the bit value as a boolean.
         * 
         * @return the bit value
         */
        public bool BitValue()
        {
            return this.bit;
        }

        /**
         * Provides the String representation of the integer representation of this
         * Bit as given by {@link #intValue() intValue()}.
         * 
         * @see java.lang.Object#toString()
         */

        public override String ToString()
        {
            return Convert.ToInt32(bit).ToString();
        }

        public int CompareTo(object obj)
        {
            if (obj is ValueType)
            {
                if (Convert.ToInt32(obj) > Convert.ToInt32(bit))
                {
                    return 1;
                }
                else if (Convert.ToInt32(obj) == Convert.ToInt32(bit))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }


        public TypeCode GetTypeCode()
        {
            return TypeCode.Boolean;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return bit;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(bit);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(bit);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(bit);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(bit);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(bit);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(bit);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(bit);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(bit);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(bit);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(bit);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(bit);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(bit, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(bit);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(bit);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(bit);
        }

        public bool Equals(Bit other)
        {
            return bit == other.bit;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString();
        }

        public int CompareTo(Bit other)
        {
            if (this.bit && other.bit || !(this.bit || other.bit))
                return 0;
            else
            {
                if (this.bit)
                    return 1;
                else
                    return -1;
            }
        }
    }
}
