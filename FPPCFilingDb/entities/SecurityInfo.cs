using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace J4JSoftware.FppcFiling
{
    [EntityConfiguration( typeof( SecurityInfoConfigurator ) )]
    public class SecurityInfo
    {
        public int ID { get; set; }
        public string Issuer { get; set; }
        public string Ticker { get; set; }
        public string Category { get; set; }
        public bool Reportable { get; set; }
        public string ErrorMessage { get; set; }
        public bool RetrievedData { get; set; }

        public List<HistoricalData> HistoricalData { get; set; }
    }

    internal class SecurityInfoConfigurator : EntityConfigurator<SecurityInfo>
    {
        protected override void Configure( EntityTypeBuilder<SecurityInfo> builder )
        {
            builder.HasKey( x => x.ID );

            builder.HasIndex( x => x.Issuer )
                .IsUnique();

            builder.HasIndex( x => x.Ticker )
                .IsUnique();
        }
    }
}
