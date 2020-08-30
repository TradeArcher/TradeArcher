using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TradeArcher.Core.Models
{
    public class TradeArcherDataContext : DbContext
    {
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Trade> TradeHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=tradearcher.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Broker>()
                .HasMany<Account>(b => b.Accounts)
                .WithOne(a => a.Broker)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasOne<Broker>()
                .WithMany(b => b.Accounts);

            modelBuilder.Entity<Account>()
                .HasMany<Trade>(a => a.Trades)
                .WithOne(t => t.Account)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trade>()
                .HasOne<Account>()
                .WithMany(a => a.Trades);

            modelBuilder.Entity<Broker>().HasData(new Broker { BrokerId = 1, Name = "TD Ameritrade" }, new Broker { BrokerId = 2, Name = "ThinkOrSwim" });

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Broker
    {
        public int BrokerId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; } = new List<Account>();
    }

    public class Account
    {
        public int AccountId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        [Display(Name = "Nickname")]
        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string DisplayName { get; set; }

        [Display(Name = "Broker")]
        [Required(ErrorMessage = "A broker is required.")]
        [ForeignKey("BrokerId")]
        public virtual Broker Broker { get; set; }

        public virtual ICollection<Trade> Trades { get; } = new List<Trade>();
    }

    
    public class Platform
    {
        public int PlatformId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }
    }


    public class Trade
    {
        //Exec Time, Spread, Side, Qty, Symbol, and Price, Order Type
        public int TradeId { get; set; }

        [Display(Name = "Order Execution Time")]
        [Required(ErrorMessage = "ExecutionTime is required.")]
        public DateTime ExecutionTime { get; set; }

        [Display(Name = "Order Expiration")]
        public DateTime Expiration { get; set; }

        [Required(ErrorMessage = "Spread is required.")]
        public OrderSpread Spread { get; set; }

        [Display(Name = "Side")]
        [Required(ErrorMessage = "Side is required.")]
        public OrderSide OrderSide { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Symbol is required.")]
        [StringLength(10, ErrorMessage = "Symbol cannot be more than 10 characters")]
        public string Symbol { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; }

        [Display(Name = "Net Price")]
        public double NetPrice { get; set; }

        [Display(Name = "Trade Type")]
        public TradeType TradeType { get; set; }

        [Display(Name = "Order Type")]
        [Required(ErrorMessage = "OrderType is required.")]
        public OrderType OrderType { get; set; }

        [StringLength(255, ErrorMessage = "Strike cannot be more than 255 characters")]
        public string Strike { get; set; }

        [Display(Name = "Pos Effect")]
        [StringLength(255, ErrorMessage = "PosEffect cannot be more than 255 characters")]
        public string PosEffect { get; set; }

        [Display(Name = "Account")]
        [Required(ErrorMessage = "An account is required.")]
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

    }

    public enum OrderSpread
    {
        Stock
    }

    public enum OrderSide
    {
        Buy,
        Sell
    }

    public enum OrderType
    {
        Limit,
        Market
    }

    public enum TradeType
    {
        Stock,
        Future,
        Option,
        Forex
    }
}