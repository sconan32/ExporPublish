﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Extenstions;
namespace Socona.Expor.Utilities
{

    /**
     * Utilities for bit operations.
     * 
     * Implementation note: words are stored in little-endian word order. This can
     * be a bit confusing, because a shift-right means "left" on the word level.
     * 
     * Naming: methods with a <code>C</code> return a copy, methods with
     * <code>I</code> modify in-place.
     * 
     * @author Erich Schubert
     */
    public class BitsUtil
    {
        /**
         * Shift factor for a long: 2^6 == 64 == Long.SIZE
         */
        private static int LONG_LOG2_SIZE = 6;

        /** Masking for long shifts. */
      //  private static int LONG_LOG2_MASK = 0x3f; // 6 bits

        /** Long with all bits set */
        private static long LONG_ALL_BITS = -1L;

        /** Long, with 63 bits set */
      //  private static long LONG_63_BITS = 0x7FFFFFFFFFFFFFFFL;

        /** Masking 32 bit **/
    //    private static long LONG_32_BITS = 0xFFFFFFFFL;

        /** Precomputed powers of 5 for pow5, pow10 on the bit representation. */
        private static int[] POW5_INT = {   1, 5, 25, 125, 625,//
            3125, 15625, 78125, 390625, 1953125,//
            9765625, 48828125, 244140625, 1220703125 };

        /**
         * Allocate a new long[].
         * 
         * @param bits Number of bits in storage
         * @return New array
         */
        public static long[] Zero(int bits)
        {
            return new long[((bits - 1) >> LONG_LOG2_SIZE) + 1];
        }

        /**
         * Allocate a new long[].
         * 
         * @param bits Number of bits in storage
         * @param init Initial value (of at most the size of a long, remaining bits
         *        will be 0)
         * @return New array
         */
        public static long[] Make(int bits, long init)
        {
            long[] v = new long[((bits - 1) >> LONG_LOG2_SIZE) + 1];
            v[0] = init;
            return v;
        }

        /**
         * Create a vector initialized with "bits" ones.
         * 
         * @param bits Size
         * @return new vector
         */
        //public static long[] Ones(int bits) {
        //  long[] v = new long[((bits - 1) >> LONG_LOG2_SIZE) + 1];
        //   int fillWords = bits >>LONG_LOG2_SIZE;
        //   int fillBits = bits & LONG_LOG2_MASK;
        //  v.Fill(v, 0, fillWords, LONG_ALL_BITS);
        //  v[v.length - 1] = (1L << fillBits) - 1;
        //  return v;
        //}

        ///**
        // * Copy a bitset
        // * 
        // * @param v Array to copy
        // * @return Copy
        // */
        //public static long[] copy(long[] v) {
        //  return Arrays.copyOf(v, v.length);
        //}

        ///**
        // * Copy a bitset.
        // * 
        // * Note: Bits beyond mincap <em>may</em> be retained!
        // * 
        // * @param v Array to copy
        // * @param mincap Target <em>minimum</em> capacity
        // * @return Copy with space for at least "capacity" bits
        // */
        //public static long[] copy(long[] v, int mincap) {
        //  int words = ((mincap - 1) >> LONG_LOG2_SIZE) + 1;
        //  if (v.length == words) {
        //    return Arrays.copyOf(v, v.length);
        //  }
        //  long[] ret = new long[words];
        //  Array.Copy(v, 0, ret, 0, Math.Min(v.length, words));
        //  return ret;
        //}

        ///**
        // * Copy a bitset.
        // * 
        // * Note: Bits beyond mincap <em>may</em> be retained!
        // * 
        // * @param v Array to copy
        // * @param mincap Target <em>minimum</em> capacity
        // * @param shift Number of bits to shift left
        // * @return Copy with space for at least "capacity" bits
        // */
        //public static long[] copy(long[] v, int mincap, int shift) {
        //  int words = ((mincap - 1) >> LONG_LOG2_SIZE) + 1;
        //  if (v.length == words && shift == 0) {
        //    return Arrays.copyOf(v, v.length);
        //  }
        //  long[] ret = new long[words];
        //   int shiftWords = shift >> LONG_LOG2_SIZE;
        //   int shiftBits = shift & LONG_LOG2_MASK;
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //    for (int i = shiftWords; i < ret.length; i++) {
        //      ret[i] |= v[i - shiftWords];
        //    }
        //    return ret;
        //  }
        //  // Overlapping case
        //   int unshiftBits = Long.SIZE - shiftBits;
        //   int end = Math.min(ret.length, v.length + shiftWords) - 1;
        //  for (int i = end; i > shiftWords; i--) {
        //     int src = i - shiftWords;
        //    ret[i] |= (v[src] << shiftBits) | (v[src - 1] >> unshiftBits);
        //  }
        //  ret[shiftWords] |= v[0] << shiftBits;
        //  return ret;
        //}

        ///**
        // * Compute corresponding gray code as v XOR (v >> 1)
        // * 
        // * @param v Value
        // * @return Gray code
        // */
        //public static long grayC(long v) {
        //  return v ^ (v >> 1);
        //}

