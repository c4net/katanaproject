// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Security.Cryptography;

namespace Microsoft.Owin.Security.DataProtection
{
    internal class DpapiDataProtector : IDataProtector
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "appName"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "purposes")]
        public DpapiDataProtector(string appName, string[] purposes)
        {
            
        }

        public byte[] Protect(byte[] userData)
        {
            return userData;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
