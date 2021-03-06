﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Data.Migrations
{
    [DbContext(typeof(TradeArcherDataContext))]
    partial class TradeArcherDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("TradeArcher.Core.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BrokerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.HasKey("AccountId");

                    b.HasIndex("BrokerId");

                    b.HasIndex("Name");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("TradeArcher.Core.Models.BackTestTrade", b =>
                {
                    b.Property<int>("BackTestTradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderSide")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<double>("Quantity")
                        .HasColumnType("REAL");

                    b.Property<int>("StrategyBackTestSessionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StrategyFullName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<int>("SymbolTradeId")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("TickerSessionPnl")
                        .HasColumnType("REAL");

                    b.Property<double>("TickerSessionPosition")
                        .HasColumnType("REAL");

                    b.Property<double?>("TradePnl")
                        .HasColumnType("REAL");

                    b.HasKey("BackTestTradeId");

                    b.HasIndex("ExecutionTime");

                    b.HasIndex("OrderSide");

                    b.HasIndex("Price");

                    b.HasIndex("StrategyBackTestSessionId");

                    b.HasIndex("Symbol");

                    b.HasIndex("TickerSessionPnl");

                    b.HasIndex("TradePnl");

                    b.HasIndex("SymbolTradeId");

                    b.ToTable("BackTestTrades");
                });

            modelBuilder.Entity("TradeArcher.Core.Models.Broker", b =>
                {
                    b.Property<int>("BrokerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.HasKey("BrokerId");

                    b.HasIndex("Name");

                    b.ToTable("Brokers");

                    b.HasData(
                        new
                        {
                            BrokerId = 1,
                            CreatedDate = new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(5068),
                            ModifiedDate = new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(5850),
                            Name = "TD Ameritrade"
                        },
                        new
                        {
                            BrokerId = 2,
                            CreatedDate = new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(6459),
                            ModifiedDate = new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(6469),
                            Name = "ThinkOrSwim"
                        });
                });

            modelBuilder.Entity("TradeArcher.Core.Models.Strategy", b =>
                {
                    b.Property<int>("StrategyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.HasKey("StrategyId");

                    b.HasIndex("Name");

                    b.ToTable("Strategies");
                });

            modelBuilder.Entity("TradeArcher.Core.Models.StrategyBackTestSession", b =>
                {
                    b.Property<int>("StrategyBackTestSessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<int>("StrategyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("StrategyBackTestSessionId");

                    b.HasIndex("Date");

                    b.HasIndex("Name");

                    b.HasIndex("StrategyId");

                    b.ToTable("StrategyBackTestSessions");
                });

            modelBuilder.Entity("TradeArcher.Core.Models.Trade", b =>
                {
                    b.Property<int>("TradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("NetPrice")
                        .HasColumnType("REAL");

                    b.Property<int>("OrderSide")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OrderType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PosEffect")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Spread")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Strike")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<int>("TradeType")
                        .HasColumnType("INTEGER");

                    b.HasKey("TradeId");

                    b.HasIndex("AccountId");

                    b.HasIndex("ExecutionTime");

                    b.HasIndex("OrderSide");

                    b.HasIndex("Price");

                    b.HasIndex("Symbol");

                    b.ToTable("TradeHistory");
                });

            modelBuilder.Entity("TradeArcher.Core.Models.Account", b =>
                {
                    b.HasOne("TradeArcher.Core.Models.Broker", "Broker")
                        .WithMany("Accounts")
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TradeArcher.Core.Models.BackTestTrade", b =>
                {
                    b.HasOne("TradeArcher.Core.Models.StrategyBackTestSession", "StrategyBackTestSession")
                        .WithMany("BackTestTrades")
                        .HasForeignKey("StrategyBackTestSessionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TradeArcher.Core.Models.StrategyBackTestSession", b =>
                {
                    b.HasOne("TradeArcher.Core.Models.Strategy", "Strategy")
                        .WithMany("Sessions")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TradeArcher.Core.Models.Trade", b =>
                {
                    b.HasOne("TradeArcher.Core.Models.Account", "Account")
                        .WithMany("Trades")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
