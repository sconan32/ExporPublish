using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Utilities.Options.Parameterizations
{

    public class TrackParameters : IParameterization
    {
        /**
         * Inner parameterization
         */
        IParameterization inner;

        /**
         * Tracking storage
         */
        List<IPair<Object, IParameter>> options = new List<IPair<Object, IParameter>>();

        /**
         * Tree information: parent links
         */
        IDictionary<Object, Object> parents = new Dictionary<Object, Object>();

        /**
         * Tree information: child links
         */
        // Implementation note: we need the map to support "null" keys!
        IDictionary<Object, List<Object>> children = new Dictionary<Object, List<Object>>();

        /**
         * Current parent for nested parameterization
         */
        Object cur = null;

        /**
         * Constructor.
         * 
         * @param inner Inner parameterization to wrap.
         */
        public TrackParameters(IParameterization inner)
            : base()
        {

            this.inner = inner;
        }

        /**
         * Internal constructor, for nested tracking.
         * 
         * @param inner
         * @param option
         * @param options
         * @param parents
         * @param children
         */
        private TrackParameters(IParameterization inner,
            Object option, List<IPair<Object, IParameter>> options,
            IDictionary<Object, Object> parents, IDictionary<Object, List<Object>> children)
            : base()
        {

            this.inner = inner.Descend(option);
            this.cur = option;
            this.options = options;
            this.parents = parents;
            this.children = children;
        }


        public IList<ParameterException> GetErrors()
        {
            return inner.GetErrors();
        }


        public bool HasErrors()
        {
            return inner.HasErrors();
        }


        public bool Grab(IParameter opt)
        {
            RegisterChild(opt);
            options.Add(new Pair<Object, IParameter>(cur, (IParameter)opt));
            return inner.Grab(opt);
        }


        public bool HasUnusedParameters()
        {
            return inner.HasUnusedParameters();
        }


        public void ReportError(ParameterException e)
        {
            inner.ReportError(e);
        }


        public bool SetValueForOption(IParameter opt)
        {
            RegisterChild(opt);
            options.Add(new Pair<Object, IParameter>(cur, opt));
            return inner.SetValueForOption(opt);
        }

        /**
         * Get all seen parameters, set or unset, along with their owner objects.
         * 
         * @return Parameters seen
         */
        public ICollection<IPair<Object, IParameter>> GetAllParameters()
        {
            return options;
        }

        /**
         * Get the tracked parameters that were actually set.
         * 
         * @return Parameters given
         */
        public ICollection<Pair<OptionDescription, Object>> GetGivenParameters()
        {
            List<Pair<OptionDescription, Object>> ret = new List<Pair<OptionDescription, Object>>();
            foreach (IPair<Object, IParameter> pair in options)
            {
                if (pair.Second.IsDefined() && pair.Second.GetGivenValue() != null)
                {
                    ret.Add(new Pair<OptionDescription, Object>(pair.Second.GetOptionDescription(),
                        pair.Second.GetGivenValue()));
                }
            }
            return ret;
        }

        /** {@inheritDoc} */

        public bool CheckConstraint(IGlobalParameterConstraint constraint)
        {
            return inner.CheckConstraint(constraint);
        }

        /**
         * {@inheritDoc} Track parameters using a shared options list with parent
         * tracker.
         */

        public IParameterization Descend(Object option)
        {
            RegisterChild(option);
            return new TrackParameters(inner, option, options, parents, children);
        }

        private void RegisterChild(Object opt)
        {
            if (opt == cur)
            {
                Socona.Log.Logging.GetLogger(this.GetType()).Error("Options shouldn't have themselves as parents!", new Exception());
            }
            if (cur == null)
            {
                cur = typeof(NullKey);
            }
            parents[opt] = cur;
            List<Object> c = null;

            children.TryGetValue(cur, out c);
            if (c == null)
            {
                c = new List<Object>();
                children[cur] = c;
            }
            if (!c.Contains(opt))
            {
                c.Add(opt);
            }
            if (cur is Type && (Type)cur == typeof(NullKey))
            {
                cur = null;
            }
        }

        /**
         * Traverse the tree upwards.
         * 
         * @param pos Current object
         * @return Parent object
         */
        public Object GetParent(Object pos)
        {
            return (Type)parents[pos] == typeof(NullKey) ? null : parents[pos];
        }


        public C TryInstantiate<C>(Type r, Type c)
        {
            try
            {
                return ClassGenericsUtil.TryInstantiate<C>(r, c, this);
            }
            catch (Exception e)
            {
                ReportError(new InternalParameterizationErrors("Error instantiating internal class: " + c.Name, e));
                return default(C);
            }
        }


        public C TryInstantiate<C>(Type c)
        {
            try
            {
                return ClassGenericsUtil.TryInstantiate<C>(c, c, this);
            }
            catch (Exception e)
            {
                ReportError(new InternalParameterizationErrors("Error instantiating internal class: " + c.Name, e));
                return default(C);
            }
        }



    }
}