        ///**
        // * Compute corresponding gray code as v XOR (v >> 1)
        // * 
        // * @param v Value
        // * @return Gray code
        // */
        //public static long[] grayI(long[] v) {
        //  // TODO: copy less
        //  long[] t = copy(v);
        //  shiftRightI(t, 1);
        //  xorI(v, t);
        //  return v;
        //}

        ///**
        // * Compute the inverted gray code, v XOR (v >> 1) XOR (v >> 2) ...
        // * 
        // * @param v Value
        // * @return Inverted gray code
        // */
        //public static long invgrayC(long v) {
        //  v ^= (v >> 1);
        //  v ^= (v >> 2);
        //  v ^= (v >> 4);
        //  v ^= (v >> 8);
        //  v ^= (v >> 16);
        //  v ^= (v >> 32);
        //  return v;
        //}

        ///**
        // * Compute the inverted gray code, v XOR (v >> 1) XOR (v >> 2) ...
        // * 
        // * @param v Value
        // * @return Inverted gray code
        // */
        //public static long[] invgrayI(long[] v) {
        //   int last = v.length - 1;
        //  int o;
        //  // Sub word level:
        //  for (o = 1; o < long.SIZE; o <<= 1) {
        //    for (int i = 0; i < last; i++) {
        //      v[i] ^= (v[i] >> o) ^ (v[i + 1] << (Long.SIZE - o));
        //    }
        //    v[last] ^= (v[last] >> o);
        //  }
        //  // Word level:
        //  for (o = 1; o <= last; o <<= 1) {
        //    for (int i = o; i <= last; i++) {
        //      v[i - o] ^= v[i];
        //    }
        //  }
        //  return v;
        //}

        ///**
        // * Test for the bitstring to be all-zero.
        // * 
        // * @param v Bitstring
        // * @return true when all zero
        // */
        //public static bool isZero(long[] v) {
        //  for (int i = 0; i < v.length; i++) {
        //    if (v[i] != 0) {
        //      return false;
        //    }
        //  }
        //  return true;
        //}

        ///**
        // * Compute the cardinality (number of set bits)
        // * 
        // * @param v Value
        // * @return Number of bits set in long
        // */
        //public static int cardinality(long v) {
        //  return long.bitCount(v);
        //}

        /**
         * Compute the cardinality (number of set bits)
         * 
         * Low-endian layout for the array.
         * 
         * @param v Value
         * @return Number of bits set in long[]
         */
        public static long Cardinality(long[] v)
        {
            int sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += BitCount(v[i]);
            }
            return sum;
        }

        /**
         * Invert bit number "off" in v.
         * 
         * @param v Buffer
         * @param off Offset to flip
         */
        public static long FlipC(long v, int off)
        {
            v ^= (1L << off);
            return v;
        }

        /**
         * Invert bit number "off" in v.
         * 
         * Low-endian layout for the array.
         * 
         * @param v Buffer
         * @param off Offset to flip
         */
        public static long[] FlipI(long[] v, int off)
        {
            int wordindex = off >> LONG_LOG2_SIZE;
            v[wordindex] ^= (1L << off);
            return v;
        }

        /**
         * Set bit number "off" in v.
         * 
         * @param v Buffer
         * @param off Offset to set
         */
        public static long SetC(long v, int off)
        {
            v |= (1L << off);
            return v;
        }

        /**
         * Set bit number "off" in v.
         * 
         * Low-endian layout for the array.
         * 
         * @param v Buffer
         * @param off Offset to set
         */
        public static long[] SetI(long[] v, int off)
        {
            int wordindex = off >> LONG_LOG2_SIZE;
            v[wordindex] |= (1L << off);
            return v;
        }

        /**
         * Clear bit number "off" in v.
         * 
         * @param v Buffer
         * @param off Offset to clear
         */
        public static long ClearC(long v, int off)
        {
            v &= ~(1L << off);
            return v;
        }

        /**
         * Clear bit number "off" in v.
         * 
         * Low-endian layout for the array.
         * 
         * @param v Buffer
         * @param off Offset to clear
         */
        public static long[] ClearI(long[] v, int off)
        {
            int wordindex = off >> LONG_LOG2_SIZE;
            v[wordindex] &= ~(1L << off);
            return v;
        }

        /**
         * Set bit number "off" in v.
         * 
         * @param v Buffer
         * @param off Offset to set
         */
        public static bool Get(long v, int off)
        {
            return (v & (1L << off)) != 0;
        }

        /**
         * Set bit number "off" in v.
         * 
         * Low-endian layout for the array.
         * 
         * @param v Buffer
         * @param off Offset to set
         */
        public static bool get(long[] v, int off)
        {
            int wordindex = off >> LONG_LOG2_SIZE;
            return (v[wordindex] & (1L << off)) != 0;
        }

        /**
         * Zero the given set
         * 
         * Low-endian layout for the array.
         * 
         * @param v existing set
         * @return array set to zero
         */
        public static long[] ZeroI(long[] v)
        {
            v.Fill(0);
            return v;
        }

        /**
         * XOR o onto v inplace, i.e. v ^= o
         * 
         * @param v Primary object
         * @param o data to xor
         * @return v
         */
        //public static long[] XorI(long[] v, long[] o) {
        //  Debug.Assert (o.length <= v.length,"Bit set sizes do not agree.");
        //  for (int i = 0; i < o.Length; i++) {
        //    v[i] ^= o[i];
        //  }
        //  return v;
        //}

