using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Socona.Log.Progress;

namespace Socona.Log
{
    public class Logging
    {

        private static Dictionary<string, Logging> loggers = new Dictionary<string, Logging>();

        private ILog logger;
        static Logging()
        {

        }

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


            Logging logger;
            if (!loggers.TryGetValue(name, out logger))
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
        public void Warning(object message)
        {
            logger.Warn(message);
        }
        public void Warning(object message, Exception ex)
        {
            logger.Warn(message, ex);
        }
        public void Debug(object message, Exception ex)
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
        public void Verbose(object message, Exception ex)
        {
            logger.Debug(message, ex);
        }
        public void Verbose(object message)
        {
            logger.Debug(message);
        }

        public void Progress(IProgress pgr)
        {
            logger.Info(pgr.ToString());
        }
        public static void Shutdown()
        {
            LogManager.Shutdown();
        }


    }
}
