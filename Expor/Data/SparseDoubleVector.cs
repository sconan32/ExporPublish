using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data
{
    public class SparseDoubleVector:MathNet.Numerics.LinearAlgebra.Double.SparseVector,INumberVector
    {

        public SparseDoubleVector():base(10)
        { }
        public Maths.LinearAlgebra.Vector GetColumnVector()
        {
            throw new NotImplementedException();
        }

        public INumberVector NewNumberVector(double[] values)
        {
            throw new NotImplementedException();
        }

       
        public IDataVector NewFeatureVector(IList<object> array, Utilities.DataStructures.ArrayLike.IArrayAdapter adapter)
        {
            throw new NotImplementedException();
        }
        public object Get (int dim)
        {
            return this[dim];
        }
       

        public double GetMin(int dimension)
        {
            throw new NotImplementedException();
        }

        public double GetMax(int dimension)
        {
            throw new NotImplementedException();
        }
    }
}