        ///**
        // * XOR o onto v inplace, i.e. v ^= (o << off)
        // * 
        // * @param v Primary object
        // * @param o data to or
        // * @param off Offset
        // * @return v
        // */
        //public static long[] XorI(long[] v, long[] o, int off) {
        //  if (off == 0) {
        //    return XorI(v, o);
        //  }
        //  if (off < 0) {
        //    throw new InvalidOperationException("Negative shifts are not supported.");
        //  }
        //  // Break shift into integers to shift and bits to shift
        //   int shiftWords = off >> LONG_LOG2_SIZE;
        //   int shiftBits = off & LONG_LOG2_MASK;

        //  if (shiftWords >= v.Length) {
        //    return v;
        //  }
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //     int end = Math.Min(v.Length, o.Length + shiftWords);
        //    for (int i = shiftWords; i < end; i++) {
        //      v[i] ^= o[i - shiftWords];
        //    }
        //    return v;
        //  }
        //  // Overlapping case
        //   int unshiftBits = long.SIZE - shiftBits;
        //   int end = Math.Min(v.Length, o.Length + shiftWords) - 1;
        //  for (int i = end; i > shiftWords; i--) {
        //     int src = i - shiftWords;
        //    v[i] ^= (o[src] << shiftBits) | (o[src - 1] >> unshiftBits);
        //  }
        //  v[shiftWords] ^= o[0] << shiftBits;
        //  return v;
        //}

        ///**
        // * OR o onto v inplace, i.e. v |= o
        // * 
        // * @param v Primary object
        // * @param o data to or
        // * @return v
        // */
        //public static long[] orI(long[] v, long[] o) {
        //  Debug.Assert (o.Length <= v.Length, "Bit set sizes do not agree.");
        //   int max = Math.Min(v.Length, o.Length);
        //  for (int i = 0; i < max; i++) {
        //    v[i] |= o[i];
        //  }
        //  return v;
        //}

        ///**
        // * OR o onto v inplace, i.e. v |= (o << off)
        // * 
        // * Note: Bits that are shifted outside of the size of v are discarded.
        // * 
        // * @param v Primary object
        // * @param o data to or
        // * @param off Offset
        // * @return v
        // */
        //public static long[] OrI(long[] v, long[] o, int off) {
        //  if (off == 0) {
        //    return orI(v, o);
        //  }
        //  if (off < 0) {
        //    throw new InvalidOperationException("Negative shifts are not supported.");
        //  }
        //  // Break shift into integers to shift and bits to shift
        //   int shiftWords = off >> LONG_LOG2_SIZE;
        //   int shiftBits = off & LONG_LOG2_MASK;

        //  if (shiftWords >= v.length) {
        //    return v;
        //  }
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //     int end = Math.Min(v.length, o.length + shiftWords);
        //    for (int i = shiftWords; i < end; i++) {
        //      v[i] |= o[i - shiftWords];
        //    }
        //    return v;
        //  }
        //  // Overlapping case
        //   int unshiftBits = Long.SIZE - shiftBits;
        //   int end = Math.Min(v.Length, o.Length + shiftWords) - 1;
        //  for (int i = end; i > shiftWords; i--) {
        //     int src = i - shiftWords;
        //    v[i] |= (o[src] << shiftBits) | (o[src - 1] >> unshiftBits);
        //  }
        //  v[shiftWords] |= o[0] << shiftBits;
        //  return v;
        //}

        ///**
        // * AND o onto v inplace, i.e. v &= o
        // * 
        // * @param v Primary object
        // * @param o data to and
        // * @return v
        // */
        //public static long[] andI(long[] v, long[] o) {
        //  int i = 0;
        //  for (; i < o.length; i++) {
        //    v[i] |= o[i];
        //  }
        //  // Zero higher words
        //  Arrays.fill(v, i, v.length, 0);
        //  return v;
        //}

        ///**
        // * AND o onto v inplace, i.e. v &= (o << off)
        // * 
        // * @param v Primary object
        // * @param o data to or
        // * @param off Offset
        // * @return v
        // */
        //public static long[] andI(long[] v, long[] o, int off) {
        //  if (off == 0) {
        //    return andI(v, o);
        //  }
        //  if (off < 0) {
        //    throw new UnsupportedOperationException("Negative shifts are not supported.");
        //  }
        //  // Break shift into integers to shift and bits to shift
        //   int shiftWords = off >> LONG_LOG2_SIZE;
        //   int shiftBits = off & LONG_LOG2_MASK;

        //  if (shiftWords >= v.length) {
        //    return v;
        //  }
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //     int end = Math.min(v.length, o.length + shiftWords);
        //    for (int i = shiftWords; i < end; i++) {
        //      v[i] &= o[i - shiftWords];
        //    }
        //    // Clear bottom words
        //    Arrays.fill(v, 0, shiftWords, 0);
        //    return v;
        //  }
        //  // Overlapping case
        //   int unshiftBits = Long.SIZE - shiftBits;
        //   int end = Math.min(v.length, o.length + shiftWords) - 1;
        //  Arrays.fill(v, end + 1, v.length, 0);
        //  for (int i = end; i > shiftWords; i--) {
        //     int src = i - shiftWords;
        //    v[i] &= (o[src] << shiftBits) | (o[src - 1] >> unshiftBits);
        //  }
        //  v[shiftWords] &= o[0] << shiftBits;
        //  // Clear bottom words
        //  Arrays.fill(v, 0, shiftWords, 0);
        //  return v;
        //}

