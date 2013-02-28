using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Algorithms
{

    [Title("Null Algorithm")]
    [Description("Algorithm which does nothing, just return a null object.")]
    public class NullAlgorithm : AbstractAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(NullAlgorithm));

        /**
         * Constructor.
         */
        public NullAlgorithm() :
            base()
        {
        }


        public override IResult Run(IDatabase database)
        {
            return null;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array();
        }
    }
}
