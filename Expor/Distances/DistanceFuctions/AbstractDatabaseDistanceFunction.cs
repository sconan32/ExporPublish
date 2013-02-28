using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public abstract class AbstractDatabaseDistanceFunction<O> : IDistanceFunction
    {
        /**
         * Constructor, supporting
         * {@link de.lmu.ifi.dbs.elki.utilities.optionhandling.Parameterizable} style
         * classes.
         */
        public AbstractDatabaseDistanceFunction()
        {

        }

        public abstract IDistanceQuery Instantiate(IRelation relation) ;
        abstract public IDistanceValue DistanceFactory { get; }
        public abstract ITypeInformation GetInputTypeRestriction();

        public virtual bool IsMetric
        {
            get { return false; }
        }


        public virtual bool IsSymmetric
        {
            get { return true; }
        }

        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         */
        abstract public class Instance : AbstractDatabaseDistanceQuery<O>
        {
            /**
             * Parent distance
             */
            IDistanceFunction parent;

            /**
             * Constructor.
             * 
             * @param database Database
             * @param parent Parent distance
             */
            public Instance(IRelation database, IDistanceFunction parent) :
                base(database)
            {
                this.parent = parent;
            }


            public override IDistanceFunction DistanceFunction
            {
                get { return parent; }
            }
        }
    }
}
