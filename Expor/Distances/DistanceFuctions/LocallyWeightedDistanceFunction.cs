using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed;
using Socona.Expor.Indexes.Preprocessed.LocalPca;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Distances.DistanceFuctions
{

    // FIXME: implements SpatialPrimitiveDistanceFunction<V, DoubleDistance>
    public class LocallyWeightedDistanceFunction<V> :
        AbstractIndexBasedDistanceFunction<V>, IFilteredLocalPCABasedDistanceFunction<V>
    where V : INumberVector
    {

        /**
         * Constructor
         * 
         * @param indexFactory Index factory
         */
        public LocallyWeightedDistanceFunction(ILocalProjectionIndexFactory indexFactory) :
            base(indexFactory)
        {
        }


        public override IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
        }


        public bool isMetric()
        {
            return false;
        }


        public bool isSymmetric()
        {
            return true;
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            // We can't really avoid these warnings, due to a limitation in Java
            // Generics (AFAICT)

            ILocalProjectionIndex indexinst = (ILocalProjectionIndex)indexFactory.
                Instantiate((IRelation)database);
            return new Instance(database, indexinst, this);
        }
      

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            if (this.indexFactory.Equals(((LocallyWeightedDistanceFunction<V>)obj).indexFactory))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this.indexFactory.GetHashCode() ^ base.GetHashCode();
        }
        /**
         * Instance of this distance for a particular database.
         * 
         * @author Erich Schubert
         */
        public new class Instance :
            AbstractIndexBasedDistanceFunction<V>.Instance, IFilteredLocalPCABasedDistanceFunctionInstance
        {
            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Index
             * @param distanceFunction Distance Function
             */
            public Instance(IRelation database, ILocalProjectionIndex index,
                LocallyWeightedDistanceFunction<V> distanceFunction) :
                base(database, index, distanceFunction)
            {
            }

            /**
             * Computes the distance between two given real vectors according to this
             * distance function.
             * 
             * @param id1 first object id
             * @param id2 second object id
             * @return the distance between two given real vectors according to this
             *         distance function
             */

            public override IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
            {
                var ind = index as ILocalProjectionIndex;
                Matrix m1 = ind.GetLocalProjection(id1).SimilarityMatrix();
                Matrix m2 = ind.GetLocalProjection(id2).SimilarityMatrix();

                if (m1 == null || m2 == null)
                {
                    return new DoubleDistanceValue(Double.PositiveInfinity);
                }

                V v1 = (V)relation[(id1)];
                V v2 = (V)relation[(id2)];
                Vector v1Mv2 = v1.GetColumnVector().MinusEquals(v2.GetColumnVector());
                Vector v2Mv1 = v2.GetColumnVector().MinusEquals(v1.GetColumnVector());

                double dist1 = v1Mv2.TransposeTimesTimes(m1, v1Mv2);
                double dist2 = v2Mv1.TransposeTimesTimes(m2, v2Mv1);

                if (dist1 < 0)
                {
                    if (-dist1 < 0.000000000001)
                    {
                        dist1 = 0;
                    }
                    else
                    {
                        throw new ArgumentException("dist1 " + dist1 + "  < 0!");
                    }
                }
                if (dist2 < 0)
                {
                    if (-dist2 < 0.000000000001)
                    {
                        dist2 = 0;
                    }
                    else
                    {
                        throw new ArgumentException("dist2 " + dist2 + "  < 0!");
                    }
                }

                return new DoubleDistanceValue(Math.Max(Math.Sqrt(dist1), Math.Sqrt(dist2)));
            }

            // 
            // TODO: re-enable spatial interfaces
            public DoubleDistanceValue minDistBROKEN(ISpatialComparable mbr, V v)
            {
                if (mbr.Count != v.Count)
                {
                    throw new ArgumentException("Different dimensionality of objects\n  first argument: " + mbr.ToString() + "\n  second argument: " + v.ToString());
                }

                double[] r = new double[v.Count];
                for (int d = 1; d <= v.Count; d++)
                {
                    double value = v[(d)];
                    if (value < mbr.GetMin(d))
                    {
                        r[d - 1] = mbr.GetMin(d);
                    }
                    else if (value > mbr.GetMax(d))
                    {
                        r[d - 1] = mbr.GetMax(d);
                    }
                    else
                    {
                        r[d - 1] = value;
                    }
                }

                Matrix m = null; // index.GetLocalProjection(v.GetID()).similarityMatrix();
                Vector rv1Mrv2 = v.GetColumnVector().MinusEquals(new Vector(r));
                double dist = rv1Mrv2.TransposeTimesTimes(m, rv1Mrv2);

                return new DoubleDistanceValue(Math.Sqrt(dist));
            }

            // TODO: Remove?
            // 
            // public DoubleDistance minDist(SpatialComparable mbr, IDbId id) {
            // return minDist(mbr, database.Get(id));
            // }

            // 
            // TODO: re-enable spatial interface
            public DoubleDistanceValue distance(ISpatialComparable mbr1, ISpatialComparable mbr2)
            {
                if (mbr1.Count != mbr2.Count)
                {
                    throw new ArgumentException("Different dimensionality of objects\n  first argument: " + mbr1.ToString() + "\n  second argument: " + mbr2.ToString());
                }

                double sqrDist = 0;
                for (int d = 1; d <= mbr1.Count; d++)
                {
                    double m1, m2;
                    if (mbr1.GetMax(d) < mbr2.GetMin(d))
                    {
                        m1 = mbr2.GetMin(d);
                        m2 = mbr1.GetMax(d);
                    }
                    else if (mbr1.GetMin(d) > mbr2.GetMax(d))
                    {
                        m1 = mbr1.GetMin(d);
                        m2 = mbr2.GetMax(d);
                    }
                    else
                    { // The mbrs intersect!
                        m1 = 0;
                        m2 = 0;
                    }
                    double manhattanI = m1 - m2;
                    sqrDist += manhattanI * manhattanI;
                }
                return new DoubleDistanceValue(Math.Sqrt(sqrDist));
            }

            // 
            // TODO: re-enable spatial interface
            public DoubleDistanceValue centerDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
            {
                if (mbr1.Count != mbr2.Count)
                {
                    throw new ArgumentException("Different dimensionality of objects\n first argument:  " + mbr1.ToString() + "\n  second argument: " + mbr2.ToString());
                }

                double sqrDist = 0;
                for (int d = 1; d <= mbr1.Count; d++)
                {
                    double c1 = (mbr1.GetMin(d) + mbr1.GetMax(d)) / 2;
                    double c2 = (mbr2.GetMin(d) + mbr2.GetMax(d)) / 2;

                    double manhattanI = c1 - c2;
                    sqrDist += manhattanI * manhattanI;
                }
                return new DoubleDistanceValue(Math.Sqrt(sqrDist));
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractIndexBasedDistanceFunction<V>.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigIndexFactory(config, typeof(ILocalProjectionIndexFactory),
                    typeof(KNNQueryFilteredPCAIndex.Factory));
            }


            protected override object MakeInstance()
            {
                return new LocallyWeightedDistanceFunction<V>((ILocalProjectionIndexFactory)factory);
            }
        }
    }
}
