﻿// <auto-generated />
using System;
using CardCollectiveBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CardCollectiveBot.Data.Migrations
{
    [DbContext(typeof(CardCollectiveBotContext))]
    [Migration("20210628124954_initialCreate")]
    partial class initialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CardCollectiveBot.Data.Entities.Currency", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<int>("Mangoes")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Currency");
                });
#pragma warning restore 612, 618
        }
    }
}