using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
namespace Socona.Log
{
    public class Logging
    {

        private static Dictionary<string, Logging> loggers = new Dictionary<string, Logging>();

        private ILog logger;


        public Logging(ILog logger)
        {
            this.logger = logger;
        }

        public static Logging GetLogger(Type type)
        {
            return GetLogger(type.ToString());
        }
        public static Logging GetLogger(string name)
        {
            Logging logger = loggers[name];
            if (loggers == null)
            {
                logger = new Logging(LogManager.GetLogger(name));
                loggers[name] = logger;
            }
            return logger;
        }
        public bool IsVerbose
        {
            get { return logger.IsInfoEnabled; }
        }
        public bool IsInfo
        {
            get { return logger.IsInfoEnabled; }
        }
        public bool IsDebugging
        {
            get { return logger.IsDebugEnabled; }
        }
        public void Debug(object message,Exception ex)
        {
            logger.Debug(message, ex);
        }
        public void Debug(object message)
        {
            logger.Debug(message);
        }
        public void Error(object message, Exception ex)
        {
            logger.Error(message, ex);
        }
        public void Error(object message)
        {
            logger.Error(message);
        }

    }
}
