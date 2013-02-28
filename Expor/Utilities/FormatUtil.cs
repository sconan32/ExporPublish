using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Socona.Expor.Maths.LinearAlgebra;

namespace Socona.Expor.Utilities
{

    /**
     * Utility methods for output Formatting of various number objects
     */
    public class FormatUtil
    {
        /**
         * Dynamic number Formatter, but with language constraint.
         */
        public static NumberFormatInfo NF = new NumberFormatInfo();

        /**
         * Number Formatter (0 digits) for output purposes.
         */
        public static NumberFormatInfo NF0 = new NumberFormatInfo();

        /**
         * Number Formatter (2 digits) for output purposes.
         */
        public static NumberFormatInfo NF2 = new NumberFormatInfo();

        /**
         * Number Formatter (4 digits) for output purposes.
         */
        public static NumberFormatInfo NF4 = new NumberFormatInfo();

        /**
         * Number Formatter (6 digits) for output purposes.
         */
        public static NumberFormatInfo NF6 = new NumberFormatInfo();

        /**
         * Number Formatter (8 digits) for output purposes.
         */
        public static NumberFormatInfo NF8 = new NumberFormatInfo();

        static FormatUtil()
        {
            NF.NumberDecimalDigits = 10;
            NF.NumberGroupSeparator = string.Empty;
            NF0.NumberDecimalDigits = 0;
            NF0.NumberGroupSeparator = string.Empty;
            NF2.NumberDecimalDigits = 2;
            NF2.NumberGroupSeparator = string.Empty;
            NF4.NumberDecimalDigits = 4;
            NF4.NumberGroupSeparator = string.Empty;
            NF6.NumberDecimalDigits = 6;
            NF6.NumberGroupSeparator = string.Empty;
            NF8.NumberDecimalDigits = 8;
            NF8.NumberGroupSeparator = string.Empty;
        }

        /**
         * Whitespace. The string should cover the commonly used Length.
         */
        private static String WHITESPACE_BUFFER = "                                                                                ";

        /**
         * The system newline setting.
         */
        public static String NEWLINE = Environment.NewLine;

        /**
         * Non-breaking unicode space character.
         */
        public static String NONBREAKING_SPACE = "\u00a0";

        /**
         * The time unit sizes: ms, s, m, h, d; all in ms.
         */
        private static long[] TIME_UNIT_SIZES = new long[] { 1L, 1000L, 60000L, 3600000L, 86400000L };

        /**
         * The strings used in serialization
         */
        private static String[] TIME_UNIT_NAMES = new String[] { "ms", "s", "m", "h", "d" };

        /**
         * The number of digits used for Formatting
         */
        private static int[] TIME_UNIT_DIGITS = new int[] { 3, 2, 2, 2, 2 };

        /**
         * Formats the double d with the specified fraction digits.
         * 
         * @param d the double array to be Formatted
         * @param digits the number of fraction digits
         * @return a String representing the double d
         */
        public static String Format(double d, int digits)
        {
            NumberFormatInfo nf = new CultureInfo("en-US").NumberFormat;
            nf.NumberDecimalDigits = digits;

            return d.ToString("N", nf);
        }

        /**
         * Formats the double d with the specified number Format.
         * 
         * @param d the double array to be Formatted
         * @param nf the number Format to be used for Formatting
         * @return a String representing the double d
         */
        public static String Format(double d, NumberFormatInfo nf)
        {
            return d.ToString("N", nf);
        }

        /**
         * Formats the double d with 2 fraction digits.
         * 
         * @param d the double to be Formatted
         * @return a String representing the double d
         */
        public static String Format(double d)
        {
            return Format(d, 4);
        }

