using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Results.Outliers
{
    public class OutlierResult : BasicResult
    {
        /**
         * Outlier score meta information
         */
        private IOutlierScoreMeta meta;

        /**
         * Outlier scores.
         */
        private IRelation scores;

        /**
         * Outlier ordering.
         */
        private IOrderingResult ordering;

        /**
         * Constructor.
         * 
         * @param meta Outlier score metadata.
         * @param scores Scores result.
         */
        public OutlierResult(IOutlierScoreMeta meta, IRelation scores)
            : base(scores.LongName, scores.ShortName)
        {
            this.meta = meta;
            this.scores = scores;
            this.ordering = new OrderingFromRelation(scores, !(meta is InvertedOutlierScoreMeta));
            this.AddChildResult(scores);
            this.AddChildResult(ordering);
            this.AddChildResult(meta);
        }

        /**
         * Get the outlier score meta data
         * 
         * @return the outlier meta information
         */
        public IOutlierScoreMeta GetOutlierMeta()
        {
            return meta;
        }

        /**
         * Get the outlier scores association.
         * 
         * @return the scores
         */
        public IRelation GetScores()
        {
            return scores;
        }

        /**
         * Get the outlier ordering
         * 
         * @return the ordering
         */
        public IOrderingResult GetOrdering()
        {
            return ordering;
        }
    }
}
