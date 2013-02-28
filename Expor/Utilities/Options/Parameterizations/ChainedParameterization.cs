using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Parameterizations
{

    public class ChainedParameterization : AbstractParameterization
    {
        /**
         * Keep the list of parameterizations.
         */
        private List<IParameterization> chain = new List<IParameterization>();

        /**
         * Error tarGet
         */
        private IParameterization errorTarget;

        /**
         * Constructor that takes a number of Parameterizations to chain.
         * 
         * @param ps Parameterizations
         */
        public ChainedParameterization(params IParameterization[] ps)
        {
            errorTarget = this;
            foreach (IParameterization p in ps)
            {

                chain.Add(p);
            }
            //logger.warning("Chain length: "+chain.size()+ " for "+this);
        }

        /**
         * Append a new Parameterization to the chain.
         * 
         * @param p Parameterization
         */
        public void appendParameterization(IParameterization p)
        {
            chain.Add(p);
            //logger.warning("Chain length: "+chain.size()+ " for "+this);
        }


        public override bool SetValueForOption(IParameter opt)
        {
            foreach (IParameterization p in chain)
            {
                if (p.SetValueForOption(opt))
                {
                    return true;
                }
            }
            return false;
        }


        public override bool HasUnusedParameters()
        {
            foreach (IParameterization p in chain)
            {
                if (p.HasUnusedParameters())
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Set the error tarGet, since there is no unique way where
         * errors can be reported.
         * 
         * @param config Parameterization to report errors to
         */
        public void ErrorsTo(IParameterization config)
        {
            this.errorTarget = config;
        }

        /** {@inheritDoc} */

        public override void ReportError(ParameterException e)
        {
            if (this.errorTarget == this)
            {
                base.ReportError(e);
            }
            else
            {
                this.errorTarget.ReportError(e);
            }
        }

        /** {@inheritDoc}
         * Parallel descend in all chains.
         */

        public override IParameterization Descend(Object option)
        {
            ChainedParameterization n = new ChainedParameterization();
            n.ErrorsTo(this.errorTarget);
            foreach (IParameterization p in this.chain)
            {
                n.appendParameterization(p.Descend(option));
            }
            return n;
        }
    }
}
