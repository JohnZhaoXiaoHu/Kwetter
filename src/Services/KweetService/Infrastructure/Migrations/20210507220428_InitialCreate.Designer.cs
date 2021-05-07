﻿// <auto-generated />
using System;
using Kwetter.Services.KweetService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kwetter.Services.KweetService.Infrastructure.Migrations
{
    [DbContext(typeof(KweetDbContext))]
    [Migration("20210507220428_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.HashTag", b =>
                {
                    b.Property<Guid>("KweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("KweetId", "Tag");

                    b.ToTable("HashTag");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(140)
                        .HasColumnType("nvarchar(140)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Kweets");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.KweetLike", b =>
                {
                    b.Property<Guid>("KweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LikedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("KweetId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("KweetLike");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Mention", b =>
                {
                    b.Property<Guid>("KweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("KweetId", "UserName");

                    b.HasIndex("UserId");

                    b.ToTable("Mention");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.UserAggregate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserDisplayName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("UserProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.HashTag", b =>
                {
                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", null)
                        .WithMany("HashTags")
                        .HasForeignKey("KweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", b =>
                {
                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.UserAggregate", null)
                        .WithMany("Kweets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.KweetLike", b =>
                {
                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", null)
                        .WithMany("Likes")
                        .HasForeignKey("KweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.UserAggregate", null)
                        .WithMany("KweetLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Mention", b =>
                {
                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", null)
                        .WithMany("Mentions")
                        .HasForeignKey("KweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.UserAggregate", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.Kweet", b =>
                {
                    b.Navigation("HashTags");

                    b.Navigation("Likes");

                    b.Navigation("Mentions");
                });

            modelBuilder.Entity("Kwetter.Services.KweetService.Domain.AggregatesModel.UserAggregate.UserAggregate", b =>
                {
                    b.Navigation("KweetLikes");

                    b.Navigation("Kweets");
                });
#pragma warning restore 612, 618
        }
    }
}
