using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Utilities.Options
{
    /// <summary>
    /// 参数化类的基类
    /// </summary>
    public abstract class AbstractParameterizer : IParameterizer
    {

        /**
         * Constant for "fresh" state
         */
        private static readonly int STATE_FRESH = 0;

        /**
         * Constant for "initializing" state
         */
        private static readonly int STATE_INIT = 1;

        /**
         * Constant for "complete" state
         */
        private static readonly int STATE_COMPLETE = 2;

        /**
         * Constant for "errors" state
         */
        private static readonly int STATE_ERRORS = -1;

        /**
         * Parameterization state.
         */
        private int state = STATE_FRESH;

        /**
         * Add all options.
         * 
         * <b>ALWAYS call super.makeOptions(config), unless you have a strong reason
         * to do otherwise!</b>
         * 
         * @param config Parameterization to add options to.
         */
        protected virtual void MakeOptions(IParameterization config)
        {
            // Nothing to do here.
        }

        // TODO: remove

        public void Configure(IParameterization config)
        {
            MakeOptions(config);
        }

        /**
         * Make an instance after successful configuration.
         * 
         * @return instance
         */
        abstract protected object MakeInstance();

        /**
         * Method to configure a class, then instantiate when the configuration step
         * was successful.
         * 
         * <b>Don't call this directly use unless you know what you are doing. <br />
         * Instead, use {@link Parameterization#tryInstantiate(Class)}!</b>
         * 
         * Otherwise, {@code null} will be returned, and the resulting errors can be
         * retrieved from the {@link Parameterization} parameter object. In general,
         * you should be checking the {@link Parameterization} object for errors
         * before accessing the returned value, since it may be {@code null}
         * unexpectedly otherwise.
         * 
         * @param config Parameterization
         * @return Instance or {@code null}
         */
        public Object Make(IParameterization config)
        {
            if (state != STATE_FRESH)
            {
                throw new AbortException("Parameterizers may only be set up once!");
            }
            state = STATE_INIT;

            Object owner = this.GetType();
            if (owner == null)
            {
                owner = this;
            }
            config = config.Descend(owner);
            MakeOptions(config);

            if (!config.HasErrors())
            {
                state = STATE_COMPLETE;
                Object ret = MakeInstance();
                if (ret == null)
                {
                    throw new AbortException("MakeInstance() returned null!");
                }
                return ret;
            }
            else
            {
                state = STATE_ERRORS;
                return null;
            }
        }
    }
}
