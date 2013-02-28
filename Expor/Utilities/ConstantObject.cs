using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities
{
    public abstract class ConstantObject<D> : IComparable<D> where D : ConstantObject<D>
    {
        /**
   * Index of constant objects.
   */
        private static readonly Dictionary<Type, Dictionary<String, object>> CONSTANT_OBJECTS_INDEX =
              new Dictionary<Type, Dictionary<String, object>>();

        /**
         * Holds the value of the property's name.
         */
        private readonly String name;

        /**
         * The cached hash code of this object.
         */
        private readonly int hashCode;

        /**
         * Provides a ConstantObject of the given name.
         * 
         * @param name name of the ConstantObject
         */
        protected ConstantObject(String name)
        {
            if (name == null)
            {
                throw new ArgumentException("The name of a constant object must not be null.");
            }
            Dictionary<String, object> index;
            if (CONSTANT_OBJECTS_INDEX.ContainsKey(this.GetType()))
            {
                index = CONSTANT_OBJECTS_INDEX[this.GetType()];
            }
            else
            {
                index = new Dictionary<String, object>();
                CONSTANT_OBJECTS_INDEX[this.GetType()] = index;
            }
            if (index.ContainsKey(name))
            {
                throw new ArgumentException("A constant object of type \"" + this.GetType().FullName + "\" with value \"" + name + "\" is existing already.");
            }
            this.name = name;
            index[name] = this;
            this.hashCode = name.GetHashCode();
        }

        /**
         * Returns the name of the ConstantObject.
         * 
         * @return the name of the ConstantObject
         */
        public String Name
        {
            get { return name; }
        }

        /**
         * Provides a ConstantObject of specified class and name if it exists.
         * 
         * @param <D> Type for compile time type checking
         * @param type the type of the desired ConstantObject
         * @param name the name of the desired ConstantObject
         * @return the ConstantObject of designated type and name if it exists, null
         *         otherwise
         */

        public static D Lookup(Type type, String name)
        {
            if (CONSTANT_OBJECTS_INDEX.ContainsKey(type))
            {
                Dictionary<String, object> typeindex = CONSTANT_OBJECTS_INDEX[type];
                if (typeindex.ContainsKey(name))
                {
                    object val = typeindex[name];
                    return (D)val;
                }
            }
            return null;
        }

        /**
         * Method for use by the serialization mechanism to ensure identity of
         * ConstantObjects.
         * 
         * @return the ConstantObject that already exists in the virtual machine
         *         rather than a new instance as created by the serialization
         *         mechanism
         */

        protected Object ReadResolve()
        {
            Object result = Lookup(GetType(), Name);
            if (result == null)
            {
                throw new NullReferenceException("No constant object of type \"" + GetType().FullName + "\" found for name \"" + Name + "\".");
            }
            return result;
        }

        /**
         * @see Object#equals(Object)
         */

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            D that = o as D;

            if (GetHashCode() != that.GetHashCode())
            {
                return false;
            }
            if (name == null)
            {
                return (that.Name == null);
            }
            return name.Equals(that.Name);
        }

        /**
         * @see Object#hashCode()
         */

        public override int GetHashCode()
        {
            return hashCode;
        }

        /**
         * Two constant objects are generally compared by their name. The result
         * reflects the lexicographical order of the names by
         * {@link String#compareTo(String) this.getName().compareTo(o.getName()}.
         * @param o Object to compare to.
         * @return comparison result
         * 
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */

        public int CompareTo(D o)
        {
            return this.Name.CompareTo(o.Name);
        }
    }
}
