using System;
using System.IO;
using J4JSoftware.Logging;

namespace J4JSoftware.FppcFiling
{
    public class FppcFilingConfiguration
    {
        public static string DbName = "FppcFiling.db";
        public static string BackupDbName = "FppcFiling.backup.db";

        private string _dbPath = Path.Combine( Environment.CurrentDirectory, DbName );
        private string _dbBackupPath = Path.Combine( Environment.CurrentDirectory, BackupDbName );

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

        public string BackupDatabasePath
        {
            get => Path.GetFullPath( _dbBackupPath );
            set => _dbBackupPath = value;
        }

        public J4JLoggerConfiguration Logger { get; set; }
    }
}
