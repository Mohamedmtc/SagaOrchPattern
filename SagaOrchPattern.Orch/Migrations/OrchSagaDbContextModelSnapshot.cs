﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SagaOrchPattern.DB;

namespace SagaOrchPattern.Orch.Migrations
{
    [DbContext(typeof(OrchSagaDbContext))]
    partial class OrchSagaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SagaOrchPattern.DB.OrderStateData", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CurrentState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("OrderCancelDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("OrderCreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("OrderFinishedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PaymentCardNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CorrelationId");

                    b.ToTable("OrderStateData");
                });
#pragma warning restore 612, 618
        }
    }
}
