using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;

namespace Socona.Expor.Data.Types
{

    /**
     * Class wrapping a particular data type.
     * 
     * @author Erich Schubert
     * 
     * @param <T> Java type we represent.
     */
    public class SimpleTypeInformation : ITypeInformation
    {
        /**
         * The restriction class we represent.
         */
        private Type cls;

        /**
         * Type label
         */
        private String label = null;

        /**
         * Type serializer
         */
        private IByteBufferSerializer serializer = null;

        /**
         * Constructor.
         * 
         * @param cls restriction class
         */
        public SimpleTypeInformation(Type cls)
        {

            this.cls = cls;
        }

        /**
         * Constructor.
         * 
         * @param cls restriction class
         * @param label type label
         */
        public SimpleTypeInformation(Type cls, String label)
        {

            this.cls = cls;
            this.label = label;
        }

        /**
         * Constructor.
         * 
         * @param cls restriction class
         * @param serializer Serializer
         */
        public SimpleTypeInformation(Type cls, IByteBufferSerializer serializer)
        {

            this.cls = cls;
            this.serializer = serializer;
        }

        /**
         * Constructor.
         * 
         * @param cls restriction class
         * @param label type label
         * @param serializer Serializer
         */
        public SimpleTypeInformation(Type cls, String label, IByteBufferSerializer serializer)
        {

            this.cls = cls;
            this.label = label;
            this.serializer = serializer;
        }


        /**
         * Get the raw restriction class.
         * 
         * @return Restriction class
         */
        public Type GetRestrictionClass()
        {
            return cls;
        }


        public virtual bool IsAssignableFromType(ITypeInformation type)
        {
            if (!(type is SimpleTypeInformation))
            {
                return false;
            }
            SimpleTypeInformation simpleType = (SimpleTypeInformation)type;
            return cls.IsAssignableFrom(simpleType.GetRestrictionClass());
        }


        public virtual bool IsAssignableFrom(Object other)
        {
            return cls.IsInstanceOfType(other);
        }

        /**
         * Cast the object to type T (actually to the given restriction class!)
         * 
         * @param other Object to cast.
         * @return Instance
         */

        public T Cast<T>(Object other)
        {
            if (cls.IsInstanceOfType(other))
            {
                return  (T)(other);
            }
            else
            {
                return default(T);
            }
        }


        public override String ToString()
        {
            return GetRestrictionClass().Name;
        }

        /**
         * Get the type label
         * 
         * @return Label
         */
        public String Label
        {
            get
            {
                return label;
            }
        }

        /**
         * Get the serializer for this type.
         * 
         * @return Type serializer
         */
        public IByteBufferSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }
    }
}
