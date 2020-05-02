using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using J4JSoftware.EFCoreUtilities;
using J4JSoftware.Logging;
using Microsoft.EntityFrameworkCore.Design;

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
                if (string.IsNullOrEmpty(ApiKeyEncrypted))
                    return null;

                var encoded = Convert.FromBase64String(ApiKeyEncrypted);
                var decrypted = ProtectedData.Unprotect(encoded, null, DataProtectionScope.CurrentUser);

                return Encoding.Unicode.GetString(decrypted);
            }
        }
    }
}
