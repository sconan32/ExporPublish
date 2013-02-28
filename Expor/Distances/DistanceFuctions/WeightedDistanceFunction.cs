using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Maths;
using Socona.Expor.Maths.LinearAlgebra;

namespace Socona.Expor.Distances.DistanceFuctions
{

    // TODO: Factory with parameterizable weight matrix?
    public class WeightedDistanceFunction : AbstractVectorDoubleDistanceFunction
    {
        /**
         * The weight matrix.
         */
        protected Matrix weightMatrix;

        /**
         * Provides the Weighted distance for feature vectors.
         * 
         * @param weightMatrix weight matrix
         */
        public WeightedDistanceFunction(Matrix weightMatrix)
            : base()
        {
            this.weightMatrix = weightMatrix;
            Debug.Assert(weightMatrix.ColumnCount == weightMatrix.RowCount);
        }

        /**
         * Provides the Weighted distance for feature vectors.
         * 
         * @return the Weighted distance between the given two vectors
         */

        public override double DoubleDistance(INumberVector o1, INumberVector o2)
        {
            Debug.Assert(o1.Count == o2.Count, "Different dimensionality of FeatureVectors" +
                "\n  first argument: " + o1.ToString() + "\n  second argument: " + o2.ToString());

            Vector o1_minus_o2 = o1.GetColumnVector().MinusEquals(o2.GetColumnVector());
            return MathUtil.MahalanobisDistance(weightMatrix, o1_minus_o2);
        }


        public override ITypeInformation GetInputTypeRestriction()
        {
            return VectorFieldTypeInformation<INumberVector>.Get(typeof(INumberVector),
                weightMatrix.ColumnCount);
        }

       

        public override DistanceValues.IDistanceValue DistanceFactory
        {
            get { throw new NotImplementedException(); }
        }
    }
}
