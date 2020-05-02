using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class Configuration
    {
        public string OutputFilePath { get; set; }
        public string ApiKeyEncrypted { get; set; }
        public float CallsPerMinute { get; set; }
        public List<string> Tickers { get; set; }

        public string ApiKey
        {
            get
            {
                if( string.IsNullOrEmpty( ApiKeyEncrypted ) )
                    return null;

                var encoded = Convert.FromBase64String( ApiKeyEncrypted );
                var decrypted = ProtectedData.Unprotect( encoded, null, DataProtectionScope.CurrentUser );

                return Encoding.Unicode.GetString( decrypted );
            }
        }
    }
}
