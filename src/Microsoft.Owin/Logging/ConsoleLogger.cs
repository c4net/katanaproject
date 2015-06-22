using System;
using System.Diagnostics;

namespace Microsoft.Owin.Logging
{

    public class ConsoleLogger : ILogger
    {

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            return true;
        }

    }

}
