﻿// <auto-generated />
using System;
using J4JSoftware.FppcFiling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace J4JSoftware.FppcFiling.Migrations
{
    [DbContext(typeof(FppcFilingContext))]
    partial class FppcFilingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("J4JSoftware.FppcFiling.HistoricalData", b =>
                {
                    b.Property<int>("SecurityInfoID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Close")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("High")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Low")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Open")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Volume")
                        .HasColumnType("TEXT");

                    b.HasKey("SecurityInfoID", "Timestamp");

                    b.ToTable("HistoricalData");
                });

            modelBuilder.Entity("J4JSoftware.FppcFiling.SecurityInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("Issuer")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Reportable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("RetrievedData")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ticker")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Issuer")
                        .IsUnique();

                    b.HasIndex("Ticker")
                        .IsUnique();

                    b.ToTable("Securities");
                });

            modelBuilder.Entity("J4JSoftware.FppcFiling.HistoricalData", b =>
                {
                    b.HasOne("J4JSoftware.FppcFiling.SecurityInfo", "SecurityInfo")
                        .WithMany("HistoricalData")
                        .HasForeignKey("SecurityInfoID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
