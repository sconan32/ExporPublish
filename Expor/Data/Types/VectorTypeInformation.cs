using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using System.Diagnostics;

namespace Socona.Expor.Data.Types
{

    /**
     * Construct a type information for vector spaces with fixed dimensionality.
     * 
     * @author Erich Schubert
     * 
     */
    public class VectorTypeInformation : SimpleTypeInformation
    {
        /**
         * Minimum dimensionality
         */
        protected readonly int mindim;

        /**
         * Maximum dimensionality
         */
        protected readonly int maxdim;

        /**
         * Constructor.
         * 
         * @param cls base class
         * @param serializer Serializer
         * @param mindim Minimum dimensionality
         * @param maxdim Maximum dimensionality
         */
        public VectorTypeInformation(Type cls, IByteBufferSerializer serializer, int mindim, int maxdim)
            : base(cls, serializer)
        {
            Debug.Assert(this.mindim <= this.maxdim);
            this.mindim = mindim;
            this.maxdim = maxdim;
        }

        /**
         * Constructor without size constraints.
         * 
         * @param cls base class
         * @param serializer Serializer
         */
        public VectorTypeInformation(Type cls, IByteBufferSerializer serializer) :
            this(cls, serializer, -1, Int32.MaxValue)
        {
        }

        /**
         * Constructor.
         * 
         * @param cls base class
         * @param mindim Minimum dimensionality
         * @param maxdim Maximum dimensionality
         */
        public VectorTypeInformation(Type cls, int mindim, int maxdim) :
            this(cls, null, mindim, maxdim)
        {
        }

        /**
         * Constructor without size constraints.
         * 
         * @param cls
         */
        public VectorTypeInformation(Type cls) :
            this(cls, null, -1, Int32.MaxValue)
        {
        }


        public override bool IsAssignableFromType(ITypeInformation type)
        {
            // This validates the base type V
            if (!base.IsAssignableFromType(type))
            {
                return false;
            }
            // Other type must also be a vector type
            if (!(type is VectorTypeInformation))
            {
                return false;
            }
            VectorTypeInformation othertype = (VectorTypeInformation)type;
            Debug.Assert(othertype.mindim <= othertype.maxdim);
            // the other must not have a lower minimum dimensionality
            if (this.mindim > othertype.mindim)
            {
                return false;
            }
            // ... or a higher maximum dimensionality.
            if (othertype.maxdim > this.maxdim)
            {
                return false;
            }
            return true;
        }


        public override bool IsAssignableFrom(Object other)
        {
            // Validate that we can assign
            if (!base.IsAssignableFrom(other))
            {
                return false;
            }
            // Get the object dimensionality
            int odim = Cast<IDataVector>(other).Count;
            if (odim < mindim)
            {
                return false;
            }
            if (odim > maxdim)
            {
                return false;
            }
            return true;
        }

        /**
         * Get the minimum dimensionality of the occurring vectors.
         * 
         * @return dimensionality
         */
        public int GetMindim()
        {
            if (mindim < 0)
            {
                throw new InvalidOperationException("Requesting dimensionality for a request without defined dimensionality!");
            }
            return mindim;
        }

        /**
         * Get the maximum dimensionality of the occurring vectors.
         * 
         * @return dimensionality
         */
        public int GetMaxdim()
        {
            if (maxdim == Int32.MaxValue)
            {
                throw new InvalidOperationException("Requesting dimensionality for a request without defined dimensionality!");
            }
            return maxdim;
        }

        /**
         * Pseudo constructor that is often convenient to use when T is not completely known.
         * 
         * @param <T> Type
         * @param cls Class restriction
         * @param mindim Minimum dimensionality
         * @param maxdim Maximum dimensionality
         * @return Type information
         */
        public static VectorTypeInformation Get(Type cls, int mindim, int maxdim)
        {
            return new VectorTypeInformation(cls, mindim, maxdim);
        }

        /**
         * Pseudo constructor that is often convenient to use when T is not completely known.
         * 
         * @param <T> Type
         * @param cls Class restriction
         * @return Type information
         */
        //public static <T extends FeatureVector<?, ?>> VectorTypeInformation<T> get(Class<T> cls) {
        //   return new VectorTypeInformation<T>(cls);
        //}


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());
            buf.Append(",variable");
            if (mindim >= 0)
            {
                buf.Append(",mindim=" + mindim);
            }
            if (maxdim < Int32.MaxValue)
            {
                buf.Append(",maxdim=" + maxdim);
            }
            return buf.ToString();
        }
    }
}
