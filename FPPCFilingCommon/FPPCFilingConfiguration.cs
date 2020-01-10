using System;
using System.Collections.Generic;
using System.IO;
using J4JSoftware.Logging;

namespace FPPCFilingCommon
{
    public class FPPCFilingConfiguration
    {
        public static string DbName = "FPPCFiling.db";

        private string _dbPath = Path.Combine( Environment.CurrentDirectory, DbName );

        public List<string> Sources { get; set; }

        public string DatabasePath
        {
            get => Path.GetFullPath( _dbPath );
            set => _dbPath = value;
        }

        public J4JLoggerConfiguration Logger { get; set; }
    }
}
