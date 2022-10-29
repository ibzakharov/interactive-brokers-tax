﻿// <auto-generated />
using System;
using Investing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Investing.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("Investing.Data.Models.Currency", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("TaxCode")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Investing.Data.Models.ExchangeRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrencyId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("Investing.Data.Models.ExchangeRate", b =>
                {
                    b.HasOne("Investing.Data.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId");

                    b.Navigation("Currency");
                });
#pragma warning restore 612, 618
        }
    }
}
