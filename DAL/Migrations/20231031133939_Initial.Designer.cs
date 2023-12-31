﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20231031133939_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence<int>("OrderNumber")
                .StartsAt(100L)
                .IncrementsBy(333)
                .HasMin(0L)
                .HasMax(900L)
                .IsCyclic();

            modelBuilder.Entity("Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTime")
                        .IsConcurrencyToken()
                        .HasPrecision(3)
                        .HasColumnType("datetime2(3)");

                    b.Property<string>("Description")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasComputedColumnSql("[Name] + ': ala ma kota'", true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR OrderNumber");

                    b.Property<string>("OrderType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Parameters")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Timer")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasComputedColumnSql("Cast(getdate() as varchar(250))");

                    b.HasKey("Id");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Models.Person", b =>
                {
                    b.Property<int>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Key"));

                    b.Property<string>("Description")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    SqlServerPropertyBuilderExtensions.IsSparse(b.Property<string>("Description"));

                    b.Property<DateTime>("From")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasPrecision(3)
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("From");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("To")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasPrecision(3)
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("To");

                    b.HasKey("Key");

                    b.ToTable("People", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("PeopleHistory");
                                ttb
                                    .HasPeriodStart("From")
                                    .HasColumnName("From");
                                ttb
                                    .HasPeriodEnd("To")
                                    .HasColumnName("To");
                            }));
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("Models.ProductDetails", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.Property<byte[]>("_TableSharingConcurrencyTokenConvention_Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.HasOne("Models.Order", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Models.ProductDetails", b =>
                {
                    b.HasOne("Models.Product", null)
                        .WithOne("Detail")
                        .HasForeignKey("Models.ProductDetails", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Models.Order", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.Navigation("Detail");
                });
#pragma warning restore 612, 618
        }
    }
}
