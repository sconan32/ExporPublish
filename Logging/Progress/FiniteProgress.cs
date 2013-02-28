using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Log.Progress
{

    public class FiniteProgress : AbstractProgress
    {
        /**
         * The overall number of items to process.
         */
        private int total;

        /**
         * Holds the length of a String describing the total number.
         */
        // TODO: move this to a console logging related class instead?
        private int totalLength;

        /**
         * A progress object for a given overall number of items to process.
         * 
         * @param task the name of the task
         * @param total the overall number of items to process
         */
       // [Obsolete]
        public FiniteProgress(String task, int total) :
            base(task)
        {
            this.total = total;
            this.totalLength = total.ToString().Length;
        }

        /**
         * Constructor with auto-reporting to logging.
         * 
         * @param task the name of the task
         * @param total the overall number of items to process
         * @param logger the logger to report to
         */
        public FiniteProgress(String task, int total, Logging logger) :
            base(task)
        {
            this.total = total;
            this.totalLength = total.ToString().Length;
            logger.Progress(this);
        }

        /**
         * Sets the number of items already processed at a time being.
         * 
         * @param processed the number of items already processed at a time being
         * @throws IllegalArgumentException if the given number is negative or exceeds
         *         the overall number of items to process
         */

        public override void SetProcessed(int processed)
        {
            if (processed > total)
            {
                throw new ArgumentException(processed + " exceeds total: " + total);
            }
            if (processed < 0)
            {
                throw new ArgumentException("Negative number of processed: " + processed);
            }
            base.SetProcessed(processed);
        }

        /**
         * Append a string representation of the progress to the given string buffer.
         * 
         * @param buf Buffer to serialize to
         * @return Buffer the data was serialized to.
         */

        public override StringBuilder AppendToBuffer(StringBuilder buf)
        {
            String processedString = GetProcessed().ToString();
            int percentage = (int)(GetProcessed() * 100.0 / total);
            buf.Append(Task);
            buf.Append(": ");
            for (int i = 0; i < totalLength - processedString.Length; i++)
            {
                buf.Append(' ');
            }
            buf.Append(GetProcessed());
            buf.Append(" [");
            if (percentage < 100)
            {
                buf.Append(' ');
            }
            if (percentage < 10)
            {
                buf.Append(' ');
            }
            buf.Append(percentage);
            buf.Append("%]");
            return buf;
        }

        /**
         * Test whether the progress was completed.
         */

        public override bool IsComplete
        {
            get { return GetProcessed() == total; }
        }

        /**
         * Get the final value for the progress.
         * 
         * @return final value
         */
        public int Total
        {
            get { return total; }
        }

        /**
         * Ensure that the progress was completed, to make progress bars disappear
         * 
         * @param logger Logger to report to.
         */
        public void EnsureCompleted(Logging logger)
        {
            if (!IsComplete)
            {
                logger.Warning("Progress had not completed automatically as expected.", new Exception());
                SetProcessed(Total);
                logger.Progress(this);
            }
        }
    }
}
