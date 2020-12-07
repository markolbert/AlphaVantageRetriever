using System;
using J4JSoftware.EFCoreUtilities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever
{
    public class AlphaVantageContext : DbContext
    {
        private readonly IDbContextFactoryConfiguration _config;

        public AlphaVantageContext( IDbContextFactoryConfiguration config )
        {
            _config = config ?? throw new NullReferenceException( nameof(config) );
        }

        public DbSet<SecurityInfo> Securities { get; set; }
        public DbSet<HistoricalData> HistoricalData { get; set; }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            base.OnConfiguring( optionsBuilder );

            // we open the connection, and use the opened connection to initialize the entity
            // framework via optionsBuilder, to preserve the UDF configuration
            var connection = new SqliteConnection( $"DataSource={_config.DatabasePath}" );
            //var connection = new SqliteConnection($"DataSource=:memory:");
            connection.Open();

            optionsBuilder.UseSqlite( connection );
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );

            modelBuilder.ConfigureEntities( this.GetType().Assembly );
        }
    }
}
