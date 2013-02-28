using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Log.Progress
{

    public class IndefiniteProgress : AbstractProgress
    {
        /**
         * Store completion flag.
         */
        private bool completed = false;

        /**
         * Constructor.
         * 
         * @param task Task name.
         */
        [Obsolete]
        public IndefiniteProgress(String task) :
            base(task)
        {
        }

        /**
         * Constructor with logging.
         * 
         * @param task Task name.
         * @param logger Logger to report to
         */
        public IndefiniteProgress(String task, Logging logger) :
            base(task)
        {
            logger.Progress(this);
        }

        /**
         * Serialize 'indefinite' progress.
         */

        public override StringBuilder AppendToBuffer(StringBuilder buf)
        {
            buf.Append(Task);
            buf.Append(": ");
            buf.Append(GetProcessed());
            return buf;
        }

        /**
         * Return whether the progress is complete
         * 
         * @return Completion status.
         */

        public override bool IsComplete
        {
            get { return completed; }
        }

        /**
         * Set the completion flag.
         */

        public  void SetCompleted()
        {
            this.completed = true;
        }

        /**
         * Set the completion flag and log it
         * 
         * @param logger Logger to report to.
         */
        public void SetCompleted(Logging logger)
        {
            this.completed = true;
            logger.Progress(this);
        }
    }
}