        ///**
        // * Invert v inplace.
        // * 
        // * @param v Object to invert
        // * @return v
        // */
        //public static long[] invertI(long[] v) {
        //  for (int i = 0; i < v.length; i++) {
        //    v[i] = ~v[i];
        //  }
        //  return v;
        //}

        ///**
        // * Shift a long[] bitset inplace.
        // * 
        // * Low-endian layout for the array.
        // * 
        // * @param v existing bitset
        // * @param off Offset to shift by
        // * @return Bitset
        // */
        //public static long[] shiftRightI(long[] v, int off) {
        //  if (off == 0) {
        //    return v;
        //  }
        //  if (off < 0) {
        //    return shiftLeftI(v, -off);
        //  }
        //  // Break shift into integers to shift and bits to shift
        //   int shiftWords = off >> LONG_LOG2_SIZE;
        //   int shiftBits = off & LONG_LOG2_MASK;

        //  if (shiftWords >= v.length) {
        //    return zeroI(v);
        //  }
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //    // Move whole words down
        //    System.arraycopy(v, shiftWords, v, 0, v.length - shiftWords);
        //    // Fill top words with zeros
        //    Arrays.fill(v, v.length - shiftWords, v.length, 0);
        //    return v;
        //  }
        //  // Overlapping case
        //   int unshiftBits = Long.SIZE - shiftBits;
        //  // Bottom-up to not overlap the operations.
        //  for (int i = 0; i < v.length - shiftWords - 1; i++) {
        //     int src = i + shiftWords;
        //    v[i] = (v[src + 1] << unshiftBits) | (v[src] >> shiftBits);
        //  }
        //  // The last original word
        //  v[v.length - shiftWords - 1] = v[v.length - 1] >> shiftBits;
        //  // Fill whole words "lost" by the shift
        //  Arrays.fill(v, v.length - shiftWords, v.length, 0);
        //  return v;
        //}

        ///**
        // * Shift a long[] bitset inplace.
        // * 
        // * Low-endian layout for the array.
        // * 
        // * @param v existing bitset
        // * @param off Offset to shift by
        // * @return Bitset
        // */
        //public static long[] shiftLeftI(long[] v, int off) {
        //  if (off == 0) {
        //    return v;
        //  }
        //  if (off < 0) {
        //    return shiftRightI(v, -off);
        //  }
        //  // Break shift into integers to shift and bits to shift
        //   int shiftWords = off >> LONG_LOG2_SIZE;
        //   int shiftBits = off & LONG_LOG2_MASK;

        //  if (shiftWords >= v.length) {
        //    return zeroI(v);
        //  }
        //  // Simple case - multiple of word size
        //  if (shiftBits == 0) {
        //    // Move whole words up
        //    System.arraycopy(v, 0, v, shiftWords, v.length - shiftWords);
        //    // Fill the initial words with zeros
        //    Arrays.fill(v, 0, shiftWords, 0);
        //    return v;
        //  }
        //  // Overlapping case
        //   int unshiftBits = Long.SIZE - shiftBits;
        //  // Top-Down to not overlap the operations.
        //  for (int i = v.length - 1; i > shiftWords; i--) {
        //     int src = i - shiftWords;
        //    v[i] = (v[src] << shiftBits) | (v[src - 1] >> unshiftBits);
        //  }
        //  v[shiftWords] = v[0] << shiftBits;
        //  // Fill the initial words with zeros
        //  Arrays.fill(v, 0, shiftWords, 0);
        //  return v;
        //}

        ///**
        // * Rotate a long to the right, cyclic with length len
        // * 
        // * @param v Bits
        // * @param shift Shift value
        // * @param len Length
        // * @return cycled bit set
        // */
        //public static long cycleRightC(long v, int shift, int len) {
        //  if (shift == 0) {
        //    return v;
        //  }
        //  if (shift < 0) {
        //    return cycleLeftC(v, -shift, len);
        //  }
        //   long ones = (1 << len) - 1;
        //  return (((v) >> (shift)) | ((v) << ((len) - (shift)))) & ones;
        //}

        ///**
        // * Cycle a bitstring to the right.
        // * 
        // * @param v Bit string
        // * @param shift Number of steps to cycle
        // * @param len Length
        // */
        //public static long[] cycleRightI(long[] v, int shift, int len) {
        //  long[] t = copy(v, len, len - shift);
        //  truncateI(t, len);
        //  shiftRightI(v, shift);
        //  orI(v, t);
        //  return v;
        //}

        ///**
        // * Truncate a bit string to the given length (setting any higher bit to 0).
        // * 
        // * @param v String to process
        // * @param len Length (in bits) to truncate to
        // */
        //public static long[] truncateI(long[] v, int len) {
        //   int zap = (v.length * Long.SIZE) - len;
        //   int zapWords = (zap >> LONG_LOG2_SIZE);
        //   int zapbits = zap & LONG_LOG2_MASK;
        //  Arrays.fill(v, v.length - zapWords, v.length, 0);
        //  if (zapbits > 0) {
        //    v[v.length - zapWords - 1] &= (LONG_ALL_BITS >> zapbits);
        //  }
        //  return v;
        //}

