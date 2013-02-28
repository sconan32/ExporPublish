using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Maths;
using Socona.Expor.Maths.Scales;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Maths.Histograms
{

    public class FlexiHistogram<T, D> : AggregatingHistogram<T, D>
    {
        /**
         * Adapter class, extended "Maker".
         */
        private FlexAdapter<T, D> Downsampler;

        /**
         * Cache for elements when not yet initialized.
         */
        private List<DoubleObjPair<D>> tempcache = null;

        /**
         * Destination (minimum) size of the structure. At most 2*destsize bins are
         * allowed.
         */
        private int destsize;

        /**
         * Adapter interface to specify bin creation, data caching and combination.
         * 
         * @author Erich Schubert
         * 
         * @param <T> Type of data in histogram
         * @param <D> Type of input data
         */

        /**
         * Create a new histogram for an unknown data range.
         * 
         * The generated histogram is guaranteed to have within {@code bins} and
         * {@code 2*bins} bins in length.
         * 
         * @param bins TarGet number of bins
         * @param adapter Adapter for data types and combination rules.
         */
        public FlexiHistogram(int bins, FlexAdapter<T, D> adapter) :
            base(bins, 0.0, 1.0, adapter)
        {
            this.destsize = bins;
            this.Downsampler = adapter;
            tempcache = new List<DoubleObjPair<D>>(this.destsize * 2);
        }

        private void Materialize()
        {
            lock (this)
            {
                // already Materialized?
                if (tempcache == null)
                {
                    return;
                }
                // we can't really initialize, but since we have to, we'll just stick
                // to 0.0 and 1.0 as used in the constructor.
                if (tempcache.Count <= 0)
                {
                    tempcache = null;
                    return;
                }
                double min = Double.MaxValue;
                double max = Double.MinValue;
                foreach (DoubleObjPair<D> pair in tempcache)
                {
                    min = Math.Min(min, pair.first);
                    max = Math.Max(max, pair.first);
                }
                // use the LinearScale magic to round to "likely suiteable" step sizes.
                LinearScale scale = new LinearScale(min, max);
                min = scale.GetMin();
                max = scale.GetMax();
                this._base = min;
                this.max = max;
                this.binsize = (max - min) / this.destsize;
                // initialize array
                this.data = new List<T>(this.destsize * 2);
                for (int i = 0; i < this.destsize; i++)
                {
                    this.data.Add(Downsampler.Make());
                }
                // re-insert data we have
                foreach (DoubleObjPair<D> pair in tempcache)
                {
                    base.Aggregate(pair.first, pair.second);
                }
                // delete cache, signal that we're initialized
                tempcache = null;
            }
        }


        public override void Replace(double coord, T d)
        {
            lock (this)
            {
                Materialize();
                // base class put will already handle histogram resizing
                base.Replace(coord, d);
                // but not resampling
                TestResample();
            }
        }

        private void TestResample()
        {
            while (base.size >= 2 * this.destsize)
            {
                // Resampling.
                List<T> newdata = new List<T>(this.destsize * 2);
                for (int i = 0; i < base.size; i += 2)
                {
                    if (i + 1 < base.size)
                    {
                        newdata.Add(Downsampler.Downsample(base.data[i], base.data[i + 1]));
                    }
                    else
                    {
                        newdata.Add(Downsampler.Downsample(base.data[i], base.Make()));
                    }
                }
                // recalculate histogram base.
                double _base = base._base - base.offset * base.binsize;
                // update data
                base.data = newdata;
                // update sizes
                base._base = _base;
                base.offset = 0;
                base.size = newdata.Count;
                base.binsize = base.binsize * 2;
                base.max = base._base + base.binsize * base.size;
            }
        }


        public override T Get(double coord)
        {
            Materialize();
            return base.Get(coord);
        }


        public override double GetBinsize()
        {
            Materialize();
            return base.GetBinsize();
        }


        public override double GetCoverMaximum()
        {
            Materialize();
            return base.GetCoverMaximum();
        }


        public override double GetCoverMinimum()
        {
            Materialize();
            return base.GetCoverMinimum();
        }


        public override List<T> GetData()
        {
            Materialize();
            return base.GetData();
        }


        public override int GetNumBins()
        {
            Materialize();
            return base.GetNumBins();
        }


        public override double GetBinMean(int bin)
        {
            Materialize();
            return base.GetBinMean(bin);
        }


        public override double GetBinMin(int bin)
        {
            Materialize();
            return base.GetBinMin(bin);
        }


        public override double GetBinMax(int bin)
        {
            Materialize();
            return base.GetBinMax(bin);
        }


        //public Iterator<DoubleObjPair<T>> iterator() {
        //  Materialize();
        //  return base.iterator();
        //}


        //public Iterator<DoubleObjPair<T>> reverseIterator() {
        //  Materialize();
        //  return base.reverseIterator();
        //}


        public override void Aggregate(double coord, D value)
        {
            if (tempcache != null)
            {
                if (tempcache.Count < this.destsize * 2)
                {
                    tempcache.Add(new DoubleObjPair<D>(coord, Downsampler.CloneForCache(value)));
                    return;
                }
                else
                {
                    Materialize();
                    // .. and continue below!
                }
            }
            // base class put will already handle histogram resizing
            base.Aggregate(coord, value);
            // but not resampling
            TestResample();
        }

        /**
         * Convenience constructor for int-based Histograms. Uses a constructor to
         * initialize bins with int(0)
         * 
         * @param bins Number of bins
         * @return New histogram for int.
         */
        public static FlexiHistogram<int, int> IntSumHistogram(int bins)
        {
            return new FlexiHistogram<int, int>(bins, new FlexIntIntAdapter());
        }

        /**
         * Convenience constructor for long-based Histograms. Uses a constructor to
         * initialize bins with long(0)
         * 
         * @param bins Number of bins
         * @return New histogram for long.
         */
        public static FlexiHistogram<long, long> longSumHistogram(int bins)
        {
            return new FlexiHistogram<long, long>(bins, new FlexLongLongAdapter());
        }

        /**
         * Convenience constructor for Double-based Histograms. Uses a constructor to
         * initialize bins with Double(0), and downsampling is done by summation.
         * 
         * @param bins Number of bins
         * @return New histogram for Doubles.
         */
        public static FlexiHistogram<Double, Double> DoubleSumHistogram(int bins)
        {
            return new FlexiHistogram<Double, Double>(bins, new FlexDoubleDoubleAdapter());
        }

        /**
         * Convenience constructor for {@link MeanVariance}-based Histograms. Uses a
         * constructor to initialize bins with new {@link MeanVariance} objects
         * 
         * @param bins Number of bins
         * @return New histogram for {@link MeanVariance}.
         */
        public static FlexiHistogram<MeanVariance, Double> MeanVarianceHistogram(int bins)
        {
            return new FlexiHistogram<MeanVariance, Double>(bins, new FlexMeanVarianceDoubleAdapter());
        }

        /**
         * Histograms that work like two {@link #IntSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @return New Histogram object
         */
        public static FlexiHistogram<IntIntPair, IntIntPair> IntSumIntSumHistogram(int bins)
        {
            return new FlexiHistogram<IntIntPair, IntIntPair>(bins, new FlexIntIntPairIntIntPairAdapter());
        }

        /**
         * Histograms that work like two {@link #longSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @return New Histogram object
         */
        public static FlexiHistogram<Pair<long, long>, Pair<long, long>> longSumlongSumHistogram(int bins)
        {
            return new FlexiHistogram<Pair<long, long>, Pair<long, long>>(bins, new FlexLongLongPairLongLongPairAdapter());
        }

        /**
         * Histograms that work like two {@link #DoubleSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @return New Histogram object
         */
        public static FlexiHistogram<DoubleDoublePair, DoubleDoublePair> DoubleSumDoubleSumHistogram(int bins)
        {
            return new FlexiHistogram<DoubleDoublePair, DoubleDoublePair>(bins, new FlexDoubleDoublePairDoubleDoublePairAdapter());
        }
    }

    public abstract class FlexAdapter<T, D> : AggrAdapter<T, D>
    {
        /**
         * Rule to combine two bins into one.
         * 
         * first and second MAY be modified and returned.
         * 
         * @param first First bin value
         * @param second Second bin value
         * @return combined bin value
         */
        public abstract T Downsample(T first, T second);

        /**
         * Clone a data passed to the algorithm for computing the initial size.
         * 
         * @param data Data to be cloned
         * @return cloned data
         */
        public abstract D CloneForCache(D data);
    }
    public class FlexIntIntAdapter : FlexAdapter<int, int>
    {

        public override int Make()
        {
            return new int();
        }


        public override int CloneForCache(int data)
        {
            // no need to clone, int are singletons
            return data;
        }


        public override int Downsample(int first, int second)
        {
            return first + second;
        }


        public override int Aggregate(int existing, int data)
        {
            return existing + data;
        }
    }
    public class FlexLongLongAdapter : FlexAdapter<long, long>
    {

        public override long Make()
        {
            return new long();
        }


        public override long CloneForCache(long data)
        {
            // no need to clone, long are singletons
            return data;
        }


        public override long Downsample(long first, long second)
        {
            return first + second;
        }


        public override long Aggregate(long existing, long data)
        {
            return existing + data;
        }
    }
    public class FlexDoubleDoubleAdapter : FlexAdapter<double, double>
    {

        public override Double Make()
        {
            return new Double();
        }


        public override Double CloneForCache(Double data)
        {
            // no need to clone, Doubles are singletons
            return data;
        }


        public override Double Downsample(Double first, Double second)
        {
            return first + second;
        }


        public override Double Aggregate(Double existing, Double data)
        {
            return existing + data;
        }
    }
    public class FlexMeanVarianceDoubleAdapter : FlexAdapter<MeanVariance, double>
    {

        public override MeanVariance Make()
        {
            return new MeanVariance();
        }


        public override Double CloneForCache(Double data)
        {
            return data;
        }


        public override MeanVariance Downsample(MeanVariance first, MeanVariance second)
        {
            first.Put(second);
            return first;
        }


        public override MeanVariance Aggregate(MeanVariance existing, Double data)
        {
            existing.Put(data);
            return existing;
        }
    }
    public class FlexIntIntPairIntIntPairAdapter : FlexAdapter<IntIntPair, IntIntPair>
    {

        public override IntIntPair Make()
        {
            return new IntIntPair(0, 0);
        }


        public override IntIntPair CloneForCache(IntIntPair data)
        {
            return new IntIntPair(data.first, data.second);
        }


        public override IntIntPair Downsample(IntIntPair first, IntIntPair second)
        {
            return new IntIntPair(first.first + second.first, first.second + second.second);
        }


        public override IntIntPair Aggregate(IntIntPair existing, IntIntPair data)
        {
            existing.first = existing.first + data.first;
            existing.second = existing.second + data.second;
            return existing;
        }
    }
    public class FlexLongLongPairLongLongPairAdapter : FlexAdapter<Pair<long, long>, Pair<long, long>>
    {

        public override Pair<long, long> Make()
        {
            return new Pair<long, long>(0L, 0L);
        }


        public override Pair<long, long> CloneForCache(Pair<long, long> data)
        {
            return new Pair<long, long>(data.GetFirst(), data.GetSecond());
        }


        public override Pair<long, long> Downsample(Pair<long, long> first, Pair<long, long> second)
        {
            return new Pair<long, long>(first.GetFirst() + second.GetFirst(), first.GetSecond() + second.GetSecond());
        }


        public override Pair<long, long> Aggregate(Pair<long, long> existing, Pair<long, long> data)
        {
            existing.SetFirst(existing.GetFirst() + data.GetFirst());
            existing.SetSecond(existing.GetSecond() + data.GetSecond());
            return existing;
        }
    }
    public class FlexDoubleDoublePairDoubleDoublePairAdapter : FlexAdapter<DoubleDoublePair, DoubleDoublePair>
    {

        public override DoubleDoublePair Make()
        {
            return new DoubleDoublePair(0.0, 0.0);
        }


        public override DoubleDoublePair CloneForCache(DoubleDoublePair data)
        {
            return new DoubleDoublePair(data.first, data.second);
        }


        public override DoubleDoublePair Downsample(DoubleDoublePair first, DoubleDoublePair second)
        {
            return new DoubleDoublePair(first.first + second.first, first.second + second.second);
        }


        public override DoubleDoublePair Aggregate(DoubleDoublePair existing, DoubleDoublePair data)
        {
            existing.first = existing.first + data.first;
            existing.second = existing.second + data.second;
            return existing;
        }
    }
}
