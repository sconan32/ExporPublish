using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.Utilities
{

    public class ClassGenericsUtil
    {
        /**
         * Static logger to use.
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(ClassGenericsUtil));

        /**
         * Name for a static "parameterize" factory method.
         */
        public static readonly String FACTORY_METHOD_NAME = "Parameterize";

        /**
         * <p>
         * Returns a new instance of the given type for the specified className.
         * </p>
         * 
         * <p>
         * If the Class for className is not found, the instantiation is tried using
         * the package of the given type as package of the given className.
         * </p>
         * 
         * @param <T> Class type for compile time type checking
         * @param type desired Class type of the Object to retrieve
         * @param className name of the class to instantiate
         * @return a new instance of the given type for the specified className
         * @throws UnableToComplyException if the instantiation cannot be performed
         *         successfully
         */
        public static T Instantiate<T>(Type type, String className)
        {
            T instance = default(T);
            ObjectHandle oh = null;
            try
            {
                try
                {
                    oh = Activator.CreateInstance(null, className);
                }
                catch (TypeLoadException)
                {
                    // try package of type
                    oh = Activator.CreateInstance(null, type.Namespace + "." + className);
                }
                if (type.IsInstanceOfType(oh.Unwrap()))
                {
                    instance = (T)Convert.ChangeType(oh.Unwrap(), type);
                }
            }
            catch (TypeLoadException e)
            {
                throw new UnableToComplyException("", e);
            }
            catch (InvalidCastException e)
            {
                throw new UnableToComplyException("", e);
            }


            return instance;
        }

        /**
         * <p>
         * Returns a new instance of the given type for the specified className.
         * </p>
         * 
         * <p>
         * If the Class for className is not found, the instantiation is tried using
         * the package of the given type as package of the given className.
         * </p>
         * 
         * <p>
         * This is a weaker type checked version of "{@link #instantiate}" for use
         * with Generics.
         * </p>
         * 
         * @param <T> Class type for compile time type checking
         * @param type desired Class type of the Object to retrieve
         * @param className name of the class to instantiate
         * @return a new instance of the given type for the specified className
         * @throws UnableToComplyException if the instantiation cannot be performed
         *         successfully
         */

        public static T InstantiateGenerics<T>(Type type, String className)
        {
            T instance = default(T);
            ObjectHandle oh = null;
            // TODO: can we do a verification that type conforms to T somehow?
            // (probably not because generics are implemented via erasure.
            try
            {
                try
                {
                    oh = Activator.CreateInstance(null, className);
                }
                catch (TypeLoadException)
                {
                    // try package of type
                    oh = Activator.CreateInstance(null, type.Namespace + "." + className);
                }
                if (type.IsInstanceOfType(oh.Unwrap()))
                {
                    instance = (T)Convert.ChangeType(oh.Unwrap(), type);
                }
            }
            catch (TypeLoadException e)
            {
                throw new UnableToComplyException("", e);
            }
            catch (InvalidCastException e)
            {
                throw new UnableToComplyException("", e);
            }
            return instance;
        }

        /**
         * Inspect the class for a static "parameterize" method that satisfies certain
         * constraints.
         * 
         * @param <C> Return class type
         * @param c Class to inspect.
         * @param ret Expected return type
         * @return factory method that can be called with
         *         {@code factory(null, Parameterization)}.
         * @throws NoSuchMethodException When no factory method was found, or it
         *         doesn't fit the constraints.
         * @throws Exception On other errors such as security exceptions
         */
        public static MethodInfo GetParameterizationFactoryMethod<C>(Type c, Type ret)
        {
            MethodInfo m = c.GetMethod(FACTORY_METHOD_NAME, new[] { typeof(IParameterization) });
            if (m == null)
            {
                throw new MissingMethodException("No parameterization method found.");
            }
            if (!ret.IsAssignableFrom(m.ReturnType))
            {
                throw new MissingMethodException("Return type doesn't match: " + m.ReturnType.Name + ", expected: " + ret.Name);
            }
            if (!m.IsStatic)
            {
                throw new MissingMethodException("Factory method is not static.");
            }
            return m;
        }

        /**
         * Get a parameterizer for the given class.
         * 
         * @param c Class
         * @return Parameterizer or null.
         */
        public static IParameterizer GetParameterizer(Type c)
        {
            foreach (Type inner in c.GetNestedTypes())
            {
                if (typeof(IParameterizer).IsAssignableFrom(inner))
                {
                    try
                    {
                        if (inner.IsGenericType)
                        {

                            Type[] gtypes = c.GetGenericArguments();
                            Type contype = inner.MakeGenericType(gtypes);
                            object o = Activator.CreateInstance(contype);
                            return o as IParameterizer;
                        }
                        else
                        {
                            object o = Activator.CreateInstance(inner);
                            return o as IParameterizer;
                        }
                    }
                    catch (Exception)
                    {
                        logger.Warning("Non-usable Parameterizer in class: " + c.Name);
                    }
                }
            }
            return null;
        }

        /**
         * Instantiate a parameterizable class. When using this, consider using
         * {@link Parameterization#descend}!
         * 
         * @param <C> base type
         * @param r Base (restriction) class
         * @param c Class to instantiate
         * @param config Configuration to use for instantiation.
         * @return Instance
         * @throws InvocationTarGetException when an exception occurred within the
         *         constructor
         * @throws NoSuchMethodException when no suitable constructor was found
         * @throws Exception when other instantiation errors occurred
         */
        public static C TryInstantiate<C>(Type r, Type c, IParameterization config)
        {
            if (c == null)
            {
                // TODO: better class? AbortException maybe?
                throw new InvalidOperationException("Trying to instantiate 'null' class!");
            }
            // Try a V3 parameterization class
            IParameterizer par = GetParameterizer(c);
            // TODO: API good?
            if (par != null && par is AbstractParameterizer)
            {
                Object instance = ((AbstractParameterizer)par).Make(config);
                return (C)instance;
            }
            // Try a V2 static parameterization method
            try
            {
                MethodInfo factory = GetParameterizationFactoryMethod<C>(c, r);
                Object instance = factory.Invoke(null, new[] { config });
                return (C)instance;
            }
            catch (MissingMethodException)
            {
                // continue.
            }
            // Try a regular "parameterization" constructor
            try
            {
                ConstructorInfo constructor = c.GetConstructor(new[] { typeof(IParameterization) });
                Object instance = constructor.Invoke(new[] { config });
                return (C)instance;
            }
            catch (MissingMethodException)
            {
                // continue
            }
            // Try a default constructor.
            Object obj = c.GetConstructor(System.Type.EmptyTypes).Invoke(new object[0]);
            return (C)obj;
        }

        /**
         * Force parameterization method.
         * 
         * Please use this only in "runner" classes such as unit tests, since the
         * error handling is not very flexible.
         * 
         * @param <C> Type
         * @param c Class to instantiate
         * @param config Parameters
         * @return Instance or throw an AbortException
         */

        public static C ParameterizeOrAbort<C>(Type c, IParameterization config)
        {
            try
            {
                return TryInstantiate<C>((Type)c, c, config);
            }
            catch (Exception e)
            {
                throw new AbortException("Instantiation failed", e);
            }
        }

        /**
         * Create an array (of null values)
         * 
         * This is a common unchecked cast we have to do due to Java Generics
         * limitations.
         * 
         * @param <T> Type the array elements have
         * @param len array size
         * @param base template class for array creation.
         * @return new array of null pointers.
         */

        public static T[] NewArrayOfNull<T>(int len, Type baseType)
        {
            Array arr = Array.CreateInstance(baseType, len);
            return (T[])arr;
        }

        /**
         * Convert a collection to an array.
         * 
         * @param <B> Base type
         * @param <T> Type the array elements have
         * @param coll collection to convert.
         * @param base Template class for array creation.
         * @return new array with the collection contents.
         */

        public static T[] ToArray<T>(ICollection<T> coll, Type baseType)
        {
            return coll.ToArray<T>();
        }

        /**
         * Create an array of <code>len</code> empty ArrayLists.
         * 
         * This is a common unchecked cast we have to do due to Java Generics
         * limitations.
         * 
         * @param <T> Type the list elements have
         * @param len array size
         * @return new array of ArrayLists
         */

        public static IList<T>[] NewArrayOfEmptyArrayList<T>(int len)
        {
            IList<T>[] result = new IList<T>[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = new List<T>();
            }
            return result;
        }

        /**
         * Create an array of <code>len</code> empty HashSets.
         * 
         * This is a common unchecked cast we have to do due to Java Generics
         * limitations.
         * 
         * @param <T> Type the set elements have
         * @param len array size
         * @return new array of HashSets
         */

        public static HashSet<T>[] newArrayOfEmptyHashSet<T>(int len)
        {
            HashSet<T>[] result = new HashSet<T>[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = new HashSet<T>();
            }
            return result;
        }

        /**
         * Cast the (erased) generics onto a class.
         * 
         * Note: this function is a hack - notice that it would allow you to up-cast
         * any class! Still it is preferable to have this cast in one place than in
         * dozens without any explanation.
         * 
         * The reason this is needed is the following: There is no
         * Class&lt;Set&lt;String&gt;&gt;.class. This method allows you to do <code>
         * Class&lt;Set&lt;String&gt;&gt; setclass = uglyCastIntoSubclass(Set.class);
         * </code>
         * 
         * We can't type check at runtime, since we don't have T.
         * 
         * @param cls Class type
         * @param <D> Base type
         * @param <T> Supertype
         * @return {@code cls} parameter, but cast to {@code Class<T>}
         */

        public static Type UglyCastIntoSubclass(Type cls)
        {
            return cls;
        }

        /**
         * This class performs an ugly cast, from <code>Class&lt;F&gt;</code> to
         * <code>Class&lt;T&gt;</code>, where both F and T need to extend B.
         * 
         * The restrictions are there to avoid misuse of this cast helper.
         * 
         * While this sounds really ugly, the common use case will be something like
         * 
         * <pre>
         * BASE = Class&lt;Database&gt;
         * FROM = Class&lt;Database&gt;
         * TO = Class&lt;Database&lt;V&gt;&gt;
         * </pre>
         * 
         * i.e. the main goal is to add missing Generics to the compile time type.
         * 
         * @param <BASE> Base type
         * @param <TO> Destination type
         * @param <FROM> Source type
         * @param cls Class to be cast
         * @param base Base class for type checking.
         * @return Casted class.
         */

        public static Type UglyCrossCast(Type cls, Type baseType)
        {
            if (!baseType.IsAssignableFrom(cls))
            {
                if (cls == null)
                {
                    throw new InvalidCastException("Attempted to use 'null' as class.");
                }
                throw new InvalidCastException(cls.Name + " is not a superclass of " + baseType.Name);
            }
            return cls;
        }

        /**
         * Cast an object at a base class, but return a subclass (for Generics!).
         * 
         * The main goal of this is to allow casting an object from e.g. "
         * <code>List</code>" to "<code>List&lt;Something&gt;</code>" without having
         * to add SuppressWarnings everywhere.
         * 
         * @param <B> Base type to cast at
         * @param <T> Derived type returned
         * @param base Base class to cast at
         * @param obj Object
         * @return Cast object or null.
         */

        public static T CastWithGenericsOrNull<T>(Type baseType, Object obj)
        {
            try
            {
                return (T)obj;
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /**
         * Generic newInstance that tries to clone an object.
         * 
         * @param <T> Object type, generic
         * @param obj Master copy - must not be null.
         * @return New instance, if possible
         * @throws InstantiationException on error
         * @throws IllegalAccessException on error
         * @throws InvocationTarGetException on error
         * @throws NoSuchMethodException on error
         */

        public static T NewInstance<T>(T obj)
        {
            try
            {
                Object n = obj.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                return (T)n;
            }
            catch (NullReferenceException e)
            {
                throw new ArgumentException("Null pointer exception in newInstance()", e);
            }
        }

        /**
         * Retrieve the component type of a given array. For cloning.
         * 
         * @param <T> Array type, generic
         * @param a Existing array
         * @return Component type of the given array.
         */

        public static Type GetComponentType<T>(T[] a)
        {
            Type k = typeof(T);
            return k;
        }

        /**
         * Make a new array of the given class and size.
         * 
         * @param <T> Generic type
         * @param k Class
         * @param size Size
         * @return new array of the given type
         */

        public static T[] NewArray<T>(Type k, int size)
        {
            if (k.IsPrimitive)
            {
                throw new ArgumentException("Argument cannot be primitive: " + k.Name);
            }
            Object a = Array.CreateInstance(k, size);
            return (T[])a;
        }

        /**
         * Clone an array of the given type.
         * 
         * @param <T> Generic type
         * @param a existing array
         * @param size array size
         * @return new array
         */
        public static T[] NewArray<T>(T[] a, int size)
        {
            return NewArray<T>(typeof(T), size);
        }

        /**
         * Clone a collection. Collection must have an empty constructor!
         * 
         * @param <T> Data type
         * @param <C> Collection type
         * @param coll Existing collection
         * @return Cloned collection
         */
        public static C CloneCollection<C, T>(C coll) where C : ICollection<T>
        {

            C copy = NewInstance(coll);
            foreach (var i in copy)
            {
                copy.Add(i);
            }
            return copy;


        }

        /**
         * Transform a collection to an Array
         * 
         * @param <T> object type
         * @param c Collection
         * @param a Array to write to or replace (i.e. sample array)
         * @return new array containing the collection elements
         */
        public static T[] CollectionToArray<T>(ICollection<T> c, T[] a)
        {
            if (a.Length < c.Count)
            {
                a = NewArray(a, c.Count);
            }
            int i = 0;
            foreach (T x in c)
            {
                a[i] = x;
                i++;
            }
            if (i < a.Length)
            {
                a[i] = default(T);
            }
            return a;
        }
    }

}
