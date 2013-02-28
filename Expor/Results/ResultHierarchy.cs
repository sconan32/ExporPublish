using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Log;
using Socona.Expor.Utilities.DataStructures.Hierarchy;
using Socona.Expor.Results;

namespace Socona.Expor.Results
{

    /**
     * Class to store a hierarchy of result objects.
     * 
     * @author Erich Schubert
     */
    // TODO: add listener merging!
    public class ResultHierarchy : HierarchyDictList<IResult>
    {
        /**
         * Logger
         */
        private static Logging logger = Logging.GetLogger(typeof(ResultHierarchy));

        /**
         * Holds the listener.
         */
        // private EventListenerList listenerList = new EventListenerList();
        public event ResultEventHandler ResultAdded;
        public event ResultEventHandler ResultRemoved;
        public event ResultEventHandler ResultChanged;

        /**
         * Constructor.
         */
        public ResultHierarchy()
            : base()
        {

        }


        public override void Add(IResult parent, IResult child)
        {
            base.Add(parent, child);
            if (child is IHierarchicalResult)
            {
                IHierarchicalResult hr = (IHierarchicalResult)child;
                IModifiableHierarchy<IResult> h = hr.Hierarchy;
                // Merge hierarchy
                hr.Hierarchy = this;
                // Add children of child
                foreach (IResult desc in h.GetChildren(hr))
                {
                    this.Add(hr, desc);
                    if (desc is IHierarchicalResult)
                    {
                        ((IHierarchicalResult)desc).Hierarchy = this;
                    }
                }
            }
            fireResultAdded(child, parent);
        }


        public override void Remove(IResult parent, IResult child)
        {
            base.Remove(parent, child);
            fireResultRemoved(child, parent);
        }


        public override void Put(IResult obj, List<IResult> parents, List<IResult> children)
        {
            // TODO: can we support this somehow? Or reduce visibility?
            throw new InvalidOperationException();
        }



        /**
         * Signal that a result has changed (public API)
         * 
         * @param res Result that has changed.
         */
        public void resultChanged(IResult res)
        {
            fireResultChanged(res);
        }

        /**
         * Informs all registered {@link ResultListener} that a new result was added.
         * 
         * @param child New child result added
         * @param parent Parent result that was added to
         */
        private void fireResultAdded(IResult child, IResult parent)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Result added: " + child + " <- " + parent);
            }
            if (ResultAdded != null)
            {
                ResultAdded(this, new ResultEventArgs() { Child = child, Parent = parent });
            }
        }

        /**
         * Informs all registered {@link ResultListener} that a result has changed.
         * 
         * @param current Result that has changed
         */
        private void fireResultChanged(IResult current)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Result changed: " + current);
            }
            if (ResultChanged != null)
            {

                ResultChanged(this, new ResultEventArgs() { Current = current });
            }
        }

        /**
         * Informs all registered {@link ResultListener} that a new result has been
         * removed.
         * 
         * @param child result that has been removed
         * @param parent Parent result that has been removed
         */
        private void fireResultRemoved(IResult child, IResult parent)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Result removed: " + child + " <- " + parent);
            }
            if (ResultRemoved != null)
            {
                ResultRemoved(this, new ResultEventArgs() { Child = child, Parent = parent });
            }
        }
    }

}

