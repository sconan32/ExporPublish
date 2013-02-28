using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results;

namespace Socona.Expor.Results
{

    public abstract class AbstractHierarchicalResult : IHierarchicalResult
    {
        /**
         * The hierarchy storage.
         */
        private ResultHierarchy hierarchy;

        /**
         * Constructor.
         */
        public AbstractHierarchicalResult()
            : base()
        {

            this.hierarchy = new ResultHierarchy();
        }

        public ResultHierarchy Hierarchy
        {
            get { return hierarchy; }
            set { hierarchy = value; }
        }
       

        /**
         * Add a child result.
         * 
         * @param child Child result
         */
        public void AddChildResult(IResult child)
        {
            hierarchy.Add(this, child);
        }

        public abstract string LongName
        {
            get;
        }

        public abstract  string ShortName
        {
            get;
        }
    }
}