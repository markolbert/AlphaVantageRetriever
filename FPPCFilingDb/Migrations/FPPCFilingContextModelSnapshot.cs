﻿// <auto-generated />
using System;
using AlphaVantageDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FPPCFilingDb.Migrations
{
    [DbContext(typeof(FPPCFilingContext))]
    partial class FPPCFilingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("FPPCFilingDb.HistoricalData", b =>
                {
                    b.Property<string>("SecurityInfoID")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<double>("Close")
                        .HasColumnType("REAL");

                    b.Property<double>("High")
                        .HasColumnType("REAL");

                    b.Property<double>("Low")
                        .HasColumnType("REAL");

                    b.Property<double>("Open")
                        .HasColumnType("REAL");

                    b.Property<int?>("SecurityInfoID1")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Volume")
                        .HasColumnType("REAL");

                    b.HasKey("SecurityInfoID", "Timestamp");

                    b.HasIndex("SecurityInfoID1");

                    b.ToTable("HistoricalData");
                });

            modelBuilder.Entity("FPPCFilingDb.SecurityInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<string>("Issuer")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Reportable")
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

            modelBuilder.Entity("FPPCFilingDb.HistoricalData", b =>
                {
                    b.HasOne("FPPCFilingDb.SecurityInfo", "SecurityInfo")
                        .WithMany("HistoricalData")
                        .HasForeignKey("SecurityInfoID1");
                });
#pragma warning restore 612, 618
        }
    }
}
