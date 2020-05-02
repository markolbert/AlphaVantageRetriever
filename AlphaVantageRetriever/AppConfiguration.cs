using System;
using System.IO;
using J4JSoftware.EFCoreUtilities;
using J4JSoftware.Logging;
using Microsoft.EntityFrameworkCore.Design;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class AppConfiguration : IDbContextFactoryConfiguration
    {
        public static string DbName = "AlphaVantage.db";

        private string _dbPath = Path.Combine( Environment.CurrentDirectory, DbName );

        public string ApiKey { get; set; }
        public string PathToSecuritiesFile { get; set; }
        public string PathToPriceFile { get; set; }
        public int ReportingYear { get; set; }
        public float CallsPerMinute { get; set; }

        public string DatabasePath
        {
            get => Path.GetFullPath( _dbPath );
            set => _dbPath = value;
        }

        //public J4JLoggerConfiguration Logger { get; set; }
    }
}
