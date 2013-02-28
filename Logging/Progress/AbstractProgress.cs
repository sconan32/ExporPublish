using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Socona.Log.Progress
{

    public abstract class AbstractProgress : IProgress
    {
        /**
         * The number of items already processed at a time being.
         * 
         * We use AtomicInteger to allow threaded use without synchronization.
         */
        private AtomicInteger processed = new AtomicInteger(0);

        /**
         * The task name.
         */
        private String task;

        /**
         * For logging rate control.
         */
        private long lastLogged = long.MinValue;

        /**
         * Default constructor.
         * 
         * @param task Task name.
         */
        public AbstractProgress(String task)
        {

            this.task = task;
        }

        /**
         * Provides the name of the task.
         * 
         * @return the name of the task
         */
        public String Task
        {
            get { return task; }
        }

        public abstract bool IsComplete { get; }
        /**
         * Sets the number of items already processed at a time being.
         * 
         * @param processed the number of items already processed at a time being
         * @throws IllegalArgumentException if an invalid value was passed.
         */
        public virtual void SetProcessed(int processed)
        {
            this.processed.Set(processed);
        }

        /**
         * Sets the number of items already processed at a time being.
         * 
         * @param processed the number of items already processed at a time being
         * @param logger Logger to report to
         * @throws IllegalArgumentException if an invalid value was passed.
         */
        public void SetProcessed(int processed, Logging logger)
        {
            SetProcessed(processed);
            if (TestLoggingRate())
            {
                logger.Progress(this);
            }
        }

        /**
         * Get the number of items already processed at a time being.
         * 
         * @return number of processed items
         */
        public int GetProcessed()
        {
            return processed.Get();
        }

        /**
         * Serialize a description into a String buffer.
         * 
         * @param buf Buffer to serialize to
         * @return Buffer the data was serialized to.
         */

        public abstract StringBuilder AppendToBuffer(StringBuilder buf);

        /**
         * Returns a String representation of the progress suitable as a message for
         * printing to the command line interface.
         * 
         * @see java.lang.Object#toString()
         */

        public override String ToString()
        {
            StringBuilder message = new StringBuilder();
            AppendToBuffer(message);
            return message.ToString();
        }

        /**
         * Increment the processed counter.
         */
        public void IncrementProcessed()
        {
            this.processed.IncrementAndGet();
        }

        /**
         * Increment the processed counter.
         * 
         * @param logger Logger to report to.
         */
        public void IncrementProcessed(Logging logger)
        {
            IncrementProcessed();
            if (TestLoggingRate())
            {
                logger.Progress(this);
            }
        }

        /**
         * Logging rate control.
         * 
         * @return true when logging is sensible
         */
        protected bool TestLoggingRate()
        {
            if (IsComplete || GetProcessed() < 10)
            {
                return true;
            }
            long now = DateTime.Now.Ticks;
            if (lastLogged > now - 1E8)
            {
                return false;
            }
            lastLogged = now;
            return true;
        }
    }
}