        /**
         * Formats the double array d with the specified separator.
         * 
         * @param d the double array to be Formatted
         * @param sep the separator between the single values of the double array,
         *        e.g. ','
         * @return a String representing the double array d
         */
        public static String Format(double[] d, String sep)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i > 0)
                {
                    buffer.Append(sep).Append(Format(d[i]));
                }
                else
                {
                    buffer.Append(Format(d[i]));
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the double array d with the specified separator and the specified
         * fraction digits.
         * 
         * @param d the double array to be Formatted
         * @param sep the separator between the single values of the double array,
         *        e.g. ','
         * @param digits the number of fraction digits
         * @return a String representing the double array d
         */
        public static String Format(double[] d, String sep, int digits)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i < d.Length - 1)
                {
                    buffer.Append(Format(d[i], digits)).Append(sep);
                }
                else
                {
                    buffer.Append(Format(d[i], digits));
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the double array d with the specified number Format.
         * 
         * @param d the double array to be Formatted
         * @param nf the number Format to be used for Formatting
         * @return a String representing the double array d
         */
        public static String Format(double[] d, NumberFormatInfo nf)
        {
            return Format(d, " ", nf);
        }

        /**
         * Formats the double array d with the specified number Format.
         * 
         * @param d the double array to be Formatted
         * @param sep the separator between the single values of the double array,
         *        e.g. ','
         * @param nf the number Format to be used for Formatting
         * @return a String representing the double array d
         */
        public static String Format(double[] d, String sep, NumberFormatInfo nf)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i < d.Length - 1)
                {
                    buffer.Append(Format(d[i], nf)).Append(sep);
                }
                else
                {
                    buffer.Append(Format(d[i], nf));
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the double array d with ',' as separator and 2 fraction digits.
         * 
         * @param d the double array to be Formatted
         * @return a String representing the double array d
         */
        public static String Format(double[] d)
        {
            return Format(d, ", ", 4);
        }

        /**
         * Formats the double array d with ', ' as separator and with the specified
         * fraction digits.
         * 
         * @param d the double array to be Formatted
         * @param digits the number of fraction digits
         * @return a String representing the double array d
         */
        public static String Format(double[] d, int digits)
        {
            return Format(d, ", ", digits);
        }

        /**
         * Formats the double array d with ',' as separator and 2 fraction digits.
         * 
         * @param d the double array to be Formatted
         * @return a String representing the double array d
         */
        public static String Format(double[,] d)
        {
            StringBuilder buffer = new StringBuilder();
            // foreach (double[] array in d)
            for (int i = 0; i < d.Length; i++)
            {
                for (int j = 0; j < d.GetLength(1); j++)
                {

                    buffer.Append(Format(d[i, j], 4)).Append(", ");
                }
                buffer.Append("\n");
            }
            return buffer.ToString();
        }

        /**
         * Formats the array of double arrays d with 'the specified separators and
         * fraction digits.
         * 
         * @param d the double array to be Formatted
         * @param sep1 the first separator of the outer array
         * @param sep2 the second separator of the inner array
         * @param digits the number of fraction digits
         * @return a String representing the double array d
         */
        public static String Format(double[,] d, String sep1, String sep2, int digits)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < d.Length; i++)
            {
                for (int j = 0; j < d.GetLength(2); j++)
                {
                    buffer.Append(Format(d[i, j], digits)).Append(sep2);
                }
                if (i < d.Length - 1)
                {

                    buffer.Append(sep1);
                }

            }

