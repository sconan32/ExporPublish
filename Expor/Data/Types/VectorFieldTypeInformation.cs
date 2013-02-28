using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using System.Diagnostics;

namespace Socona.Expor.Data.Types
{

    /**
     * Type information to specify that a type has a fixed dimensionality.
     * 
     * @author Erich Schubert
     * 
     * @param <V> Vector type
     */
    public class VectorFieldTypeInformation<T> : VectorTypeInformation
        where T:IDataVector
    {
        /**
         * Object factory for producing new instances
         */
        private readonly T factory;

        /**
         * Labels
         */
        private String[] labels = null;

        /**
         * Constructor with given dimensionality and factory, so usually an instance.
         * 
         * @param cls Restriction java class
         * @param serializer Serializer
         * @param dim Dimensionality
         * @param labels Labels
         * @param factory Factory class
         */
        public VectorFieldTypeInformation(Type cls, IByteBufferSerializer serializer, int dim, String[] labels, T factory) :
            base(cls, serializer, dim, dim)
        {
            this.labels = labels;
            this.factory = factory;
            Debug.Assert(labels == null || labels.Length == dim, "Created vector field with incomplete labels.");
        }

        /**
         * Constructor for a request with minimum and maximum dimensionality.
         * 
         * @param cls Vector restriction class.
         * @param serializer Serializer
         * @param mindim Minimum dimensionality request
         * @param maxdim Maximum dimensionality request
         */
        public VectorFieldTypeInformation(Type cls, IByteBufferSerializer serializer, int mindim, int maxdim)
            : base(cls, serializer, mindim, maxdim)
        {
            this.factory = default(T);
        }

        /**
         * Constructor with given dimensionality and factory, so usually an instance.
         * 
         * @param cls Restriction java class
         * @param serializer Serializer
         * @param dim Dimensionality
         * @param factory Factory class
         */
        public VectorFieldTypeInformation(Type cls, IByteBufferSerializer serializer,
            int dim, T factory) :
            base(cls, serializer, dim, dim)
        {
            this.factory = factory;
        }

        /**
         * Constructor for a request with fixed dimensionality.
         * 
         * @param cls Vector restriction class.
         * @param serializer Serializer
         * @param dim Dimensionality request
         */
        public VectorFieldTypeInformation(Type cls, IByteBufferSerializer serializer, int dim)
            : base(cls, serializer, dim, dim)
        {
            this.factory = default(T);
        }

        /**
         * Constructor for a request without fixed dimensionality.
         * 
         * @param cls Vector restriction class.
         * @param serializer Serializer
         */
        public VectorFieldTypeInformation(Type cls, IByteBufferSerializer serializer)
            : base(cls, serializer)
        {
            this.factory = default(T);
        }

        /**
         * Constructor with given dimensionality and factory, so usually an instance.
         * 
         * @param cls Restriction java class
         * @param dim Dimensionality
         * @param labels Labels
         * @param factory Factory class
         */
        public VectorFieldTypeInformation(Type cls, int dim, String[] labels, T factory) :
            base(cls, dim, dim)
        {
            this.labels = labels;
            this.factory = factory;
            Debug.Assert(labels == null || labels.Length == dim, "Created vector field with incomplete labels.");
        }

        /**
         * Constructor for a request with minimum and maximum dimensionality.
         * 
         * @param cls Vector restriction class.
         * @param mindim Minimum dimensionality request
         * @param maxdim Maximum dimensionality request
         */
        public VectorFieldTypeInformation(Type cls, int mindim, int maxdim) :
            base(cls, mindim, maxdim)
        {
            this.factory = default(T);
        }

        /**
         * Constructor with given dimensionality and factory, so usually an instance.
         * 
         * @param cls Restriction java class
         * @param dim Dimensionality
         * @param factory Factory class
         */
        public VectorFieldTypeInformation(Type cls, int dim, T factory) :
            base(cls, dim, dim)
        {
            this.factory = factory;
        }

        /**
         * Constructor for a request with fixed dimensionality.
         * 
         * @param cls Vector restriction class.
         * @param dim Dimensionality request
         */
        public VectorFieldTypeInformation(Type cls, int dim) :
            base(cls, dim, dim)
        {
            this.factory = default(T);
        }

        /**
         * Constructor for a request without fixed dimensionality.
         * 
         * @param cls Vector restriction class.
         */
        public VectorFieldTypeInformation(Type cls) :
            base(cls)
        {
            this.factory = default(T);
        }


        public override bool IsAssignableFromType(ITypeInformation type)
        {
            // Do all checks from baseclass
            if (!base.IsAssignableFromType(type))
            {
                return false;
            }
            // Additionally check that mindim == maxdim.
            //VectorFieldTypeInformation<T> other = (VectorFieldTypeInformation<T>)type;
            int mindim = (int)type.GetType().
                GetField("mindim",System.Reflection. BindingFlags.NonPublic | System.Reflection. BindingFlags.Instance).
                GetValue(type);
            int maxdim = (int)type.GetType().
                GetField("maxdim", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).
                GetValue(type);
            if (mindim != maxdim)
            {
                return false;
            }
            return true;
        }

        /**
         * Get the dimensionality of the type.
         * 
         * @return dimensionality
         */
        public int Dimensionality()
        {
            if (mindim != maxdim)
            {
                throw new InvalidOperationException("Requesting dimensionality for a type request without defined dimensionality!");
            }
            return mindim;
        }

        /**
         * Get the object type factory.
         * 
         * @return the factory
         */
        public T GetFactory()
        {
            if (factory == null)
            {
                throw new InvalidOperationException("Requesting factory for a type request!");
            }
            return  factory;
        }

        /**
         * Pseudo constructor that is often convenient to use when T is not completely
         * known.
         * 
         * @param <T> Type
         * @param cls Class restriction
         * @return Type
         */
        public static VectorFieldTypeInformation<T> Get(Type cls)
        {
            return new VectorFieldTypeInformation<T>(cls);
        }

        /**
         * Pseudo constructor that is often convenient to use when T is not completely
         * known, but the dimensionality is fixed.
         * 
         * @param <T> Type
         * @param cls Class restriction
         * @param dim Dimensionality (exact)
         * @return Type
         */
        public static VectorFieldTypeInformation<T> Get(Type cls, int dim)
        {
            return new VectorFieldTypeInformation<T>(cls, dim);
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(GetRestrictionClass().Name);
            if (mindim == maxdim)
            {
                buf.Append(",dim=").Append(mindim);
            }
            else
            {
                buf.Append(",field");
                if (mindim >= 0)
                {
                    buf.Append(",mindim=" + mindim);
                }
                if (maxdim < Int32.MaxValue)
                {
                    buf.Append(",maxdim=" + maxdim);
                }
            }
            return buf.ToString();
        }

        /**
         * Get the column label
         * 
         * @param col Column number
         * @return Label
         */
        public String GetLabel(int col)
        {
            if (labels == null)
            {
                return null;
            }
            return labels[col - 1];
        }
    }
}
