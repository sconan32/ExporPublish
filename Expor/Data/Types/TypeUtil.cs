using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Persistent;
using MathNet.Numerics.LinearAlgebra.Double;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Data.Types
{

    /**
     * Utility package containing various common types
     * 
     * @author Erich Schubert
     * 
     * @apiviz.has TypeInformation oneway - -
     * @apiviz.landmark
     */
    public class TypeUtil
    {
        /**
         * Input type for algorithms that accept anything
         */
        public static readonly SimpleTypeInformation ANY = new SimpleTypeInformation(typeof(Object));

        /**
         * Database IDs
         */
        public static readonly SimpleTypeInformation DBID = new SimpleTypeInformation(typeof(IDbId), DbIdFactoryBase.FACTORY.GetDbIdSerializer());

        /**
         * Database ID lists
         */
        public static readonly SimpleTypeInformation DBIDS = new SimpleTypeInformation(typeof(IDbIds));

        /**
         * A string
         */
        public static readonly SimpleTypeInformation STRING = new SimpleTypeInformation(typeof(String), ByteArrayUtil.STRING_SERIALIZER);

        /**
         * A class label
         */
        public static readonly SimpleTypeInformation CLASSLABEL = new SimpleTypeInformation(typeof(ClassLabel));

        /**
         * A list of labels.
         */
        public static readonly SimpleTypeInformation LABELLIST = new SimpleTypeInformation(typeof(LabelList));

        /**
         * A list of neighbors
         */
        public static readonly SimpleTypeInformation NEIGHBORLIST =
            new SimpleTypeInformation(typeof(IDistanceDbIdResult));

        /**
         * Either class label, object labels or a string - anything that will be
         * accepted by
         * {@link de.lmu.ifi.dbs.elki.utilities.DatabaseUtil#guessObjectLabelRepresentation}
         */
        public static readonly ITypeInformation GUESSED_LABEL = new AlternativeTypeInformation(LABELLIST, CLASSLABEL, STRING);

        /**
         * Number vectors of <em>variable</em> length.
         */
        public static readonly SimpleTypeInformation NUMBER_VECTOR_VARIABLE_LENGTH =
            new SimpleTypeInformation(typeof(INumberVector));

        /**
         * Input type for algorithms that require number vector fields.
         */
        public static readonly VectorFieldTypeInformation<INumberVector> NUMBER_VECTOR_FIELD =
            new VectorFieldTypeInformation<INumberVector>(typeof(INumberVector));

        /**
         * Input type for algorithms that require number vector fields.
         * 
         * If possible, please use {@link #NUMBER_VECTOR_FIELD}!
         */
        public static readonly VectorFieldTypeInformation<DoubleVector> DOUBLE_VECTOR_FIELD =
            new VectorFieldTypeInformation<DoubleVector>(typeof(DoubleVector), DoubleVector.STATIC);

        /**
         * Input type for algorithms that require number vector fields.
         * 
         * If possible, please use {@link #NUMBER_VECTOR_FIELD}!
         */
        public static readonly VectorFieldTypeInformation<FloatVector> FLOAT_VECTOR_FIELD =
            new VectorFieldTypeInformation<FloatVector>(typeof(FloatVector), FloatVector.STATIC);

        /**
         * Input type for algorithms that require number vector fields.
         */
        public static readonly VectorFieldTypeInformation<BitVector> BIT_VECTOR_FIELD =
            new VectorFieldTypeInformation<BitVector>(typeof(BitVector));

        /**
         * Sparse float vector field.
         */
        //public static readonly SimpleTypeInformation<SparseNumberVector> SPARSE_VECTOR_VARIABLE_LENGTH = new SimpleTypeInformation<SparseNumberVector>(SparseNumberVector.class);

        ///**
        // * Sparse vector field.
        // */
        //public static readonly VectorFieldTypeInformation<SparseNumberVector> SPARSE_VECTOR_FIELD = new VectorFieldTypeInformation<SparseNumberVector<?, ?>>(SparseNumberVector.class);

        ///**
        // * Sparse float vector field.
        // * 
        // * If possible, please use {@link #SPARSE_VECTOR_FIELD} instead!
        // */
        //public static readonly VectorFieldTypeInformation<SparseFloatVector> SPARSE_FLOAT_FIELD = new VectorFieldTypeInformation<SparseFloatVector>(SparseFloatVector.class);

        /**
         * Sparse double vector field.
         * 
         * If possible, please use {@link #SPARSE_VECTOR_FIELD} instead!
         */
        public static readonly VectorFieldTypeInformation<SparseDoubleVector> SPARSE_DOUBLE_FIELD =
            new VectorFieldTypeInformation<SparseDoubleVector>(typeof(SparseDoubleVector));

        /**
         * External ID type
         */
        public static readonly SimpleTypeInformation EXTERNALID = new SimpleTypeInformation(typeof(ExternalID));

        ///**
        // * Type for polygons
        // */
        //public static readonly SimpleTypeInformation<PolygonsObject> POLYGON_TYPE = new SimpleTypeInformation<PolygonsObject>(PolygonsObject.class);

        /**
         * Double type, outlier scores etc.
         */
        public static readonly SimpleTypeInformation DOUBLE = new SimpleTypeInformation(typeof(Double), ByteArrayUtil.DOUBLE_SERIALIZER);

        /**
         * Integer type.
         */
        public static readonly SimpleTypeInformation INTEGER = new SimpleTypeInformation(typeof(Int32), ByteArrayUtil.INT_SERIALIZER);

        /**
         * Vector type.
         */
        public static readonly SimpleTypeInformation VECTOR = new SimpleTypeInformation(typeof(Vector));

        /**
         * Matrix type.
         */
        public static readonly SimpleTypeInformation MATRIX = new SimpleTypeInformation(typeof(Matrix));

        ///**
        // * Cluster model type.
        // */
        //public static readonly SimpleTypeInformation<Model> MODEL = new SimpleTypeInformation<Model>(Model.class);

        /**
         * Make a type array easily.
         * 
         * @param ts Types
         * @return array
         */
        public static ITypeInformation[] Array(params ITypeInformation[] ts)
        {
            return ts;
        }
    }
}
