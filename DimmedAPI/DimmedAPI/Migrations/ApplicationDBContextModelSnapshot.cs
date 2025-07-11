﻿// <auto-generated />
using System;
using DimmedAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DimmedAPI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DimmedAPI.Entidades.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Guid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsProcessResponsable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.Companies", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BusinessName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdentificationNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdentificationTypeId")
                        .HasColumnType("int");

                    b.Property<string>("LegalRepresentative")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MainAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<int?>("ModifiedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("SqlConnectionString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TradeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("IdentificationTypeId");

                    b.HasIndex("ModifiedByUserId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.IdentificationTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AppUserId")
                        .HasColumnType("int");

                    b.Property<int?>("AppUserId1")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("AppUserId1");

                    b.ToTable("IdentificationTypes");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.Insurer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("InsurerTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Nit")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("InsurerTypeId");

                    b.ToTable("Insurer");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.InsurerType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("InsurerTypes");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.Companies", b =>
                {
                    b.HasOne("DimmedAPI.Entidades.AppUser", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("DimmedAPI.Entidades.IdentificationTypes", "IdentificationType")
                        .WithMany()
                        .HasForeignKey("IdentificationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DimmedAPI.Entidades.AppUser", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId");

                    b.Navigation("CreatedByUser");

                    b.Navigation("IdentificationType");

                    b.Navigation("ModifiedByUser");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.IdentificationTypes", b =>
                {
                    b.HasOne("DimmedAPI.Entidades.AppUser", null)
                        .WithMany("CreatedIdentificationTypes")
                        .HasForeignKey("AppUserId");

                    b.HasOne("DimmedAPI.Entidades.AppUser", null)
                        .WithMany("ModifiedIdentificationTypes")
                        .HasForeignKey("AppUserId1");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.Insurer", b =>
                {
                    b.HasOne("DimmedAPI.Entidades.InsurerType", "InsurerType")
                        .WithMany()
                        .HasForeignKey("InsurerTypeId");

                    b.Navigation("InsurerType");
                });

            modelBuilder.Entity("DimmedAPI.Entidades.AppUser", b =>
                {
                    b.Navigation("CreatedIdentificationTypes");

                    b.Navigation("ModifiedIdentificationTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
