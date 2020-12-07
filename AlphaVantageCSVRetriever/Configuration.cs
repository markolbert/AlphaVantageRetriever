using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using J4JSoftware.Logging;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class Configuration
    {
        public string OutputFilePath { get; set; }
        public string ApiKeyEncrypted { get; set; }
        public float CallsPerMinute { get; set; }
        public List<SecurityInfo> Securities { get; set; }

        public string ApiKey
        {
            get
            {
                var encoded = Convert.FromBase64String( ApiKeyEncrypted );
#pragma warning disable CA1416 // Validate platform compatibility
                var decrypted = ProtectedData.Unprotect( encoded, null, DataProtectionScope.CurrentUser );
#pragma warning restore CA1416 // Validate platform compatibility

                return Encoding.Unicode.GetString( decrypted );
            }
        }
    }
}
