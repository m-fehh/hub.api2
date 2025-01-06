﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Hub.Domain.Migrations
{
    [DbContext(typeof(EntityDbContext))]
    partial class EntityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Hub.Domain.Entities.AccessRule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("KeyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Tree")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AccessRule");
                });

            modelBuilder.Entity("Hub.Domain.Entities.DocumentType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Abbrev")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("Inactive")
                        .HasColumnType("bit");

                    b.Property<string>("Mask")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("MaxLength")
                        .HasColumnType("bigint");

                    b.Property<long>("MinLength")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SpecialDocumentValidation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DocumentType");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Establishment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ClosingTime")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommercialAbbrev")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommercialName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("EstablishmentClassifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("OpeningTime")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("OrganizationalStructureId")
                        .HasColumnType("bigint");

                    b.Property<string>("SocialName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("TimeZoneDifference")
                        .HasColumnType("int");

                    b.Property<string>("TimezoneIdentifier")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationalStructureId");

                    b.ToTable("Establishment");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Incorporation.IncorporationEstablishment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("EstablishmentId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("IncorporationDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("OrganizationalStructureId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.HasIndex("OrganizationalStructureId");

                    b.HasIndex("UserId");

                    b.ToTable("IncorporationEstablishment");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Incorporation.IncorporationEstablishmentConfig", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("ConfigId")
                        .HasColumnType("bigint");

                    b.Property<long>("IncorporationEstablishmentId")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationalStructureId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ConfigId");

                    b.HasIndex("IncorporationEstablishmentId");

                    b.HasIndex("OrganizationalStructureId");

                    b.ToTable("IncorporationEstablishmentConfig");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.OrgStructConfigDefault", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("ApplyToDomain")
                        .HasColumnType("bit");

                    b.Property<bool>("ApplyToLeaf")
                        .HasColumnType("bit");

                    b.Property<bool>("ApplyToRoot")
                        .HasColumnType("bit");

                    b.Property<string>("ConfigType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("DefaultValue")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Legend")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("MaxLength")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Options")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("OrderConfig")
                        .HasColumnType("int");

                    b.Property<long?>("OrgStructConfigDefaultDependencyId")
                        .HasColumnType("bigint");

                    b.Property<string>("SearchExtraCondition")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SearchName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OrgStructConfigDefaultDependencyId");

                    b.ToTable("OrgStructConfigDefault");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.OrganizationalStructureConfig", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("ConfigId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<long>("OrganizationalStructureId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ConfigId");

                    b.HasIndex("OrganizationalStructureId");

                    b.ToTable("OrganizationalStructureConfig");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Logs.Log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long?>("CreateUserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("FatherId")
                        .HasColumnType("bigint");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LogType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LogVersion")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("ObjectId")
                        .HasColumnType("bigint");

                    b.Property<string>("ObjectName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Logs.LogField", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long?>("LogId")
                        .HasColumnType("bigint");

                    b.Property<string>("NewValue")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OldValue")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("LogField");
                });

            modelBuilder.Entity("Hub.Domain.Entities.ProfileGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("Administrator")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowMultipleAccess")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PasswordExpirationDays")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProfileGroup");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.Person", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ExternalCode")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("OwnerOrgStructId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AreaCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ChangingPass")
                        .HasColumnType("bit");

                    b.Property<string>("ChatUserStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<long?>("DefaultOrgStructureId")
                        .HasColumnType("bigint");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Inactive")
                        .HasColumnType("bit");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsFromApi")
                        .HasColumnType("bit");

                    b.Property<string>("Keyword")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("LastAccessDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastPasswordRecoverRequestDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastUserUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("OwnerOrgStructId")
                        .HasColumnType("bigint");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long?>("PersonId")
                        .HasColumnType("bigint");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("ProfileId")
                        .HasColumnType("bigint");

                    b.Property<string>("QrCodeInfo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TempPassword")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("UserRoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("PortalUser");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserFingerprint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("BrowserInfo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("BrowserName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool?>("CookieEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Lat")
                        .HasColumnType("float");

                    b.Property<double?>("Lng")
                        .HasColumnType("float");

                    b.Property<string>("OS")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PortalUserFingerprints");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserPassHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpirationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("PortalUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PortalUserId");

                    b.ToTable("PortalUserPassHistory");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("PortalUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("PortalUserId");

                    b.ToTable("PortalUserSettings");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.UserRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ExternalCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("Inactive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<long>("OwnerOrgStructId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OwnerOrgStructId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Abbrev")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("AppearInMobileApp")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreationUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ExternalCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long?>("FatherId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Inactive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDomain")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLeaf")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRoot")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Tree")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("FatherId");

                    b.ToTable("OrganizationalStructure");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Establishment", b =>
                {
                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "OrganizationalStructure")
                        .WithMany()
                        .HasForeignKey("OrganizationalStructureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrganizationalStructure");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Incorporation.IncorporationEstablishment", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Enterprise.Establishment", "Establishment")
                        .WithMany()
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "OrganizationalStructure")
                        .WithMany()
                        .HasForeignKey("OrganizationalStructureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Hub.Domain.Entities.Users.PortalUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");

                    b.Navigation("OrganizationalStructure");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.Incorporation.IncorporationEstablishmentConfig", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Enterprise.OrgStructConfigDefault", "Config")
                        .WithMany()
                        .HasForeignKey("ConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hub.Domain.Entities.Enterprise.Incorporation.IncorporationEstablishment", "IncorporationEstablishment")
                        .WithMany()
                        .HasForeignKey("IncorporationEstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "OrganizationalStructure")
                        .WithMany()
                        .HasForeignKey("OrganizationalStructureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Config");

                    b.Navigation("IncorporationEstablishment");

                    b.Navigation("OrganizationalStructure");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.OrgStructConfigDefault", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Enterprise.OrgStructConfigDefault", "OrgStructConfigDefaultDependency")
                        .WithMany()
                        .HasForeignKey("OrgStructConfigDefaultDependencyId");

                    b.Navigation("OrgStructConfigDefaultDependency");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Enterprise.OrganizationalStructureConfig", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Enterprise.OrgStructConfigDefault", "Config")
                        .WithMany()
                        .HasForeignKey("ConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "OrganizationalStructure")
                        .WithMany()
                        .HasForeignKey("OrganizationalStructureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Config");

                    b.Navigation("OrganizationalStructure");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUser", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Users.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId");

                    b.HasOne("Hub.Domain.Entities.Users.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Person");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserFingerprint", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Users.PortalUser", "PortalUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PortalUser");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserPassHistory", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Users.PortalUser", "PortalUser")
                        .WithMany()
                        .HasForeignKey("PortalUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PortalUser");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.PortalUserSetting", b =>
                {
                    b.HasOne("Hub.Domain.Entities.Users.PortalUser", "PortalUser")
                        .WithMany()
                        .HasForeignKey("PortalUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PortalUser");
                });

            modelBuilder.Entity("Hub.Domain.Entities.Users.UserRole", b =>
                {
                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "OwnerOrgStruct")
                        .WithMany()
                        .HasForeignKey("OwnerOrgStructId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwnerOrgStruct");
                });

            modelBuilder.Entity("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", b =>
                {
                    b.HasOne("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", "Father")
                        .WithMany("Childrens")
                        .HasForeignKey("FatherId");

                    b.Navigation("Father");
                });

            modelBuilder.Entity("Hub.Infrastructure.Database.Models.Tenant.OrganizationalStructure", b =>
                {
                    b.Navigation("Childrens");
                });
#pragma warning restore 612, 618
        }
    }
}
