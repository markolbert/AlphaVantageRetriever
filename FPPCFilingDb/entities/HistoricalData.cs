using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPPCFilingDb
{
    [EntityConfiguration( typeof( HistoricalDataConfigurator ) )]
    public class HistoricalData
    {
        public string SecurityInfoID { get; set; }
        public DateTime Timestamp { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

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
