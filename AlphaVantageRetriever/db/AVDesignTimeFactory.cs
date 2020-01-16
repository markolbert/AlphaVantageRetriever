using System;
using System.IO;
using J4JSoftware.EFCoreUtilities;
using Microsoft.EntityFrameworkCore.Design;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class AVDesignTimeFactory : DesignTimeFactory<AlphaVantageContext>
    {
        protected override IDbContextFactoryConfiguration GetDatabaseConfiguration()
        {
            return new AppConfiguration()
            {
                DatabasePath = Path.Combine( Environment.CurrentDirectory, AppConfiguration.DbName )
            };
        }
    }
}