            return buffer.ToString();
        }

        ///**
        // * Formats the Double array f with the specified separator and the specified
        // * fraction digits.
        // * 
        // * @param f the Double array to be Formatted
        // * @param sep the separator between the single values of the Double array,
        // *        e.g. ','
        // * @param digits the number of fraction digits
        // * @return a String representing the Double array f
        // */
        //public static String Format(Double[] f, String sep, int digits) {
        //  StringBuilder buffer = new StringBuilder();
        //  for(int i = 0; i < f.Length; i++) {
        //    if(i < f.Length - 1) {
        //      buffer.Append(Format(f[i], digits)).Append(sep);
        //    }
        //    else {
        //      buffer.Append(Format(f[i], digits));
        //    }
        //  }
        //  return buffer.ToString();
        //}

        /**
         * Formats the Double array f with ',' as separator and 2 fraction digits.
         * 
         * @param f the Double array to be Formatted
         * @return a String representing the Double array f
         */
        //public static String Format(Double[] f) {
        //  return Format(f, ", ", 2);
        // }

        ///**
        // * Formats the Double array f with the specified separator and the specified
        // * fraction digits.
        // * 
        // * @param f the Double array to be Formatted
        // * @param sep the separator between the single values of the Double array,
        // *        e.g. ','
        // * @param nf the number Format
        // * @return a String representing the Double array f
        // */
        //public static String Format(Double[] f, String sep, NumberFormatInfo nf) {
        //  StringBuilder buffer = new StringBuilder();
        //  for(int i = 0; i < f.Length; i++) {
        //    if(i < f.Length - 1) {
        //      buffer.Append(Format(f[i], nf)).Append(sep);
        //    }
        //    else {
        //      buffer.Append(Format(f[i], nf));
        //    }
        //  }
        //  return buffer.ToString();
        //}

        /**
         * Formats the Double array f with ',' as separator and 2 fraction digits.
         * 
         * @param f the Double array to be Formatted
         * @param nf the Number Format
         * @return a String representing the Double array f
         */
        //public static String Format(double[] f, NumberFormatInfo nf) {
        //  return Format(f, ", ", nf);
        // }

        /**
         * Formats the float array f with the specified separator and the specified
         * fraction digits.
         * 
         * @param f the float array to be Formatted
         * @param sep the separator between the single values of the float array, e.g.
         *        ','
         * @param digits the number of fraction digits
         * @return a String representing the float array f
         */
        public static String Format(float[] f, String sep, int digits)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < f.Length; i++)
            {
                if (i < f.Length - 1)
                {
                    buffer.Append(Format(f[i], digits)).Append(sep);
                }
                else
                {
                    buffer.Append(Format(f[i], digits));
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the float array f with ',' as separator and 2 fraction digits.
         * 
         * @param f the float array to be Formatted
         * @return a String representing the float array f
         */
        public static String Format(float[] f)
        {
            return Format(f, ", ", 2);
        }

        /**
         * Formats the int array a for printing purposes.
         * 
         * @param a the int array to be Formatted
         * @param sep the separator between the single values of the float array, e.g.
         *        ','
         * @return a String representing the int array a
         */
        public static String Format(int[] a, String sep)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i < a.Length - 1)
                {
                    buffer.Append(a[i]).Append(sep);
                }
                else
                {
                    buffer.Append(a[i]);
                }
            }
            return buffer.ToString();
        }
        public static string Format(IEnumerable arr, string sep)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in arr)
            {
                sb.Append(item.ToString());
                sb.Append(sep);
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public static string Format(IEnumerable arr)
        {
            return Format(arr, ", ");
        }
        /**
         * Formats the int array a for printing purposes.
         * 
         * @param a the int array to be Formatted
         * @return a String representing the int array a
         */
        public static String Format(int[] a)
        {
            return Format(a, ", ");
        }

        ///**
        // * Formats the Integer array a for printing purposes.
        // * 
        // * @param a the Integer array to be Formatted
        // * @param sep the separator between the single values of the float array, e.g.
        // *        ','
        // * @return a String representing the Integer array a
        // */
        //public static String Format(Int32[] a, String sep) {
        //  StringBuilder buffer = new StringBuilder();
        //  for(int i = 0; i < a.Length; i++) {
        //    if(i < a.Length - 1) {
        //      buffer.Append(a[i]).Append(sep);
        //    }
        //    else {
        //      buffer.Append(a[i]);
        //    }
        //  }
        //  return buffer.ToString();
        //}

        ///**
        // * Formats the Integer array a for printing purposes.
        // * 
        // * @param a the Integer array to be Formatted
        // * @return a String representing the Integer array a
        // */
        //public static String Format(Int32[] a) {
        //  return Format(a, ", ");
        //}

        /**
         * Formats the long array a for printing purposes.
         * 
         * @param a the long array to be Formatted
         * @return a String representing the long array a
         */
        public static String Format(long[] a)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i < a.Length - 1)
                {
                    buffer.Append(a[i]).Append(", ");
                }
                else
                {
                    buffer.Append(a[i]);
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the byte array a for printing purposes.
         * 
         * @param a the byte array to be Formatted
         * @return a String representing the byte array a
         */
        public static String Format(byte[] a)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i < a.Length - 1)
                {
                    buffer.Append(a[i]).Append(", ");
                }
                else
                {
                    buffer.Append(a[i]);
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the boolean array b with ',' as separator.
         * 
         * @param b the boolean array to be Formatted
         * @param sep the separator between the single values of the double array,
         *        e.g. ','
         * @return a String representing the boolean array b
         */
        public static String Format(bool[] b, String sep)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                if (i < b.Length - 1)
                {
                    buffer.Append(Format(b[i])).Append(sep);
                }
                else
                {
                    buffer.Append(Format(b[i]));
                }
            }
            return buffer.ToString();
        }

        /**
         * Formats the boolean b.
         * 
         * @param b the boolean to be Formatted
         * @return a String representing of the boolean b
         */
        public static String Format(bool b)
        {
            if (b)
            {
                return "1";
            }
            return "0";
        }

        /**
         * Returns a string representation of the specified bit set.
         * 
         * @param bitSet the bitSet
         * @param dim the overall dimensionality of the bit set
         * @param sep the separator
         * @return a string representation of the specified bit set.
         */
        public static String Format(BitArray bitSet, int dim, String sep)
        {
            StringBuilder msg = new StringBuilder();

            for (int d = 0; d < dim; d++)
            {
                if (d > 0)
                {
                    msg.Append(sep);
                }
                if (bitSet[d])
                {
                    msg.Append("1");
                }
                else
                {
                    msg.Append("0");
                }
            }

            return msg.ToString();
        }

        /**
         * Returns a string representation of the specified bit set.
         * 
         * @param dim the overall dimensionality of the bit set
         * @param bitSet the bitSet
         * @return a string representation of the specified bit set.
         */
        public static String Format(int dim, BitArray bitSet)
        {
            // TODO: removed whitespace - hierarchy reading to be adapted!
            return Format(bitSet, dim, ",");
        }

        /**
         * Formats the String collection with the specified separator.
         * 
         * @param d the String collection to Format
         * @param sep the separator between the single values of the double array,
         *        e.g. ' '
         * @return a String representing the String Collection d
         */
        public static String Format(ICollection<String> d, String sep = ", ")
        {
            if (d.Count == 0)
            {
                return "";
            }
            if (d.Count == 1)
            {
                return d.ElementAt<string>(0);
            }
            StringBuilder buffer = new StringBuilder();
            bool first = true;
            foreach (String str in d)
            {
                if (!first)
                {
                    buffer.Append(sep);
                }
                buffer.Append(str);
                first = false;
            }
            return buffer.ToString();
        }

        /**
         * Returns a string representation of this matrix.
         * 
         * @param w column width
         * @param d number of digits after the decimal
         * @return a string representation of this matrix
         */
        // TODO: in use?
        //public static String Format(Matrix m, int w, int d) {
        //  DecimalFormat Format = new DecimalFormat();
        //  Format.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));
        //  Format.setMinimumIntegerDigits(1);
        //  Format.setMaximumFractionDigits(d);
        //  Format.setMinimumFractionDigits(d);
        //  Format.setGroupingUsed(false);

        //  int width = w + 1;
        //  StringBuilder msg = new StringBuilder();
        //  msg.Append("\n"); // start on new line.
        //  for(int i = 0; i < m.getRowDimensionality(); i++) {
        //    for(int j = 0; j < m.getColumnDimensionality(); j++) {
        //      String s = Format.Format(m.get(i, j)); // Format the number
        //      int padding =System. Math.Max(1, width - s.Length); // At _least_ 1
        //      // space
        //      for(int k = 0; k < padding; k++) {
        //        msg.Append(' ');
        //      }
        //      msg.Append(s);
        //    }
        //    msg.Append("\n");
        //  }
        //  // msg.Append("\n");

        //  return msg.ToString();
        //}

        /**
         * Returns a string representation of this matrix.
         * 
         * @param w column width
         * @param d number of digits after the decimal
         * @return a string representation of this matrix
         */
        // TODO: in use?
        //public static String Format(Vector v, int w, int d) {
        //  DecimalFormat format = new DecimalFormat();
        //  format.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));
        //  format.setMinimumIntegerDigits(1);
        //  format.setMaximumFractionDigits(d);
        //  format.setMinimumFractionDigits(d);
        //  format.setGroupingUsed(false);

        //  int width = w + 1;
        //  StringBuilder msg = new StringBuilder();
        //  msg.Append("\n"); // start on new line.
        //  for(int i = 0; i < v.getDimensionality(); i++) {
        //    String s = format.Format(v.get(i)); // Format the number
        //    int padding = System.Math.Max(1, width - s.Length); // At _least_ 1
        //    // space
        //    for(int k = 0; k < padding; k++) {
        //      msg.Append(' ');
        //    }
        //    msg.Append(s);
        //  }
        //  // msg.Append("\n");

        //  return msg.ToString();
        //}

        /**
         * Returns a string representation of this matrix. In each line the specified
         * String <code>pre</code> is prefixed.
         * 
         * @param pre the prefix of each line
         * @return a string representation of this matrix
         */
        public static String Format(Matrix m, String pre)
        {
            StringBuilder output = new StringBuilder();
            output.Append(pre).Append("[\n").Append(pre);
            for (int i = 0; i < m.RowCount; i++)
            {
                output.Append(" [");
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    output.Append(" ").Append(Format(m[i, j]));
                    if (j < m.ColumnCount - 1)
                    {
                        output.Append(",");
                    }
                }
                output.Append(" ]\n").Append(pre);
            }
            output.Append("]\n").Append(pre);

            return (output.ToString());
        }

        /**
         * returns String-representation of Matrix.
         * 
         * @param nf NumberFormatInfo to specify output precision
         * @return String representation of this Matrix in precision as specified by
         *         given NumberFormatInfo
         */
        public static String Format(Matrix m, NumberFormatInfo nf)
        {
            int[] colMax = new int[m.ColumnCount];
            String[,] entries = new String[m.RowCount, m.ColumnCount];
            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    entries[i, j] = m[i, j].ToString("N", nf);
                    if (entries[i, j].Length > colMax[j])
                    {
                        colMax[j] = entries[i, j].Length;
                    }
                }
            }
            StringBuilder output = new StringBuilder();
            output.Append("[\n");
            for (int i = 0; i < m.RowCount; i++)
            {
                output.Append(" [");
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    output.Append(" ");
                    int space = colMax[j] - entries[i, j].Length;
                    for (int s = 0; s < space; s++)
                    {
                        output.Append(" ");
                    }
                    output.Append(entries[i, j]);
                    if (j < m.ColumnCount - 1)
                    {
                        output.Append(",");
                    }
                }
                output.Append(" ]\n");
            }
            output.Append("]\n");

            return (output.ToString());
        }

        /**
         * returns String-representation of Matrix.
         * 
         * @return String representation of this Matrix
         */
        //public static String Format(Matrix m) {
        //  return Format(m, FormatUtil.NF8);
        //}

        /**
         * returns String-representation of Vector.
         * 
         * @param nf NumberFormatInfo to specify output precision
         * @return String representation of this Matrix in precision as specified by
         *         given NumberFormatInfo
         */
        //public static String Format(Vector m, NumberFormatInfo nf) {
        //  return "[" + FormatUtil.Format(m.getArrayRef(), nf) + "]";
        //}

        /**
         * Returns String-representation of Vector.
         * 
         * @return String representation of this Vector
         */
        //public static String Format(Vector m) {
        //  return Format(m, FormatUtil.NF8);
        //}

        /**
         * Returns a string representation of this matrix. In each line the specified
         * String <code>pre</code> is prefixed.
         * 
         * @param pre the prefix of each line
         * @return a string representation of this matrix
         */
        //public static String Format(Vector v, String pre) {
        //  StringBuilder output = new StringBuilder();
        //  output.Append(pre).Append("[\n").Append(pre);
        //  for(int j = 0; j < v.getDimensionality(); j++) {
        //    output.Append(" ").Append(v.get(j));
        //    if(j < v.getDimensionality() - 1) {
        //      output.Append(",");
        //    }
        //  }
        //  output.Append("]\n").Append(pre);

        //  return (output.ToString());
        //}

        /**
         * Returns a string representation of this matrix. In each line the specified
         * String <code>pre<\code> is prefixed.
         * 
         * @param nf number Format for output accuracy
         * @param pre the prefix of each line
         * @return a string representation of this matrix
         */
        //public static String Format(Matrix m, String pre, NumberFormatInfo nf) {
        //  if(nf == null) {
        //    return FormatUtil.Format(m, pre);
        //  }

        //  int[] colMax = new int[m.getColumnDimensionality()];
        //  String[][] entries = new String[m.getRowDimensionality()][m.getColumnDimensionality()];
        //  for(int i = 0; i < m.getRowDimensionality(); i++) {
        //    for(int j = 0; j < m.getColumnDimensionality(); j++) {
        //      entries[i][j] = nf.Format(m.get(i, j));
        //      if(entries[i][j].Length() > colMax[j]) {
        //        colMax[j] = entries[i][j].Length();
        //      }
        //    }
        //  }
        //  StringBuilder output = new StringBuilder();
        //  output.Append(pre).Append("[\n").Append(pre);
        //  for(int i = 0; i < m.getRowDimensionality(); i++) {
        //    output.Append(" [");
        //    for(int j = 0; j < m.getColumnDimensionality(); j++) {
        //      output.Append(" ");
        //      int space = colMax[j] - entries[i][j].Length();
        //      for(int s = 0; s < space; s++) {
        //        output.Append(" ");
        //      }
        //      output.Append(entries[i][j]);
        //      if(j < m.getColumnDimensionality() - 1) {
        //        output.Append(",");
        //      }
        //    }
        //    output.Append(" ]\n").Append(pre);
        //  }
        //  output.Append("]\n").Append(pre);

        //  return (output.ToString());
        //}

        /**
         * Find the first space before position w or if there is none after w.
         * 
         * @param s String
         * @param width Width
         * @return index of best whitespace or <code>-1</code> if no whitespace was
         *         found.
         */
        public static int FindSplitPoint(String s, int width)
        {
            // the newline (or EOS) is the fallback split position.
            int in1 = s.IndexOf(NEWLINE);
            if (in1 < 0)
            {
                in1 = s.Length;
            }
            // Good enough?
            if (in1 < width)
            {
                return in1;
            }
            // otherwise, search for whitespace
            int iw = s.LastIndexOf(' ', width);
            // good whitespace found?
            if (iw >= 0 && iw < width)
            {
                return iw;
            }
            // sub-optimal splitpoint - retry AFTER the given position
            int bp = NextPosition(s.IndexOf(' ', width), s.IndexOf(NEWLINE, width));
            if (bp >= 0)
            {
                return bp;
            }
            // even worse - can't split!
            return s.Length;
        }

        /**
         * Helper that is similar to {@code Math.min(a,b)}, except that negative
         * values are considered "invalid".
         * 
         * @param a String position
         * @param b String position
         * @return {@code Math.min(a,b)} if {@code a >= 0} and {@code b >= 0},
         *         otherwise whichever is positive.
         */
        private static int NextPosition(int a, int b)
        {
            if (a < 0)
            {
                return b;
            }
            if (b < 0)
            {
                return a;
            }
            return Math.Min(a, b);
        }

        /**
         * Splits the specified string at the last blank before width. If there is no
         * blank before the given width, it is split at the next.
         * 
         * @param s the string to be split
         * @param width int
         * @return string fragments
         */
        public static List<String> SplitAtLastBlank(String s, int width)
        {
            List<String> chunks = new List<String>();

            String tmp = s;
            while (tmp.Length > 0)
            {
                int index = FindSplitPoint(tmp, width);
                // store first part
                chunks.Add(tmp.Substring(0, index));
                // skip whitespace at beginning of line
                while (index < tmp.Length && tmp[index] == ' ')
                {
                    index += 1;
                }
                // remove a newline
                if (index < tmp.Length && tmp.IndexOf(NEWLINE, index) >= 0)
                {
                    index += NEWLINE.Length;
                }
                if (index >= tmp.Length)
                {
                    break;
                }
                tmp = tmp.Substring(index);
            }

            return chunks;
        }

        /**
         * Returns a string with the specified number of whitespace.
         * 
         * @param n the number of whitespace characters
         * @return a string with the specified number of blanks
         */
        public static String Whitespace(int n)
        {
            if (n < WHITESPACE_BUFFER.Length)
            {
                return WHITESPACE_BUFFER.Substring(0, n);
            }
            char[] buf = new char[n];
            for (int i = 0; i < n; i++)
            {
                buf[i] = WHITESPACE_BUFFER[0];
            }
            return new String(buf);
        }

        /**
         * Pad a string to a given Length by adding whitespace to the right.
         * 
         * @param o original string
         * @param len destination Length
         * @return padded string of at least Length len (and o otherwise)
         */
        public static String Pad(String o, int len)
        {
            if (o.Length >= len)
            {
                return o;
            }
            return o + Whitespace(len - o.Length);
        }

        /**
         * Pad a string to a given Length by adding whitespace to the left.
         * 
         * @param o original string
         * @param len destination Length
         * @return padded string of at least Length len (and o otherwise)
         */
        public static String PadRightAligned(String o, int len)
        {
            if (o.Length >= len)
            {
                return o;
            }
            return Whitespace(len - o.Length) + o;
        }

        /**
         * Get the width of the terminal window (on Unix xterms), with a default of 78
         * characters.
         * 
         * @return Terminal width
         */
        public static int GetConsoleWidth()
        {
            int termwidth = 78;
            try
            {
                // termwidth = Int32.Parse(System.getenv("COLUMNS")) - 1;
            }
            catch (Exception)
            {
                // Do nothing, stick with default of 78.
            }
            return termwidth;
        }

        /**
         * Formats a time delta in human readable Format.
         * 
         * @param time time delta in ms
         * @return Formatted string
         */
        //public static String FormatTimeDelta(long time, CharSequence sep) {
        // StringBuilder sb = new StringBuilder();
        //   Formatter fmt = new Formatter(sb);

        //  for(int i = TIME_UNIT_SIZES.Length - 1; i >= 0; --i) {
        //    // We do not include ms if we are in the order of minutes.
        //    if(i == 0 && sb.Length > 4) {
        //      continue;
        //    }
        //    // Separator
        //    if(sb.Length > 0) {
        //      sb.Append(sep);
        //    }
        //     long acValue = time / TIME_UNIT_SIZES[i];
        //    time = time % TIME_UNIT_SIZES[i];
        //    if(!(acValue == 0 && sb.Length() == 0)) {
        //      fmt.Format("%0" + TIME_UNIT_DIGITS[i] + "d%s", acValue, TIME_UNIT_NAMES[i]);
        //    }
        //  }
        //  return sb.ToString();
        //}

        /**
         * Formats the string array d with the specified separator.
         * 
         * @param d the string array to be Formatted
         * @param sep the separator between the single values of the double array,
         *        e.g. ','
         * @return a String representing the string array d
         */
        public static String Format(String[] d, String sep)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i > 0)
                {
                    buffer.Append(sep).Append(d[i]);
                }
                else
                {
                    buffer.Append(d[i]);
                }
            }
            return buffer.ToString();
        }
    }
}