        ///**
        // * Rotate a long to the left, cyclic with length len
        // * 
        // * @param v Bits
        // * @param shift Shift value
        // * @param len Length
        // * @return cycled bit set
        // */
        //public static long cycleLeftC(long v, int shift, int len) {
        //  if (shift == 0) {
        //    return v;
        //  }
        //  if (shift < 0) {
        //    return cycleRightC(v, -shift, len);
        //  }
        //   long ones = (1 << len) - 1;
        //  return (((v) << (shift)) | ((v) >> ((len) - (shift)))) & ones;
        //}

        ///**
        // * Cycle a bitstring to the right.
        // * 
        // * @param v Bit string
        // * @param shift Number of steps to cycle
        // * @param len Length
        // */
        //public static long[] cycleLeftI(long[] v, int shift, int len) {
        //  long[] t = copy(v, len, shift);
        //  truncateI(t, len);
        //  shiftRightI(v, len - shift);
        //  orI(v, t);
        //  return v;
        //}

        ///**
        // * Convert bitset to a string consisting of "0" and "1", in high-endian order.
        // * 
        // * @param v Value to process
        // * @return String representation
        // */
        //public static String toString(long[] v) {
        //   int mag = magnitude(v);
        //  if (v.length == 0 || mag == 0) {
        //    return "0";
        //  }
        //   int words = ((mag - 1) >> LONG_LOG2_SIZE) + 1;
        //  char[] digits = new char[mag];

        //  int pos = mag - 1;
        //  for (int w = 0; w < words; w++) {
        //    long f = 1L;
        //    for (int i = 0; i < Long.SIZE; i++) {
        //      digits[pos] = ((v[w] & f) == 0) ? '0' : '1';
        //      pos--;
        //      f <<= 1;
        //      if (pos < 0) {
        //        break;
        //      }
        //    }
        //  }
        //  return new String(digits);
        //}

        /**
         * Convert bitset to a string consisting of "0" and "1", in high-endian order.
         * 
         * @param v Value to process
         * @param minw Minimum width
         * @return String representation
         */
        public static String ToString(long[] v, int minw)
        {
            int mag = Math.Max(Magnitude(v), minw);
            if (v.Length == 0 || mag == 0)
            {
                return "0";
            }
            int words = ((mag - 1) >> LONG_LOG2_SIZE) + 1;
            char[] digits = new char[mag];

            int pos = mag - 1;
            for (int w = 0; w < words; w++)
            {
                long f = 1L;
                for (int i = 0; i < sizeof(long) * 8; i++)
                {
                    digits[pos] = ((v[w] & f) == 0) ? '0' : '1';
                    pos--;
                    f <<= 1;
                    if (pos < 0)
                    {
                        break;
                    }
                }
            }
            return new String(digits);
        }

        ///**
        // * Convert bitset to a string consisting of "0" and "1", in high-endian order.
        // * 
        // * @param v Value to process
        // * @return String representation
        // */
        //public static String toString(long v) {
        //   int mag = magnitude(v);
        //  if (mag == 0) {
        //    return "0";
        //  }
        //  char[] digits = new char[mag];

        //  int pos = mag - 1;
        //  long f = 1L;
        //  for (int i = 0; i < Long.SIZE; i++) {
        //    digits[pos] = ((v & f) == 0) ? '0' : '1';
        //    pos--;
        //    f <<= 1;
        //    if (pos < 0) {
        //      break;
        //    }
        //  }
        //  return new String(digits);
        //}

        ///**
        // * Find the number of trailing zeros.
        // * 
        // * @param v Bitset
        // * @return Position of first set bit, -1 if no set bit was found.
        // */
        //public static int numberOfTrailingZerosSigned(long[] v) {
        //  for (int p = 0;; p++) {
        //    if (p == v.length) {
        //      return -1;
        //    }
        //    if (v[p] != 0) {
        //      return Long.numberOfTrailingZeros(v[p]) + p * Long.SIZE;
        //    }
        //  }
        //}

        ///**
        // * Find the number of trailing zeros.
        // * 
        // * @param v Bitset
        // * @return Position of first set bit, v.length * 64 if no set bit was found.
        // */
        //public static int numberOfTrailingZeros(long[] v) {
        //  for (int p = 0;; p++) {
        //    if (p == v.length) {
        //      return p * Long.SIZE;
        //    }
        //    if (v[p] != 0) {
        //      return Long.numberOfTrailingZeros(v[p]) + p * Long.SIZE;
        //    }
        //  }
        //}

        /**
         * Find the number of trailing zeros.
         * 
         * Note: this has different semantics to {@link Long#numberOfLeadingZeros}
         * when the number is 0.
         * 
         * @param v Long
         * @return Position of first set bit, -1 if no set bit was found.
         */
        //public static int numberOfTrailingZerosSigned(long v)
        //{
        //    return Long.numberOfTrailingZeros(v);
        //}

