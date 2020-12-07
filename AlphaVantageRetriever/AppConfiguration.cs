using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using J4JSoftware.EFCoreUtilities;
using J4JSoftware.Logging;
using Microsoft.EntityFrameworkCore.Design;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever
{
    public class AppConfiguration : IDbContextFactoryConfiguration
    {
        public static string DbName = "AlphaVantage.db";

        private string _dbPath = Path.Combine( Environment.CurrentDirectory, DbName );

        public string ApiKeyEncrypted { get; set; }
        public string PathToSecuritiesFile { get; set; }
        public string PathToPriceFile { get; set; }
        public int ReportingYear { get; set; }
        public float CallsPerMinute { get; set; }

        public string DatabasePath
        {
            get => Path.GetFullPath( _dbPath );
            set => _dbPath = value;
        }

        public string ApiKey
        {
            get
            {
                var encoded = Convert.FromBase64String(ApiKeyEncrypted);
#pragma warning disable CA1416 // Validate platform compatibility
                var decrypted = ProtectedData.Unprotect(encoded, null, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 // Validate platform compatibility

                return Encoding.Unicode.GetString(decrypted);
            }
        }
    }
}
