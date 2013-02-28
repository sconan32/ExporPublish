using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Results;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Algorithms
{

    [Title("APRIORI: Algorithm for Mining Association Rules")]
    [Description("Searches for frequent itemsets")]
    [Reference(Authors = "R. Agrawal, R. Srikant",
        Title = "Fast Algorithms for Mining Association Rules in Large Databases",
        BookTitle = "Proc. 20th Int. Conf. on Very Large Data Bases (VLDB '94), Santiago de Chile, Chile 1994",
        Url = "http://www.acm.org/sigmod/vldb/conf/1994/P487.PDF")]
    public class APRIORI : AbstractAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(APRIORI));

        /**
         * Optional parameter to specify the threshold for minimum frequency, must be
         * a double greater than or equal to 0 and less than or equal to 1.
         * Alternatively to parameter {@link #MINSUPP_ID}).
         */
        public static OptionDescription MINFREQ_ID = OptionDescription.GetOrCreate(
            "apriori.minfreq", "Threshold for minimum frequency as percentage value " +
            "(alternatively to parameter apriori.minsupp).");

        /**
         * Parameter to specify the threshold for minimum support as minimally
         * required number of transactions, must be an integer equal to or greater
         * than 0. Alternatively to parameter {@link #MINFREQ_ID} - setting
         * {@link #MINSUPP_ID} is slightly preferable over setting {@link #MINFREQ_ID}
         * in terms of efficiency.
         */
        public static OptionDescription MINSUPP_ID = OptionDescription.GetOrCreate(
            "apriori.minsupp", "Threshold for minimum support as minimally required number of transactions " +
            "(alternatively to parameter apriori.minfreq" +
            " - setting apriori.minsupp is slightly preferable over setting " +
            "apriori.minfreq in terms of efficiency).");

        /**
         * Holds the value of {@link #MINFREQ_ID}.
         */
        private double minfreq = Double.NaN;

        /**
         * Holds the value of {@link #MINSUPP_ID}.
         */
        private int minsupp = int.MinValue;

        /**
         * Constructor with minimum frequency.
         * 
         * @param minfreq Minimum frequency
         */
        public APRIORI(double minfreq) :
            base()
        {
            this.minfreq = minfreq;
        }

        /**
         * Constructor with minimum support.
         * 
         * @param minsupp Minimum support
         */
        public APRIORI(int minsupp) :
            base()
        {
            this.minsupp = minsupp;
        }

        /**
         * Performs the APRIORI algorithm on the given database.
         * 
         * @param database the Database to run APRIORI on
         * @param relation the Relation to process
         * @return the AprioriResult learned by this APRIORI
         */
        public AprioriResult Run(IDatabase database, IRelation relation)
        {
            IDictionary<BitArray, int?> support = new Dictionary<BitArray, int?>();
            List<BitArray> solution = new List<BitArray>();
            int size = relation.Count;
            if (size > 0)
            {
                int dim;
                try
                {
                    dim = DatabaseUtil.Dimensionality(relation);
                }
                catch (InvalidOperationException )
                {
                    dim = 0;
                }
                BitArray[] candidates = new BitArray[dim];
                for (int i = 0; i < dim; i++)
                {
                    candidates[i] = new BitArray(1000);
                    candidates[i].Set(i, true);
                }
                while (candidates.Length > 0)
                {
                    StringBuilder msg = new StringBuilder();
                    BitArray[] frequentItemsets = FrequentItemsets(support, candidates, relation);
                    if (logger.IsVerbose)
                    {
                        msg.Append("\ncandidates").Append((candidates));
                        msg.Append("\nfrequentItemsets").Append((frequentItemsets));
                    }
                    foreach (BitArray bitSet in frequentItemsets)
                    {
                        solution.Add(bitSet);
                    }
                    BitArray[] joined = Join(frequentItemsets);
                    candidates = prune(support, joined, size);
                    if (logger.IsVerbose)
                    {
                        msg.Append("\ncandidates after pruning").Append((candidates));
                        logger.Verbose(msg.ToString());
                    }
                }
            }
            return new AprioriResult("APRIORI", "apriori", solution, support);
        }

        /**
         * Prunes a given set of candidates to keep only those BitSets where all
         * subsets of bits flipping one bit are frequent already.
         * 
         * @param support Support map
         * @param candidates the candidates to be pruned
         * @param size size of the database
         * @return a set of BitSets where all subsets of bits flipping one bit are
         *         frequent already
         */
        protected BitArray[] prune(IDictionary<BitArray, int?> support, BitArray[] candidates, int size)
        {
            List<BitArray> candidateList = new List<BitArray>();
            // MinFreq pruning
            if (minfreq >= 0)
            {
                foreach (BitArray bitSet in candidates)
                {
                    bool unpruned = true;
                    for (int i = bitSet.NextSetBitIndex(0); i >= 0 && unpruned; i = bitSet.NextSetBitIndex(i + 1))
                    {
                        bitSet.Set(i, false);
                        if (support[(bitSet)] != null)
                        {
                            unpruned = (double)support[(bitSet)] / size >= minfreq;
                        }
                        else
                        {
                            unpruned = false;
                            // logger.warning("Support not found for bitSet " + bitSet);
                        }
                        bitSet.Set(i, true);
                    }
                    if (unpruned)
                    {
                        candidateList.Add(bitSet);
                    }
                }
            }
            else
            {
                // Minimum support pruning
                foreach (BitArray bitSet in candidates)
                {
                    bool unpruned = true;
                    for (int i = bitSet.NextSetBitIndex(0); i >= 0 && unpruned; i = bitSet.NextSetBitIndex(i + 1))
                    {
                        bitSet.Set(i, false);
                        if (support[bitSet] != null)
                        {
                            unpruned = support[(bitSet)] >= minsupp;
                        }
                        else
                        {
                            unpruned = false;
                            // logger.warning("Support not found for bitSet " + bitSet);
                        }
                        bitSet.Set(i, true);
                    }
                    if (unpruned)
                    {
                        candidateList.Add(bitSet);
                    }
                }
            }
            return candidateList.ToArray();
        }

        /**
         * Returns a set of BitSets generated by joining pairs of given BitSets
         * (relying on the given BitSets being sorted), increasing the length by 1.
         * 
         * @param frequentItemsets the BitSets to be joined
         * @return a set of BitSets generated by joining pairs of given BitSets,
         *         increasing the length by 1
         */
        protected BitArray[] Join(BitArray[] frequentItemsets)
        {
            List<BitArray> joined = new List<BitArray>();
            for (int i = 0; i < frequentItemsets.Length; i++)
            {
                for (int j = i + 1; j < frequentItemsets.Length; j++)
                {
                    BitArray b1 = (BitArray)frequentItemsets[i].Clone();
                    BitArray b2 = (BitArray)frequentItemsets[j].Clone();
                    int b1i = b1.Length - 1;
                    int b2i = b2.Length - 1;
                    b1.Set(b1i, false);
                    b2.Set(b2i, false);
                    if (b1.Equals(b2))
                    {
                        b1.Set(b1i, true);
                        b1.Set(b2i, true);
                        joined.Add(b1);
                    }
                }
            }
            return joined.ToArray();
        }

        /**
         * Returns the frequent BitSets out of the given BitSets with respect to the
         * given database.
         * 
         * @param support Support map.
         * @param candidates the candidates to be evaluated
         * @param database the database to evaluate the candidates on
         * @return the frequent BitSets out of the given BitSets with respect to the
         *         given database
         */
        protected BitArray[] FrequentItemsets(IDictionary<BitArray, int?> support,
            BitArray[] candidates, IRelation database)
        {
            foreach (BitArray bitSet in candidates)
            {
                if (support[(bitSet)] == null)
                {
                    support[bitSet] = 0;
                }
            }
            //for(DbIdIter iditer = database.iterDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in database.GetDbIds())
            {
                BitVector bv = (BitVector)database[(iditer)];
                foreach (BitArray bitSet in candidates)
                {
                    if (bv.Contains(bitSet))
                    {
                        support[bitSet] = support[(bitSet)] + 1;
                    }
                }
            }
            List<BitArray> frequentItemsets = new List<BitArray>();
            if (minfreq >= 0.0)
            {
                // TODO: work with integers?
                double critsupp = minfreq * database.Count;
                foreach (BitArray bitSet in candidates)
                {
                    if (support[(bitSet)] >= critsupp)
                    {
                        frequentItemsets.Add(bitSet);
                    }
                }
            }
            else
            {
                // Use minimum support
                foreach (BitArray bitSet in candidates)
                {
                    if (support[(bitSet)] >= minsupp)
                    {
                        frequentItemsets.Add(bitSet);
                    }
                }
            }
            return frequentItemsets.ToArray();
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.BIT_VECTOR_FIELD);
        }


        protected override Logging GetLogger()
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
            /**
             * Parameter for minFreq
             */
            protected Double? minfreq = 0;

            /**
             * Parameter for minSupp
             */
            protected int? minsupp = 0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleParameter minfreqP = new DoubleParameter(MINFREQ_ID, true);
                minfreqP.AddConstraint(new IntervalConstraint<double>(0, IntervalConstraint<double>.IntervalBoundary.CLOSE,
                    1, IntervalConstraint<double>.IntervalBoundary.CLOSE));
                if (config.Grab(minfreqP))
                {
                    minfreq = minfreqP.GetValue();
                }

                IntParameter minsuppP = new IntParameter(MINSUPP_ID, true);
                minsuppP.AddConstraint(new GreaterEqualConstraint<int>(0));
                if (config.Grab(minsuppP))
                {
                    minsupp = minsuppP.GetValue();
                }

                // global parameter constraints
                List<IParameter> globalConstraints = new List<IParameter>();
                globalConstraints.Add(minfreqP);
                globalConstraints.Add(minsuppP);
                config.CheckConstraint(new OnlyOneIsAllowedToBeSetGlobalConstraint(globalConstraints));
                config.CheckConstraint(new OneMustBeSetGlobalConstraint(globalConstraints));
            }


            protected override object MakeInstance()
            {
                if (minfreq != null)
                {
                    return new APRIORI(minfreq.Value);
                }
                if (minsupp != null)
                {
                    return new APRIORI(minsupp.Value);
                }
                return null;
            }
        }
    }
}
