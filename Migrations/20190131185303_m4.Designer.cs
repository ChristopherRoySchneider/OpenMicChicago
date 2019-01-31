﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenMicChicago.Models;

namespace OpenMicChicago.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20190131185303_m4")]
    partial class m4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("OpenMicChicago.Models.Genre", b =>
                {
                    b.Property<int>("GenreID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("GenreID");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("OpenMicChicago.Models.Like", b =>
                {
                    b.Property<int>("LikeID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OpenMicID");

                    b.Property<int>("UserID");

                    b.HasKey("LikeID");

                    b.HasIndex("OpenMicID");

                    b.HasIndex("UserID");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("OpenMicChicago.Models.OpenMic", b =>
                {
                    b.Property<int>("OpenMicID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("CreatorUserID");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Duration");

                    b.Property<string>("DurationUnit")
                        .IsRequired();

                    b.Property<DateTime>("EndDateTime");

                    b.Property<string>("Frequency")
                        .IsRequired()
                        .HasColumnType("VARCHAR(50)");

                    b.Property<string>("PhotoURL");

                    b.Property<DateTime>("Signup");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("VenueID");

                    b.HasKey("OpenMicID");

                    b.HasIndex("CreatorUserID");

                    b.HasIndex("VenueID");

                    b.ToTable("OpenMics");
                });

            modelBuilder.Entity("OpenMicChicago.Models.OpenMicGenre", b =>
                {
                    b.Property<int>("OpenMicGenreID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GenreID");

                    b.Property<int>("OpenMicID");

                    b.HasKey("OpenMicGenreID");

                    b.HasIndex("GenreID");

                    b.HasIndex("OpenMicID");

                    b.ToTable("OpenMicGenres");
                });

            modelBuilder.Entity("OpenMicChicago.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("PhotoURL");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OpenMicChicago.Models.Venue", b =>
                {
                    b.Property<int>("VenueID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.Property<string>("PhotoURL");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.Property<string>("StreetAndNumber")
                        .IsRequired();

                    b.Property<string>("Unit");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserID");

                    b.Property<string>("Website")
                        .IsRequired();

                    b.Property<string>("ZipCode")
                        .IsRequired();

                    b.Property<double>("latitude");

                    b.Property<double>("longitude");

                    b.HasKey("VenueID");

                    b.HasIndex("UserID");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("OpenMicChicago.Models.Like", b =>
                {
                    b.HasOne("OpenMicChicago.Models.OpenMic", "OpenMic")
                        .WithMany("Likes")
                        .HasForeignKey("OpenMicID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OpenMicChicago.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenMicChicago.Models.OpenMic", b =>
                {
                    b.HasOne("OpenMicChicago.Models.User", "Creator")
                        .WithMany("OpenMics")
                        .HasForeignKey("CreatorUserID");

                    b.HasOne("OpenMicChicago.Models.Venue", "Venue")
                        .WithMany("OpenMics")
                        .HasForeignKey("VenueID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenMicChicago.Models.OpenMicGenre", b =>
                {
                    b.HasOne("OpenMicChicago.Models.Genre", "Genre")
                        .WithMany("OpenMics")
                        .HasForeignKey("GenreID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OpenMicChicago.Models.OpenMic", "OpenMic")
                        .WithMany("Genres")
                        .HasForeignKey("OpenMicID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenMicChicago.Models.Venue", b =>
                {
                    b.HasOne("OpenMicChicago.Models.User", "Creator")
                        .WithMany("Venues")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
