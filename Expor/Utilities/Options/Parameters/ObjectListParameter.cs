using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Utilities.Options.Parameters
{
    public class ObjectListParameter<C> : ClassListParameter<C>
    {
        /**
         * Cache for the generated instances.
         */
        private List<C> instances = null;

        /**
         * Constructor with optional flag.
         * 
         * @param optionID Option ID
         * @param restrictionClass Restriction class
         * @param optional optional flag
         */
        public ObjectListParameter(OptionDescription optionID, Type restrictionClass, bool optional) :
            base(optionID, restrictionClass, optional)
        {
        }

        /**
         * Constructor for non-optional.
         * 
         * @param optionID Option ID
         * @param restrictionClass Restriction class
         */
        public ObjectListParameter(OptionDescription optionID, Type restrictionClass) :
            base(optionID, restrictionClass)
        {
        }

        /** {@inheritDoc} */

        public override String GetSyntax()
        {
            return "<object_1|class_1,...,object_n|class_n>";
        }

        /** {@inheritDoc} */


        protected override IList<Type> ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter Error.\n" + "No value for parameter \"" + this.GetName() + "\" " + "given.");
            }
            if (obj is IList<Type>)
            {
                IList<Type> l = (IList<Type>)obj;
                List<C> inst = new List<C>(l.Count);
                List<Type> classes = new List<Type>(l.Count);
                foreach (Object o in l)
                {
                    // does the given objects class fit?
                    if (restrictionClass.IsInstanceOfType(o))
                    {
                        inst.Add((C)o);
                        classes.Add((Type)o.GetType());
                    }
                    else if (o is C)
                    {
                        if (restrictionClass.IsAssignableFrom((Type)o))
                        {
                            inst.Add(default(C));
                            classes.Add((Type)o);
                        }
                        else
                        {
                            throw new WrongParameterValueException(this, ((Type)o).Name, "Given class not a subclass / implementation of " + restrictionClass.Name, null);
                        }
                    }
                    else
                    {
                        throw new WrongParameterValueException(this, o.GetType().Name, "Given instance not an implementation of " + restrictionClass.Name, null);
                    }
                }
                this.instances = inst;
                return base.ParseValue(classes);
            }
            // Did we get a single instance?
            try
            {
                C inst = (C)obj;
                this.instances = new List<C>(1);
                this.instances.Add(inst);
                return base.ParseValue(inst.GetType());
            }
            catch (Exception)
            {
                // Continue
            }
            return base.ParseValue(obj);
        }

        /** {@inheritDoc} */

        public override IList<C> InstantiateClasses(IParameterization config)
        {
            if (instances == null)
            {
                // instantiateClasses will descend itself.
                instances = new List<C>(base.InstantiateClasses(config));
            }
            else
            {
                IParameterization cfg = null;
                for (int i = 0; i < instances.Count; i++)
                {
                    if (instances[(i)] == null)
                    {
                        Type cls = GetValue()[i];
                        try
                        {
                            // Descend at most once, and only when needed
                            if (cfg == null)
                            {
                                cfg = config.Descend(this);
                            }
                            C instance = ClassGenericsUtil.TryInstantiate<C>(restrictionClass, cls, cfg);
                            instances[i] = instance;
                        }
                        catch (Exception e)
                        {
                            config.ReportError(new WrongParameterValueException(this, cls.Name, e));
                        }
                    }
                }
            }
            return new List<C>(instances);
        }
    }
}
