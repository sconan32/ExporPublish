using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Maths.Histograms
{

    public class AggregatingHistogram<T, D> : ReplacingHistogram<T>
    {
        /**
         * The class we are using for putting data.
         */
        private AggrAdapter<T, D> putter;


        /**
         * Constructor with Adapter.
         * 
         * @param bins Number of bins
         * @param min Minimum value
         * @param max Maximum value
         * @param adapter Adapter
         */
        public AggregatingHistogram(int bins, double min, double max,AggrAdapter<T, D> adapter) :
            base(bins, min, max, adapter)
        {
            this.putter = adapter;
        }

        /**
         * Add a value to the histogram using the aggregation adapter.
         * 
         * @param coord Coordinate
         * @param value New value
         */
        public virtual void Aggregate(double coord, D value)
        {
            base.Replace(coord, putter.Aggregate(base.Get(coord), value));
        }

        /**
         * Convenience constructor for {@link MeanVariance}-based Histograms. Uses a
         * constructor to initialize bins with new {@link MeanVariance} objects
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for {@link MeanVariance}.
         */
        public static AggregatingHistogram<MeanVariance, Double>
            MeanVarianceHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<MeanVariance, Double>(bins, min, max, new AggrMeanVarianceDoubleAdapter());
        }

        /**
         * Convenience constructor for Integer-based Histograms. Uses a constructor to
         * initialize bins with Integer(0). Aggregation is done by adding the values
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Integers.
         */
        public static AggregatingHistogram<Int32, Int32> IntSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<Int32, Int32>(bins, min, max, new AggrIntIntAdapter());
        }

        /**
         * Convenience constructor for long-based Histograms. Uses a constructor to
         * initialize bins with long(0L). Aggregation is done by adding the values
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Integers.
         */
        public static AggregatingHistogram<long, long> longSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<long, long>(bins, min, max, new AggrLongLongAdapter());
        }

        /**
         * Convenience constructor for Double-based Histograms. Uses a constructor to
         * initialize bins with Double(0.0). Aggregation is done by adding the values
         * 
         * @param bins Number of bins
         * @param min Minimum coordinate
         * @param max Maximum coordinate
         * @return New histogram for Double.
         */
        public static AggregatingHistogram<Double, Double> DoubleSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<Double, Double>(bins, min, max, new AggrDoubleDoubleAdapter());
        }

        /**
         * Histograms that work like two {@link #IntSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @param min Minimum value
         * @param max Maximum value
         * @return Histogram object
         */
        public static AggregatingHistogram<IntIntPair, IntIntPair> IntSumIntSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<IntIntPair, IntIntPair>(bins, min, max, new AggrIntIntPairIntIntPairAdapter());
        }

        /**
         * Histograms that work like two {@link #longSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @param min Minimum value
         * @param max Maximum value
         * @return Histogram object
         */
        public static AggregatingHistogram<Pair<long, long>, Pair<long, long>> LongSumLgongSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<Pair<long, long>, Pair<long, long>>(
                bins, min, max, new AggrLongLongPairLongLongPairAdapter());
        }

        /**
         * Histograms that work like two {@link #DoubleSumHistogram}, component wise.
         * 
         * @param bins Number of bins.
         * @param min Minimum value
         * @param max Maximum value
         * @return Histogram object
         */
        public static AggregatingHistogram<DoubleDoublePair, DoubleDoublePair> DoubleSumDoubleSumHistogram(int bins, double min, double max)
        {
            return new AggregatingHistogram<DoubleDoublePair, DoubleDoublePair>(
                bins, min, max, new AggrDoubleDoublePairDoubleDoublePairAdapter());
        }
    }
    /**
 * Adapter class for an AggregatingHistogram
 * 
 * @author Erich Schubert
 * 
 * @param <T> Histogram bin type
 * @param <D> Incoming data type
 */

    public abstract class AggrAdapter<T1, D1> : ReplAdapter<T1>
    {
        /**
         * Update an existing histogram value with new data.
         * 
         * @param existing Existing histogram data
         * @param data New value
         * @return Aggregated value
         */
        public abstract T1 Aggregate(T1 existing, D1 data);
    }
    public class AggrMeanVarianceDoubleAdapter :AggrAdapter<MeanVariance, double>
    {
        public override MeanVariance Make()
        {
            return new MeanVariance();
        }
        public override MeanVariance Aggregate(MeanVariance existing, double data)
        {
            existing.Put(data);
            return existing;
        }
    }
    public class AggrIntIntAdapter : AggrAdapter<int, int>
    {
        public override int Make()
        {
            return 0;
        }
        public override int Aggregate(int existing, int data)
        {
            return existing + data;
        }
    }
    public class AggrLongLongAdapter : AggrAdapter<long, long>
    {
        public override long Aggregate(long existing, long data)
        {
            return existing + data;
        }
        public override long Make()
        {
            return 0L;
        }
    }
    public class AggrDoubleDoubleAdapter : AggrAdapter<double, double>
    {
        public override double Aggregate(double existing, double data)
        {
            return existing + data;
        }
        public override double Make()
        {
            return 0.0;
        }
    }
    public class AggrIntIntPairIntIntPairAdapter :AggrAdapter<IntIntPair, IntIntPair>
    {
        public override IntIntPair Aggregate(IntIntPair existing, IntIntPair data)
        {
            existing.first = existing.first + data.first;
            existing.second = existing.second + data.second;
            return existing;
        }
        public override IntIntPair Make()
        {
            return new IntIntPair(0, 0);
        }
    }
    public class AggrLongLongPairLongLongPairAdapter : AggrAdapter<Pair<long, long>, Pair<long, long>>
    {
        public override Pair<long, long> Aggregate(Pair<long, long> existing, Pair<long, long> data)
        {
            existing.SetFirst(existing.GetFirst() + data.GetFirst());
            existing.SetSecond(existing.GetSecond() + data.GetSecond());
            return existing;
        }
        public override Pair<long, long> Make()
        {
            return new Pair<long, long>(0L, 0L);
        }
    }
    public class AggrDoubleDoublePairDoubleDoublePairAdapter : AggrAdapter<DoubleDoublePair, DoubleDoublePair>
    {
        public override DoubleDoublePair Aggregate(DoubleDoublePair existing, DoubleDoublePair data)
        {
            existing.SetFirst(existing.First + data.First);
            existing.SetSecond(existing.Second + data.Second);
            return existing;
        }
        public override DoubleDoublePair Make()
        {
            return new DoubleDoublePair(0.0, 0.0);
        }
    }
}
