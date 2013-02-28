using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Parameterizations
{

    public abstract class AbstractParameterization : IParameterization
    {
        // TODO: refactor "tryInstantiate" even in a higher class?

        /**
         * Errors
         */
        List<ParameterException> errors = new List<ParameterException>();

        /**
         * The logger of the class.
         */
        private readonly static Socona.Log.Logging logger = Socona.Log.Logging.GetLogger(typeof(AbstractParameterization));


        public IList<ParameterException> GetErrors()
        {
            return errors;
        }


        public bool HasErrors()
        {
            return errors.Count > 0;
        }


        public virtual void ReportError(ParameterException e)
        {
            errors.Add(e);
        }

        /**
         * Log any error that has accumulated.
         */
        public void LogAndClearReportedErrors()
        {
            foreach (ParameterException e in GetErrors())
            {
                if (logger.IsDebugging)
                {
                    logger.Warning(e.Message, e);
                }
                else
                {
                    logger.Warning(e.Message);
                }
            }
            ClearErrors();
        }

        /**
         * Clear errors.
         */
        public void ClearErrors()
        {
            // Do NOT use errors.clear(), since we might have an error report
            // referencing the collection!
            errors = new List<ParameterException>();
        }

        /**
         * Fail on errors, log any error that had occurred.
         * 
         * @throws RuntimeException if any error has occurred.
         */
        // TODO: make a multi-exception class?
        public void FailOnErrors()
        {
            int numerror = GetErrors().Count;
            if (numerror > 0)
            {
                LogAndClearReportedErrors();
                throw new ApplicationException(numerror + " errors occurred during parameterization.");
            }
        }

        /**
         * Report the internal parameterization errors to another parameterization
         * 
         * @param config Other parameterization
         */
        public void ReportInternalParameterizationErrors(IParameterization config)
        {
            int numerror = GetErrors().Count;
            if (numerror > 0)
            {
                config.ReportError(
                    new InternalParameterizationErrors(
                        numerror + " internal (re-) parameterization errors prevented execution.",
                        (ICollection<Exception>)GetErrors()));
                this.ClearErrors();
            }
        }


        public bool Grab(IParameter opt)
        {
            if (opt.IsDefined() &&logger.IsDebugging)
            {
                logger.Debug("Option " + opt.GetName() + " is already set!");
            }
            try
            {
                if (SetValueForOption(opt))
                {
                    return true;
                }
                // Try default value instead.
                if (opt.TryDefaultValue())
                {
                    return true;
                }
                // No value available.
                return false;
            }
            catch (ParameterException e)
            {
                ReportError(e);
                return false;
            }
        }

        /**
         * Perform the actual parameter assignment.
         * 
         * @param opt Option to be set
         * @return Success code (value available)
         * @throws ParameterException on assignment errors.
         */

        public abstract bool SetValueForOption(IParameter opt);

        /** Upon destruction, report any errors that weren't handled yet. */

        //public void Finalize()
        // {
        //     FailOnErrors();
        // }


        public bool CheckConstraint(IGlobalParameterConstraint constraint)
        {
            try
            {
                constraint.Test();
            }
            catch (ParameterException e)
            {
                ReportError(e);
                return false;
            }
            return true;
        }


        public C TryInstantiate<C>(Type r, Type c)
        {
            try
            {
                return ClassGenericsUtil.TryInstantiate<C>(r, c, this);
            }
            catch (Exception e)
            {
                logger.Error(e);
                ReportError(new InternalParameterizationErrors("Error instantiating internal class: "
                    + c.Name, e));
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
                logger.Error(e);
                ReportError(new InternalParameterizationErrors("Error instantiating internal class: "
                    + c.Name, e));
                return default(C);
            }
        }



        public abstract bool HasUnusedParameters();


        public abstract IParameterization Descend(object option);


    }
}