        /**
         * Find the number of trailing zeros.
         * 
         * Note: this is the same as {@link Long#numberOfTrailingZeros}
         * 
         * @param v Long
         * @return Position of first set bit, 64 if no set bit was found.
         */
        public static int NumberOfTrailingZeros(long v)
        {
            int mask = 1;
            for (int i = 0; i < sizeof(long) * 8; i++)
                if ((v & mask << i) != 0)
                    return i;
            return sizeof(long) * 8;

        }

        ///**
        // * Find the number of trailing zeros.
        // * 
        // * Note: this is the same as {@link Long#numberOfTrailingZeros}
        // * 
        // * @param v Long
        // * @return Position of first set bit, 64 if no set bit was found.
        // */
        //public static int numberOfTrailingZeros(int v) {
        //  return Integer.numberOfTrailingZeros(v);
        //}

        ///**
        // * Find the number of leading zeros.
        // * 
        // * @param v Bitset
        // * @return Position of first set bit, -1 if no set bit was found.
        // */
        //public static int numberOfLeadingZerosSigned(long[] v) {
        //  for (int p = 0, ip = v.length - 1;; p++, ip--) {
        //    if (p == v.length) {
        //      return -1;
        //    }
        //    if (v[ip] != 0) {
        //      return Long.numberOfLeadingZeros(v[ip]) + p * Long.SIZE;
        //    }
        //  }
        //}

        /**
         * Find the number of leading zeros.
         * 
         * @param v Bitset
         * @return Position of first set bit, v.length * 64 if no set bit was found.
         */
        public static int NumberOfLeadingZeros(long[] v)
        {
            for (int p = 0, ip = v.Length - 1; ; p++, ip--)
            {
                if (p == v.Length)
                {
                    return p * sizeof(long) * 8;
                }
                if (v[ip] != 0)
                {
                    return NumberOfLeadingZeros(v[ip]) + p * sizeof(long) * 8;
                }
            }
        }

        ///**
        // * Find the number of leading zeros; -1 if all zero
        // * 
        // * Note: this has different semantics to {@link Long#numberOfLeadingZeros}
        // * when the number is 0.
        // * 
        // * @param v Bitset
        // * @return Position of first set bit, -1 if no set bit was found.
        // */
        //public static int numberOfLeadingZerosSigned(long v) {
        //  if (v == 0) {
        //    return -1;
        //  }
        //  return Long.numberOfLeadingZeros(v);
        //}

        ///**
        // * Find the number of leading zeros; -1 if all zero
        // * 
        // * Note: this has different semantics to {@link Long#numberOfLeadingZeros}
        // * when the number is 0.
        // * 
        // * @param v Bitset
        // * @return Position of first set bit, -1 if no set bit was found.
        // */
        //public static int numberOfLeadingZerosSigned(int v) {
        //  if (v == 0) {
        //    return -1;
        //  }
        //  return Integer.numberOfLeadingZeros(v);
        //}

        /**
         * Find the number of leading zeros; 64 if all zero
         * 
         * Note: this the same as {@link Long#numberOfLeadingZeros}.
         * 
         * @param v Bitset
         * @return Position of first set bit, 64 if no set bit was found.
         */
        public static int NumberOfLeadingZeros(long v)
        {
            int mask = 1;
            for (int i = sizeof(long) * 8 - 1; i >= 0; i--)
                if ((v & mask << i) != 0)
                    return sizeof(long) * 8 - i - 1;
            return sizeof(long) * 8;
        }

        /**
         * Find the number of leading zeros; 32 if all zero
         * 
         * Note: this the same as {@link Integer#numberOfLeadingZeros}.
         * 
         * @param v Bitset
         * @return Position of first set bit, 32 if no set bit was found.
         */
        public static int NumberOfLeadingZeros(int v)
        {

            int mask = 1;
            for (int i = sizeof(int) * 8 - 1; i >= 0; i--)
                if ((v & mask << i) != 0)
                    return sizeof(int) * 8 - i - 1;
            return sizeof(int) * 8;
        }

        ///**
        // * Find the previous set bit.
        // * 
        // * @param v Values to process
        // * @param start Start position (inclusive)
        // * @return Position of previous set bit, or -1.
        // */
        //public static int previousSetBit(long[] v, int start) {
        //  if (start == -1) {
        //    return -1;
        //  }
        //  int wordindex = start >> LONG_LOG2_SIZE;
        //  if (wordindex >= v.length) {
        //    return magnitude(v) - 1;
        //  }
        //  // Initial word
        //   int off = Long.SIZE - 1 - (start & LONG_LOG2_MASK);
        //  long cur = v[wordindex] & (LONG_ALL_BITS >> off);
        //  for (;;) {
        //    if (cur != 0) {
        //      return (wordindex + 1) * Long.SIZE - 1 - Long.numberOfLeadingZeros(cur);
        //    }
        //    if (wordindex == 0) {
        //      return -1;
        //    }
        //    wordindex--;
        //    cur = v[wordindex];
        //  }
        //}

