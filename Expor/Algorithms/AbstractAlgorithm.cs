using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;


namespace Socona.Expor.Algorithms
{
    public abstract class AbstractAlgorithm : IAlgorithm
    {
        public AbstractAlgorithm()
        {
            //  Arguments = new Arguments();
            // Results = new Results();
        }

        public virtual IResult Run(IDatabase database)
        {
            //Clusterize();
            Object[] relations1;
            Type[] signature1;
            Object[] relations2;
            Type[] signature2;
            // Build candidate method signatures
            {
                ITypeInformation[] inputs = GetInputTypeRestriction();
                relations1 = new Object[inputs.Length + 1];
                signature1 = new Type[inputs.Length + 1];
                relations2 = new Object[inputs.Length];
                signature2 = new Type[inputs.Length];
                // First parameter is the database
                relations1[0] = database;
                signature1[0] = typeof(IDatabase);
                // Other parameters are the bound relations
                for (int i = 0; i < inputs.Length; i++)
                {
                    // TODO: don't bind the same relation twice?
                    // But sometimes this is wanted (e.g. using projected distances)
                    relations1[i + 1] = database.GetRelation(inputs[i]);
                    signature1[i + 1] = typeof(IRelation);
                    relations2[i] = database.GetRelation(inputs[i]);
                    signature2[i] = typeof(IRelation);
                }
            }

            // Find appropriate Run method.
            MethodInfo runmethod1 = null;
            MethodInfo runmethod2 = null;
            try
            {
                runmethod1 = this.GetType().GetMethod("Run", signature1);
                runmethod2 = null;
            }
            catch (Exception e)
            {
                throw new APIViolationException("Security exception finding an appropriate 'Run' method.", e);
            }
            if (runmethod1 == null)
            {
                // Try without "database" parameter.
                try
                {
                    runmethod2 = this.GetType().GetMethod("Run", signature2);
                }

                catch (Exception e2)
                {
                    throw new APIViolationException("Security exception finding an appropriate 'Run' method.", e2);
                }
            }

            if (runmethod1 != null)
            {
                try
                {
                    StringBuilder buf = new StringBuilder();
                    foreach (Type cls in signature1)
                    {
                        buf.Append(cls.ToString()).Append(",");
                    }
                    return (IResult)runmethod1.Invoke(this, relations1);
                }
                catch (ArgumentException e)
                {
                    throw new APIViolationException("Invoking the real 'Run' method failed.", e);
                }
                catch (MethodAccessException e)
                {
                    throw new APIViolationException("Invoking the real 'Run' method failed.", e);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is ApplicationException)
                    {
                        throw e.InnerException;
                    }

                    throw new APIViolationException("Invoking the real 'Run' method failed: " + e.InnerException.ToString(), e.InnerException);
                }
            }
            else if (runmethod2 != null)
            {
                try
                {
                    StringBuilder buf = new StringBuilder();
                    foreach (Type cls in signature1)
                    {
                        buf.Append(cls.ToString()).Append(",");
                    }
                    return (IResult)runmethod2.Invoke(this, relations2);
                }
                catch (ArgumentException e)
                {
                    throw new APIViolationException("Invoking the real 'Run' method failed.", e);
                }
                catch (MethodAccessException e)
                {
                    throw new APIViolationException("Invoking the real 'Run' method failed.", e);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is ApplicationException)
                    {
                        throw e.InnerException;
                    }
                    throw new APIViolationException("Invoking the real 'Run' method failed: " + e.InnerException.ToString(), e.InnerException);
                }
            }
            else
            {
                throw new APIViolationException("No appropriate 'Run' method found.");
            }
        }
        protected abstract Logging GetLogger();
        public abstract ITypeInformation[] GetInputTypeRestriction();
        public static ObjectParameter<F> MakeParameterDistanceFunction<F>(Type defaultDistanceFunction, Type restriction)
        {

            return new ObjectParameter<F>(AbstractDistanceBasedAlgorithm<F>.DISTANCE_FUNCTION_ID, restriction, defaultDistanceFunction);

        }
    }
}


