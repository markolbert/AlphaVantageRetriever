using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FPPCFilingCommon;
using Microsoft.EntityFrameworkCore.Design;

namespace AlphaVantageDb
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<FPPCFilingContext>
    {
        public FPPCFilingContext CreateDbContext( string[] args )
        {
            var config = new FPPCFilingConfiguration()
            {
                DatabasePath = Path.Combine( Environment.CurrentDirectory, FPPCFilingConfiguration.DbName )
            };

            return new FPPCFilingContext(config);
        }
    }
}