        ///**
        // * Find the previous clear bit.
        // * 
        // * @param v Values to process
        // * @param start Start position (inclusive)
        // * @return Position of previous clear bit, or -1.
        // */
        //public static int previousClearBit(long[] v, int start) {
        //  if (start == -1) {
        //    return -1;
        //  }
        //  int wordindex = start >> LONG_LOG2_SIZE;
        //  if (wordindex >= v.length) {
        //    return magnitude(v);
        //  }
        //   int off = Long.SIZE + 1 - (start & LONG_LOG2_MASK);
        //  // Initial word
        //  long cur = ~v[wordindex] & (LONG_ALL_BITS >> off);
        //  for (;;) {
        //    if (cur != 0) {
        //      return (wordindex + 1) * Long.SIZE - 1 - Long.numberOfTrailingZeros(cur);
        //    }
        //    if (wordindex == 0) {
        //      return -1;
        //    }
        //    wordindex--;
        //    cur = ~v[wordindex];
        //  }
        //}

        /**
         * Find the next set bit.
         * 
         * @param v Value to process
         * @param start Start position (inclusive)
         * @return Position of next set bit, or -1.
         */
        public static int NextSetBit(long[] v, int start)
        {
            int wordindex = start >> LONG_LOG2_SIZE;
            if (wordindex >= v.Length)
            {
                return -1;
            }

            // Initial word
            long cur = v[wordindex] & (LONG_ALL_BITS << start);
            for (; ; )
            {
                if (cur != 0)
                {
                    return (wordindex * sizeof(long) * 8) + NumberOfTrailingZeros(cur);
                }
                wordindex++;
                if (wordindex == v.Length)
                {
                    return -1;
                }
                cur = v[wordindex];
            }
        }

        /**
         * Find the next clear bit.
         * 
         * @param v Value to process
         * @param start Start position (inclusive)
         * @return Position of next clear bit, or -1.
         */
        public static int NextClearBit(long[] v, int start)
        {
            int wordindex = start >> LONG_LOG2_SIZE;
            if (wordindex >= v.Length)
            {
                return -1;
            }

            // Initial word
            long cur = ~v[wordindex] & (LONG_ALL_BITS << start);
            for (; wordindex < v.Length; )
            {
                if (cur != 0)
                {
                    return (wordindex * sizeof(long) * 8) + NumberOfTrailingZeros(cur);
                }
                wordindex++;
                cur = ~v[wordindex];
            }
            return -1;
        }

        /**
         * The magnitude is the position of the highest bit set
         * 
         * @param v Vector v
         * @return position of highest bit set, or 0.
         */
        public static int Magnitude(long[] v)
        {
            int l = NumberOfLeadingZeros(v);
            return Capacity(v) - l;
        }

        ///**
        // * The magnitude is the position of the highest bit set
        // * 
        // * @param v Vector v
        // * @return position of highest bit set, or 0.
        // */
        //public static int magnitude(long v) {
        //  return Long.SIZE - Long.numberOfLeadingZeros(v);
        //}

        ///**
        // * The magnitude is the position of the highest bit set
        // * 
        // * @param v Vector v
        // * @return position of highest bit set, or 0.
        // */
        //public static int magnitude(int v) {
        //  return Integer.SIZE - Integer.numberOfLeadingZeros(v);
        //}

        /**
         * Capacity of the vector v.
         * 
         * @param v Vector v
         * @return Capacity
         */
        public static int Capacity(long[] v)
        {
            return v.Length * sizeof(long) * 8;
        }

        ///**
        // * Compare two bitsets.
        // * 
        // * @param x First bitset
        // * @param y Second bitset
        // * @return Comparison result
        // */
        //public static int compare(long[] x, long[] y) {
        //  int p = Math.min(x.length, y.length) - 1;
        //  for (int i = x.length - 1; i > p; i--) {
        //    if (x[i] != 0) {
        //      return +1;
        //    }
        //  }
        //  for (int i = y.length - 1; i > p; i--) {
        //    if (y[i] != 0) {
        //      return -1;
        //    }
        //  }
        //  for (; p >= 0; p--) {
        //     long xp = x[p];
        //     long yp = y[p];
        //    if (xp != yp) {
        //      if (xp < 0) {
        //        if (yp < 0) {
        //          return (yp < xp) ? -1 : ((yp == xp) ? 0 : 1);
        //        } else {
        //          return +1;
        //        }
        //      } else {
        //        if (yp < 0) {
        //          return -1;
        //        } else {
        //          return (xp < yp) ? -1 : ((xp == yp) ? 0 : 1);
        //        }
        //      }
        //    }
        //  }
        //  return 0;
        //}

        //public static double lpow2(long m, int n) {
        //  if (m == 0) {
        //    return 0.0;
        //  }
        //  if (m == Long.MIN_VALUE) {
        //    return lpow2(Long.MIN_VALUE >> 1, n + 1);
        //  }
        //  if (m < 0) {
        //    return -lpow2(-m, n);
        //  }
        //  assert(m >= 0);
        //  int bitLength = magnitude(m);
        //  int shift = bitLength - 53;
        //  long exp = 1023L + 52 + n + shift; // Use long to avoid overflow.
        //  if (exp >= 0x7FF) {
        //    return Double.POSITIVE_INFINITY;
        //  }
        //  if (exp <= 0) { // Degenerated number (subnormal, assume 0 for bit 52)
        //    if (exp <= -54) {
        //      return 0.0;
        //    }
        //    return lpow2(m, n + 54) / 18014398509481984L; // 2^54 Exact.
        //  }
        //  // Normal number.
        //  long bits = (shift > 0) ? (m >> shift) + ((m >> (shift - 1)) & 1) : // Rounding.
        //  m << -shift;
        //  if (((bits >> 52) != 1) && (++exp >= 0x7FF)) {
        //    return Double.POSITIVE_INFINITY;
        //  }
        //  bits &= 0x000fffffffffffffL; // Clears MSB (bit 52)
        //  bits |= exp << 52;
        //  return Double.longBitsToDouble(bits);
        //}

