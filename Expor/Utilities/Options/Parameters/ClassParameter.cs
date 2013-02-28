using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.Utilities.Options.Parameters
{

    // TODO: add additional constructors with parameter constraints.
    // TODO: turn restrictionClass into a constraint?
    public class ClassParameter : Parameter<Type>
    {
        private Logging logger = Logging.GetLogger(typeof(ClassParameter));
        /**
       * Class loader.
       */
        // protected static readonly ClassLoader loader = ClassLoader.getSystemClassLoader();

        /**
         * Factory class postfix.
         */

        public static readonly String FACTORY_POSTFIX = "$Factory";

        /**
         * The restriction class for this class parameter.
         */
        protected Type restrictionClass;

        /**
         * Constructs a class parameter with the given optionID, restriction class,
         * and default value.
         * 
         * @param optionID the unique id of the option
         * @param restrictionClass the restriction class of this class parameter
         * @param defaultValue the default value of this class parameter
         */

        public ClassParameter(OptionDescription optionID, Type restrictionClass, Type defaultValue) :
            base(optionID, defaultValue)
        {
            // It would be nice to be able to use Class<C> here, but this won't work
            // with nested Generics:
            // * ClassParameter<Foo<Bar>>(optionID, Foo.class) doesn't satisfy Class<C>
            // * ClassParameter<Foo<Bar>>(optionID, Foo<Bar>.class) isn't valid
            // * ClassParameter<Foo<Bar>>(optionID, (Class<Foo<Bar>>) Foo.class) is an
            // invalid cast.
            this.restrictionClass = restrictionClass;
            if (restrictionClass == null)
            {
                logger.Warning("Restriction class 'null' for parameter '" + optionID + "'");
            }
        }

        /**
         * Constructs a class parameter with the given optionID, restriction class,
         * and optional flag.
         * 
         * @param optionID the unique id of the option
         * @param restrictionClass the restriction class of this class parameter
         * @param optional specifies if this parameter is an optional parameter
         */

        public ClassParameter(OptionDescription optionID, Type restrictionClass, bool optional) :
            base(optionID, optional)
        {
            // It would be nice to be able to use Class<C> here, but this won't work
            // with nested Generics:
            // * ClassParameter<Foo<Bar>>(optionID, Foo.class) doesn't satisfy Class<C>
            // * ClassParameter<Foo<Bar>>(optionID, Foo<Bar>.class) isn't valid
            // * ClassParameter<Foo<Bar>>(optionID, (Class<Foo<Bar>>) Foo.class) is an
            // invalid cast.
            this.restrictionClass = restrictionClass;
            if (restrictionClass == null)
            {
                logger.Warning("Restriction class 'null' for parameter '" + optionID + "'");
            }
        }

        /**
         * Constructs a class parameter with the given optionID, and restriction
         * class.
         * 
         * @param optionID the unique id of the option
         * @param restrictionClass the restriction class of this class parameter
         */
        public ClassParameter(OptionDescription optionID, Type restrictionClass) :
            this(optionID, restrictionClass, false)
        {
        }

        protected override Type ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter Error.\n" + "No value for parameter \"" + GetName() + "\" " + "given.");
            }
            if (obj is Type)
            {
                return obj as Type;
            }
            if (obj is String)
            {
                String value = (String)obj;
                try
                {
                    // Try exact class factory first.
                    try
                    {

                        return Type.GetType(value + FACTORY_POSTFIX, true);
                    }
                    catch (TypeLoadException )
                    {
                        // Ignore, retry
                    }
                    try
                    {
                        return Type.GetType(value);
                    }
                    catch (TypeLoadException )
                    {
                        // Ignore, retry
                    }
                    // Try factory for guessed name next
                    try
                    {
                        return Type.GetType(restrictionClass.Namespace + "." + value + FACTORY_POSTFIX);
                    }
                    catch (TypeLoadException )
                    {
                        // Ignore, retry
                    }
                    // Last try: guessed name prefix only
                    return Type.GetType(restrictionClass.Namespace + "." + value);
                }
                catch (TypeLoadException e)
                {
                    throw new WrongParameterValueException(this, value, "Given class \"" + value + "\" not found.", e);
                }
            }
            throw new WrongParameterValueException(this, obj.ToString(), "Class not found for given value. Must be a subclass / implementation of " +restrictionClass.Name);
        }

        /**
         * Checks if the given parameter value is valid for this ClassParameter. If
         * not a parameter exception is thrown.
         */

        protected override bool Validate(Type obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter Error.\n" + "No value for parameter \"" + GetName() + "\" " + "given.");
            }
            if (!restrictionClass.IsAssignableFrom(obj))
            {
                throw new WrongParameterValueException(this, obj.Name, "Given class not a subclass / implementation of " + restrictionClass.Name);
            }
            if (!base.Validate(obj))
            {
                return false;
            }
            return true;
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;class&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<class>";
        }

        /**
         * This class sometimes provides a list of value descriptions.
         * 
         * @see de.lmu.ifi.dbs.elki.utilities.optionhandling.parameters.Parameter#hasValuesDescription()
         */

        public override bool HasValuesDescription()
        {
            return restrictionClass != null && restrictionClass != typeof(object);
        }

        /**
         * Return a description of known valid classes.
         * 
         * @see de.lmu.ifi.dbs.elki.utilities.optionhandling.parameters.Parameter#getValuesDescription()
         */

        public override String GetValuesDescription()
        {
            if (restrictionClass != null && restrictionClass != typeof(object))
            {
                return "";// restrictionString();
            }
            return "";
        }


        public override String GetValueAsString()
        {
            return CanonicalClassName(GetValue(), GetRestrictionClass());
        }

        /**
         * Returns a new instance for the value (i.e., the class name) of this class
         * parameter. The instance has the type of the restriction class of this class
         * parameter.
         * <p/>
         * If the Class for the class name is not found, the instantiation is tried
         * using the package of the restriction class as package of the class name.
         * 
         * @param config Parameterization to use (if Parameterizable))
         * @return a new instance for the value of this class parameter
         */
        public C InstantiateClass<C>(IParameterization config)
        {
           // try
            {
                if (GetValue() == null /* && !optionalParameter */)
                {
                    throw new UnusedParameterException("Value of parameter " + GetName() + " has not been specified.");
                }
                C instance;
               // try
                {
                    config = config.Descend(this);
                    instance = ClassGenericsUtil.TryInstantiate<C>(restrictionClass, GetValue(), config);
                }
                //catch (TargetInvocationException e)
                //{
                //    // inner exception during instantiation. Log, so we don't lose it!
                //    logger.Error(e);
                //    throw new WrongParameterValueException(this, GetValue().FullName, "Error instantiating class.", e);
                //}
                //catch (MissingMethodException )
                //{
                //    throw new WrongParameterValueException(this, GetValue().FullName, "Error instantiating class - no usable public constructor.");
                //}
                //catch (Exception e)
                //{
                //    throw new WrongParameterValueException(this, GetValue().FullName, "Error instantiating class.", e);
                //}
                return instance;
            }
            //catch (ParameterException e)
            //{
            //    config.ReportError(e);
            //    return default(C);
            //}
        }

        /**
         * Returns the restriction class of this class parameter.
         * 
         * @return the restriction class of this class parameter.
         */
        public Type GetRestrictionClass()
        {
            return restrictionClass;
        }

        /**
         * Get an iterator over all known implementations of the class restriction.
         * 
         * @return List object
         */
        public IList<Type> GetKnownImplementations()
        {
            return InspectionUtil.CachedFindAllImplementations(GetRestrictionClass());
        }

        /**
         * Provides a description string listing all classes for the given superclass
         * or interface as specified in the properties.
         * 
         * @return a description string listing all classes for the given superclass
         *         or interface as specified in the properties
         */
        public String RestrictionString()
        {
            StringBuilder info = new StringBuilder();
            if (restrictionClass.IsInterface)
            {
                info.Append("Implementing ");
            }
            else
            {
                info.Append("Extending ");
            }
            info.Append(restrictionClass.Name);
            info.Append(FormatUtil.NEWLINE);

            IList<Type> known = GetKnownImplementations();
            if (known.Count > 0)
            {
                info.Append("Known classes (default package " + restrictionClass.Assembly.FullName + "):");
                info.Append(FormatUtil.NEWLINE);
                foreach (Type c in known)
                {
                    info.Append("->" + FormatUtil.NONBREAKING_SPACE);
                    info.Append(CanonicalClassName(c, GetRestrictionClass()));
                    info.Append(FormatUtil.NEWLINE);
                }
            }
            return info.ToString();
        }

        /**
         * Get the "simple" form of a class name.
         * 
         * @param c Class
         * @param pkg Package
         * @param postfix Postfix to strip
         * 
         * @return Simplified class name
         */
        public static String CanonicalClassName(Type c, Assembly pkg, String postfix)
        {
            String name = c.Name;
            if (pkg != null)
            {
                String prefix = pkg.FullName + ".";
                if (name.StartsWith(prefix))
                {
                    name = name.Substring(prefix.Length);
                }
            }
            if (postfix != null && name.EndsWith(postfix))
            {
                name = name.Substring(0, name.Length - postfix.Length);
            }
            return name;
        }

        /**
         * Get the "simple" form of a class name.
         * 
         * @param c Class name
         * @param parent Parent/restriction class (to get package name to strip)
         * @return Simplified class name.
         */
        public static String CanonicalClassName(Type c, Type parent)
        {
            if (parent == null)
            {
                return CanonicalClassName(c, null, FACTORY_POSTFIX);
            }
            return CanonicalClassName(c, parent.Assembly, FACTORY_POSTFIX);
        }


        public override String GetDefaultValueAsString()
        {
            return CanonicalClassName(GetDefaultValue(), GetRestrictionClass());
        }
    }
}
