// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ReSharper
{
    using System;

    using Catel.Logging;
    using JetBrains.Util;
    using LogEvent = Logging.LogEvent;
    using JetBrains.Util.Logging;

    /// <summary>
    /// The logger listener.
    /// </summary>
    public class LoggerListener : LogListenerBase
    {
        #region Constructors and Destructors
        public LoggerListener()
        {
#if DEBUG && !R9X && !R10X && !R2017X
            Logger.AppendListener(new DebugOutputLogEventListener("CatelR#"));
#endif
        }

        #endregion

        #region Public Methods and Operators
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            Logger.LogMessage(message);
        }

        #endregion
    }
}