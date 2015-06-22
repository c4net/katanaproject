// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Owin.Logging
{
    /// <summary>
    /// Provides a default ILoggerFactory.
    /// </summary>
    public static class LoggerFactory
    {
        static LoggerFactory()
        {
            Default = new ConsoleLoggerFactory();
        }

        /// <summary>
        /// Provides a default ILoggerFactory based on System.Diagnostics.TraceSorce.
        /// </summary>
        public static ILoggerFactory Default { get; set; }
    }
}
