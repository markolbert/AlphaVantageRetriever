using System;
using System.Collections.Generic;
using System.IO;
using J4JSoftware.Logging;

namespace J4JSoftware.FppcFiling
{
    public class FppcFilingConfiguration
    {
        public static string DbName = "FppcFiling.db";

        private string _dbPath = Path.Combine( Environment.CurrentDirectory, DbName );

        public string ApiKey { get; set; }
        public string PathToSecuritiesFile { get; set; }
        //public List<string> Sources { get; set; }

        public string DatabasePath
        {
            get => Path.GetFullPath( _dbPath );
            set => _dbPath = value;
        }

        public J4JLoggerConfiguration Logger { get; set; }
    }
}