        ///**
        // * Compute {@code m * Math.pow(10,e)} on the bit representation, for
        // * assembling a floating point decimal value.
        // * 
        // * @param m Mantisse
        // * @param n Exponent to base 10.
        // * @return Double value.
        // */
        //public static double lpow10(long m, int n) {
        //  if (m == 0) {
        //    return 0.0;
        //  }
        //  if (m == Long.MIN_VALUE) {
        //    return lpow10(Long.MIN_VALUE / 10, n + 1);
        //  }
        //  if (m < 0) {
        //    return -lpow10(-m, n);
        //  }
        //  if (n >= 0) { // Positive power.
        //    if (n > 308) {
        //      return Double.POSITIVE_INFINITY;
        //    }
        //    // Works with 4 x 32 bits registers (x3:x2:x1:x0)
        //    long x0 = 0; // 32 bits.
        //    long x1 = 0; // 32 bits.
        //    long x2 = m & LONG_32_BITS; // 32 bits.
        //    long x3 = m >> 32; // 32 bits.
        //    int pow2 = 0;
        //    while (n != 0) {
        //      int i = (n >= POW5_INT.length) ? POW5_INT.length - 1 : n;
        //      int coef = POW5_INT[i]; // 31 bits max.

        //      if (((int) x0) != 0) {
        //        x0 *= coef; // 63 bits max.
        //      }
        //      if (((int) x1) != 0) {
        //        x1 *= coef; // 63 bits max.
        //      }
        //      x2 *= coef; // 63 bits max.
        //      x3 *= coef; // 63 bits max.

        //      x1 += x0 >> 32;
        //      x0 &= LONG_32_BITS;

        //      x2 += x1 >> 32;
        //      x1 &= LONG_32_BITS;

        //      x3 += x2 >> 32;
        //      x2 &= LONG_32_BITS;

        //      // Adjusts powers.
        //      pow2 += i;
        //      n -= i;

        //      // Normalizes (x3 should be 32 bits max).
        //      long carry = x3 >> 32;
        //      if (carry != 0) { // Shift.
        //        x0 = x1;
        //        x1 = x2;
        //        x2 = x3 & LONG_32_BITS;
        //        x3 = carry;
        //        pow2 += 32;
        //      }
        //    }

        //    // Merges registers to a 63 bits mantissa.
        //    assert(x3 >= 0);
        //    int shift = 31 - magnitude(x3); // -1..30
        //    pow2 -= shift;
        //    long mantissa = (shift < 0) ? (x3 << 31) | (x2 >> 1) : // x3 is 32 bits.
        //    (((x3 << 32) | x2) << shift) | (x1 >> (32 - shift));
        //    return lpow2(mantissa, pow2);
        //  } else { // n < 0
        //    if (n < -324 - 20) {
        //      return 0.0;
        //    }

        //    // Works with x1:x0 126 bits register.
        //    long x1 = m; // 63 bits.
        //    long x0 = 0; // 63 bits.
        //    int pow2 = 0;
        //    while (true) {
        //      // Normalizes x1:x0
        //      assert(x1 >= 0);
        //      int shift = 63 - magnitude(x1);
        //      x1 <<= shift;
        //      x1 |= x0 >> (63 - shift);
        //      x0 = (x0 << shift) & LONG_63_BITS;
        //      pow2 -= shift;

        //      // Checks if division has to be performed.
        //      if (n == 0) {
        //        break; // Done.
        //      }

        //      // Retrieves power of 5 divisor.
        //      int i = (-n >= POW5_INT.length) ? POW5_INT.length - 1 : -n;
        //      int divisor = POW5_INT[i];

        //      // Performs the division (126 bits by 31 bits).
        //      long wh = (x1 >> 32);
        //      long qh = wh / divisor;
        //      long r = wh - qh * divisor;
        //      long wl = (r << 32) | (x1 & LONG_32_BITS);
        //      long ql = wl / divisor;
        //      r = wl - ql * divisor;
        //      x1 = (qh << 32) | ql;

        //      wh = (r << 31) | (x0 >> 32);
        //      qh = wh / divisor;
        //      r = wh - qh * divisor;
        //      wl = (r << 32) | (x0 & LONG_32_BITS);
        //      ql = wl / divisor;
        //      x0 = (qh << 32) | ql;

        //      // Adjusts powers.
        //      n += i;
        //      pow2 -= i;
        //    }
        //    return lpow2(x1, pow2);
        //  }
        //}
        public static int BitCount(long n)
        {
            int ret = 0;
            while (n != 0)
            {
                n &= (n - 1);
                ret++;
            }
            return ret;
        }
    }

}
