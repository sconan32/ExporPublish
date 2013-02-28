using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Utilities.Options.Parameterizations
{

    public class SerializedParameterization : AbstractParameterization
    {
        /**
         * Prefix of option markers on the command line.
         * <p/>
         * The option markers are supposed to be given on the command line with
         * leading -.
         */
        public static String OPTION_PREFIX = "-";

        /**
         * Parameter storage
         */
        List<String> parameters = null;

        /**
         * Constructor
         */
        public SerializedParameterization()
            : base()
        {
            parameters = new List<String>();
        }

        /**
         * Constructor
         * 
         * @param args Parameters 
         */
        public SerializedParameterization(String[] args)
            : this()
        {
            foreach (String arg in args)
            {
                parameters.Add(arg);
            }
        }

        /**
         * Constructor
         * 
         * @param args Parameter list 
         */
        public SerializedParameterization(IList<String> args) :
            this()
        {
            parameters.AddRange(args);
        }

        /**
         * Return the yet unused parameters.
         * 
         * @return Unused parameters.
         */
        public IList<String> GetRemainingParameters()
        {
            return parameters;
        }


        public override  bool HasUnusedParameters()
        {
            return (parameters.Count > 0);
        }

        /**
         * Log a warning if there were unused parameters.
         */
        public void LogUnusedParameters()
        {
            if (HasUnusedParameters())
            {
                Socona.Log.Logging.GetLogger(this.GetType()).Warning("The following parameters were not processed: " +FormatUtil.Format(parameters));
            }
        }


        public override bool SetValueForOption(IParameter opt)
        {
            for (int ii = 0; ii < parameters.Count; ii++)
            {
                string cur = parameters[ii];


                if (!cur.StartsWith(OPTION_PREFIX))
                {
                    continue;
                    // throw new NoParameterValueException(cur + " is no parameter!");
                }


                // Get the option without the option prefix -
                String noPrefixOption = cur.Substring(OPTION_PREFIX.Length);

                if (opt.GetName().Equals(noPrefixOption))
                {
                    // Consume.
                    parameters.RemoveAt(ii);
                    // check if the option is a parameter or a flag
                    if (opt is BoolParameter)
                    {
                        String set = BoolParameter.SET;
                        // The next element must be a parameter
                        if (ii + 1 < parameters.Count)
                        {
                            String next = parameters[ii + 1];
                            if (BoolParameter.SET.Equals(next))
                            {
                                set = BoolParameter.SET;
                                parameters.RemoveAt(ii);
                            }
                            else if (BoolParameter.NOT_SET.Equals(next))
                            {
                                set = BoolParameter.NOT_SET;
                                parameters.RemoveAt(ii);
                            }
                            else if (!next.StartsWith(OPTION_PREFIX))
                            {
                                throw new NoParameterValueException(
                                    "Flag " + opt.GetName() + " requires no parameter-value! " +
                                    "(read parameter-value: " + next + ")");
                            }
                            // We do not consume the next if it's not for us ...
                        }
                        // set the Flag
                        opt.SetValue(set);
                        return true;
                    }
                    else
                    {
                        // Ensure there is a potential value for this parameter
                        if (ii + 1 > parameters.Count)
                        {
                            throw new NoParameterValueException(
                                "Parameter " + opt.GetName() + " requires a parameter value!");
                        }
                        opt.SetValue(parameters[ii]);
                        // Consume parameter
                        parameters.RemoveAt(ii);
                        // Success - return.
                        return true;
                    }
                }
            }
            return false;
        }

        /** {@inheritDoc}
         * Default implementation, for flat parameterizations. 
         */

        public override IParameterization Descend(Object option)
        {
            return this;
        }

      
    }
}
