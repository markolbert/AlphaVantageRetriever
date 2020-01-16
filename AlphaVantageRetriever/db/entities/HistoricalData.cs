using System;
using J4JSoftware.EFCoreUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace J4JSoftware.AlphaVantageRetriever
{
    [EntityConfiguration( typeof( HistoricalDataConfigurator ) )]
    public class HistoricalData
    {
        public int SecurityInfoID { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }

        public SecurityInfo SecurityInfo { get; set; }
    }

    internal class HistoricalDataConfigurator : EntityConfigurator<HistoricalData>
    {
        protected override void Configure( EntityTypeBuilder<HistoricalData> builder )
        {
            builder.HasKey( x => new { SymbolID = x.SecurityInfoID, x.Timestamp } );

            builder.HasOne<SecurityInfo>( x => x.SecurityInfo )
                .WithMany( x => x.HistoricalData );
        }
    }
}
