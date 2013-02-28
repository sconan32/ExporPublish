using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Maths.Histograms
{

    public class ReplacingHistogram<T> : IEnumerable<DoubleObjPair<T>>
    {
      
        /**
         * Array shift to account for negative indices.
         */
        protected int offset = 0;

        /**
         * Size of array storage.
         */
        protected int size;

        /**
         * Array 'base', i.e. the point of 0.0. Usually the minimum.
         */
        protected double _base;

        /**
         * To avoid introducing an extra bucket for the maximum value.
         */
        protected double max;

        /**
         * Width of a bin.
         */
        protected double binsize;

        /**
         * Data storage
         */
        protected List<T> data;

        /**
         * Constructor for new elements
         */
        private ReplAdapter<T> maker;

        /**
         * Histogram constructor
         * 
         * @param bins Number of bins to use.
         * @param min Minimum Value
         * @param max Maximum Value
         * @param maker Constructor for new elements.
         */
        public ReplacingHistogram(int bins, double min, double max, ReplAdapter<T> maker)
        {
            this._base = min;
            this.max = max;
            this.binsize = (max - min) / bins;
            this.size = bins;
            this.data = new List<T>(bins);
            this.maker = maker;
            for (int i = 0; i < bins; i++)
            {
                this.data.Add(maker.Make());
            }
        }

        /**
         * Histogram constructor without 'Constructor' to generate new elements. Empty
         * bins will be initialized with 'null'.
         * 
         * @param bins Number of bins
         * @param min Minimum value
         * @param max Maximum value.
         */
        public ReplacingHistogram(int bins, double min, double max) :
            this(bins, min, max, null)
        {
        }

        /**
         * Get the data at a given Coordinate.
         * 
         * @param coord Coordinate.
         * @return data element there (which may be a new empty bin or null)
         */
        public virtual T Get(double coord)
        {
            int bin = GetBinNr(coord);
            // compare with allocated area
            if (bin < 0)
            {
                T n = maker.Make();
                return n;
            }
            if (bin >= size)
            {
                T n = maker.Make();
                return n;
            }
            return data[(bin)];
        }

        /**
         * Put data at a given coordinate. Note: this replaces the contents, it
         * doesn't "Add" or "count".
         * 
         * @param coord Coordinate
         * @param d New Data
         */
        public virtual void Replace(double coord, T d)
        {
            int bin = GetBinNr(coord);
            PutBin(bin, d);
        }

        /**
         * Compute the bin number. Has a special case for rounding max down to the
         * last bin.
         * 
         * @param coord Coordinate
         * @return bin number
         */
        protected virtual int GetBinNr(double coord)
        {
            if (Double.IsInfinity(coord) || Double.IsNaN(coord))
            {
                throw new InvalidOperationException("Encountered non-finite value in Histogram: " + coord);
            }
            if (coord == max)
            {
                // System.err.println("Triggered special case: "+ (Math.floor((coord -
                // base) / binsize) + offset) + " vs. " + (size - 1));
                return size - 1;
            }
            return (int)Math.Floor((coord - _base) / binsize) + offset;
        }

        /**
         * Internal put function to handle the special cases of histogram resizing.
         * 
         * @param bin bin number
         * @param d data to put
         */
        private void PutBin(int bin, T d)
        {
            if (bin < 0)
            {
                // make sure to have enough space
                //data.EnsureCapacity(size - bin);
                // insert new data in front.
                data.Insert(0, d);
                // fill the gap. Note that bin < 0.
                for (int i = bin + 1; i < 0; i++)
                {
                    data.Insert(1, maker.Make());
                }
                // We still have bin < 0, thus (size - bin) > size!
                Debug.Assert(data.Count == size - bin);
                offset = offset - bin;
                size = size - bin;
                // drop max value when resizing
                max = Double.MaxValue;
            }
            else if (bin >= size)
            {
                //this.data.EnsureCapacity(bin + 1);
                while (data.Count < bin)
                {
                    data.Add(maker.Make());
                }
                // Add the new data.
                data.Add(d);
                Debug.Assert(data.Count == bin + 1);
                size = bin + 1;
                // drop max value when resizing
                max = Double.MaxValue;
            }
            else
            {
                this.data[bin] = d;
            }
        }

        /**
         * Get the number of bins actually in use.
         * 
         * @return number of bins
         */
        public virtual int GetNumBins()
        {
            return size;
        }

        /**
         * Get the size (width) of a bin.
         * 
         * @return bin size
         */
        public virtual double GetBinsize()
        {
            return binsize;
        }

        /**
         * Mean of bin
         * 
         * @param bin Bin number
         * @return Mean
         */
        public virtual double GetBinMean(int bin)
        {
            return _base + (bin + 0.5 - offset) * binsize;
        }

        /**
         * Minimum of bin
         * 
         * @param bin Bin number
         * @return Lower bound
         */
        public virtual double GetBinMin(int bin)
        {
            return _base + (bin - offset) * binsize;
        }

        /**
         * Maximum of bin
         * 
         * @param bin Bin number
         * @return Upper bound
         */
        public virtual double GetBinMax(int bin)
        {
            return _base + (bin + 1 - offset) * binsize;
        }

        /**
         * Get minimum (covered by bins, not data!)
         * 
         * @return minimum
         */
        public virtual double GetCoverMinimum()
        {
            return _base - offset * binsize;
        }

        /**
         * Get maximum (covered by bins, not data!)
         * 
         * @return maximum
         */
        public virtual double GetCoverMaximum()
        {
            return _base + (size - offset) * binsize;
        }

        /**
         * Get the raw data. Note that this does NOT include the coordinates.
         * 
         * @return raw data array.
         */
        public virtual List<T> GetData()
        {
            return data;
        }

        /**
         * Make a new bin.
         * 
         * @return new bin.
         */
        protected virtual T Make()
        {
            return maker.Make();
        }

        /**
         * Iterator class to iterate over all bins.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
        // */
        //protected class Iter implements Iterator<DoubleObjPair<T>> {
        //  /**
        //   * Current bin number
        //   */
        //  int bin = 0;

        //  @Override
        //  public boolean hasNext() {
        //    return bin < size;
        //  }

        //  @Override
        //  public DoubleObjPair<T> next() {
        //    DoubleObjPair<T> pair = new DoubleObjPair<T>(base + (bin + 0.5 - offset) * binsize, data.Get(bin));
        //    bin++;
        //    return pair;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new UnsupportedOperationException("Histogram iterators cannot be modified.");
        //  }
        //}

        /**
         * Iterator class to iterate over all bins.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
        // */
        //protected class RIter implements Iterator<DoubleObjPair<T>> {
        //  /**
        //   * Current bin number
        //   */
        //  int bin = size - 1;

        //  @Override
        //  public boolean hasNext() {
        //    return bin >= 0;
        //  }

        //  @Override
        //  public DoubleObjPair<T> next() {
        //    DoubleObjPair< T> pair = new DoubleObjPair<T>(base + (bin + 0.5 - offset) * binsize, data.Get(bin));
        //    bin--;
        //    return pair;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new UnsupportedOperationException("Histogram iterators cannot be modified.");
        //  }
        //}

        ///**
        // * Get an iterator over all histogram bins.
        // */
        //@Override
        //public Iterator<DoubleObjPair<T>> iterator() {
        //  return new Iter();
        //}

        ///**
        // * Get an iterator over all histogram bins.
        // */
        //// TODO: is there some interface to implement.
        //public Iterator<DoubleObjPair<T>> reverseIterator() {
        //  return new RIter();
        //}

        /**
         * Convenience constructor for Integer-based Histograms. Uses a constructor to
         * initialize bins with Integer(0)
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Integers.
         */


        public static ReplacingHistogram<Int32> IntHistogram(int bins, double min, double max)
        {
            return new ReplacingHistogram<Int32>(bins, min, max, new ReplIntAdapter());
        }

        /**
         * Convenience constructor for Double-based Histograms. Uses a constructor to
         * initialize bins with Double(0)
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Doubles.
         */
        public static ReplacingHistogram<Double> DoubleHistogram(int bins, double min, double max)
        {
            return new ReplacingHistogram<Double>(bins, min, max, new ReplDoubleAdapter());
        }

        /**
         * Convenience constructor for Histograms with pairs of Integers Uses a
         * constructor to initialize bins with Pair(Integer(0),Integer(0))
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Integer pairs.
         */
        public static ReplacingHistogram<IntIntPair> IntIntHistogram(int bins, double min, double max)
        {
            return new ReplacingHistogram<IntIntPair>(bins, min, max, new ReplIntIntPairAdapter());
        }

        /**
         * Convenience constructor for Histograms with pairs of Doubles Uses a
         * constructor to initialize bins with Pair(Double(0),Double(0))
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Double pairs.
         */
        public static ReplacingHistogram<DoubleDoublePair> DoubleDoubleHistogram(int bins, double min, double max)
        {
            return new ReplacingHistogram<DoubleDoublePair>(bins, min, max, new ReplDoubleDoublePairAdapter());
        }

        public IEnumerator<DoubleObjPair<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    /**
       * Interface to plug in a data type T.
       * 
       * @author Erich Schubert
       * 
       * @param <T> Data type
       */
    public abstract class ReplAdapter<T1>
    {
        /**
         * Construct a new T when needed.
         * 
         * @return new T
         */
        public abstract T1 Make();
    }
    public class ReplIntAdapter : ReplAdapter<Int32>
    {
        public override Int32 Make()
        {
            return new Int32();
        }
    }
    public class ReplDoubleAdapter : ReplAdapter<double>
    {

        public override Double Make()
        {
            return new Double();
        }
    }
    public class ReplIntIntPairAdapter : ReplAdapter<IntIntPair>
    {
        public override IntIntPair Make()
        {
            return new IntIntPair(0, 0);
        }
    }
    public class ReplDoubleDoublePairAdapter : ReplAdapter<DoubleDoublePair>
    {
        public override DoubleDoublePair Make()
        {
            return new DoubleDoublePair(0.0, 0.0);
        }
    }
    
}
