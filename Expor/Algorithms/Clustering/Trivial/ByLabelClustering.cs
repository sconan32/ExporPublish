using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Algorithms.Clustering.Trivial
{

    [Title("Clustering by label")]
    [Description("Cluster points by a (pre-assigned!) label. For comparing results with a reference clustering.")]
    public class ByLabelClustering : AbstractAlgorithm, IClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(ByLabelClustering));

        /**
         * Flag to indicate that multiple cluster assignment is possible. If an
         * assignment to multiple clusters is desired, the labels indicating the
         * clusters need to be separated by blanks.
         */
        public static OptionDescription MULTIPLE_ID = OptionDescription.GetOrCreate("bylabelclustering.multiple", "Flag to indicate that only subspaces with large coverage " + "(i.e. the fraction of the database that is covered by the dense units) " + "are selected, the rest will be pruned.");

        /**
         * Regex to recognize noise clusters by.
         */
        public static OptionDescription NOISE_ID = OptionDescription.GetOrCreate("bylabelclustering.noise", "Regex to recognize noise classes by their label.");

        /**
         * Holds the value of {@link #MULTIPLE_ID}.
         */
        private bool multiple;

        /**
         * Holds the value of {@link #NOISE_ID}.
         */
        private Regex noisepattern = null;

        /**
         * Constructor.
         * 
         * @param multiple Allow multiple cluster assignments
         * @param noisepattern Noise pattern
         */
        public ByLabelClustering(bool multiple, Regex noisepattern)
            : base()
        {

            this.multiple = multiple;
            this.noisepattern = noisepattern;
        }

        /**
         * Constructor without parameters
         */
        public ByLabelClustering() :
            this(false, null)
        {
        }


        public override IResult Run(IDatabase database)
        {
            // Prefer a true class label
            try
            {
                IRelation relation = database.GetRelation(TypeUtil.CLASSLABEL);
                return Run(relation);
            }
            catch (NoSupportedDataTypeException )
            {
                // Otherwise, try any labellike.
                return Run(database.GetRelation(GetInputTypeRestriction()[0]));
            }
        }

        /**
         * Run the actual clustering algorithm.
         * 
         * @param relation The data input we use
         */
        public virtual ClusterList Run(IRelation relation)
        {
            IDictionary<String, IDbIds> labelMap = multiple ? MultipleAssignment(relation) : SingleAssignment(relation);

            IModifiableDbIds noiseids = DbIdUtil.NewArray();
            ClusterList result = new ClusterList("By Label Clustering", "bylabel-clustering");
            foreach (var entry in labelMap)
            {
                IDbIds ids = entry.Value;
                if (ids.Count <= 1)
                {
                    noiseids.AddDbIds(ids);
                    continue;
                }
                // Build a cluster
                Cluster c = new Cluster(entry.Key, ids, ClusterModel.CLUSTER);
                if (noisepattern != null && noisepattern.IsMatch(entry.Key))
                {
                    c.SetNoise(true);
                }
                result.AddCluster(c);
            }
            // Collected noise IDs.
            if (noiseids.Count > 0)
            {
                Cluster c = new Cluster("Noise", noiseids, ClusterModel.CLUSTER);
                c.SetNoise(true);
                result.AddCluster(c);
            }
            return result;
        }

        /**
         * Assigns the objects of the database to single clusters according to their
         * labels.
         * 
         * @param data the database storing the objects
         * @return a mapping of labels to ids
         */
        private IDictionary<String, IDbIds> SingleAssignment(IRelation data)
        {
            IDictionary<String, IDbIds> labelMap = new Dictionary<String, IDbIds>();
            foreach (IDbId id in data.GetDbIds())
            {

                Object val = data[(id)];
                String label = (val != null) ? val.ToString() : null;
                Assign(labelMap, label, id);
            }
            return labelMap;
        }

        /**
         * Assigns the objects of the database to multiple clusters according to their
         * labels.
         * 
         * @param data the database storing the objects
         * @return a mapping of labels to ids
         */
        private IDictionary<String, IDbIds> MultipleAssignment(IRelation data)
        {
            IDictionary<String, IDbIds> labelMap = new Dictionary<String, IDbIds>();

            foreach (IDbId id in data.GetDbIds())
            {
                String[] labels = data[id].ToString().Split(' ');
                foreach (String label in labels)
                {
                    Assign(labelMap, label, id);
                }
            }
            return labelMap;
        }

        /**
         * Assigns the specified id to the labelMap according to its label
         * 
         * @param labelMap the mapping of label to ids
         * @param label the label of the object to be assigned
         * @param id the id of the object to be assigned
         */
        private void Assign(IDictionary<String, IDbIds> labelMap, String label, IDbIdRef id)
        {
            if (labelMap.ContainsKey(label))
            {
                IDbIds exist = labelMap[(label)];
                if (exist is IDbId)
                {
                    IModifiableDbIds n = DbIdUtil.NewHashSet();
                    n.Add((IDbId)exist);
                    n.Add(id);
                    labelMap[label] = n;
                }
                else
                {
                    Debug.Assert(exist is IHashSetModifiableDbIds);
                    Debug.Assert(exist.Count > 1);
                    ((IModifiableDbIds)exist).Add(id);
                }
            }
            else
            {
                labelMap[label] = id.DbId;
            }
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.GUESSED_LABEL);
        }


        protected  override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {
            protected bool multiple;

            protected Regex noisepat;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                BoolParameter multipleF = new BoolParameter(MULTIPLE_ID);
                if (config.Grab(multipleF))
                {
                    multiple = multipleF.GetValue();
                }

                PatternParameter noisepatP = new PatternParameter(NOISE_ID, true);
                if (config.Grab(noisepatP))
                {
                    noisepat = noisepatP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new ByLabelClustering(multiple, noisepat);
            }

        
        }

     
    }
}
