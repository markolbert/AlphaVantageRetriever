using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;

namespace J4JSoftware.FppcFiling
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<FppcFilingContext>
    {
        public FppcFilingContext CreateDbContext( string[] args )
        {
            var config = new FppcFilingConfiguration()
            {
                DatabasePath = Path.Combine( Environment.CurrentDirectory, FppcFilingConfiguration.DbName )
            };

            return new FppcFilingContext(config);
        }
    }
}
