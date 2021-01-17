using System.Collections.Generic;
using J4JSoftware.EFCoreUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever
{
    [EntityConfiguration( typeof( SecurityInfoConfigurator ) )]
    public class SecurityInfo
    {
        public int ID { get; set; }
        public string Cusip { get; set; }
        public string SecurityName { get; set; }
        public string Type { get; set; }
        public string Issuer { get; set; }
        public string? ClassSeries { get; set; }
        public bool Reportable { get; set; }
        public string? Ticker { get; set; }
        public string? ErrorMessage { get; set; }
        public bool RetrievedData { get; set; }

        public List<HistoricalData> HistoricalData { get; set; }
    }

    internal class SecurityInfoConfigurator : EntityConfigurator<SecurityInfo>
    {
        protected override void Configure( EntityTypeBuilder<SecurityInfo> builder )
        {
            builder.HasKey( x => x.ID );

            builder.HasIndex( x => x.Cusip )
                .IsUnique();
        }
    }
}
