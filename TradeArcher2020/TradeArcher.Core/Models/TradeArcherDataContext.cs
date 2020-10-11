using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;

namespace TradeArcher.Core.Models
{
    public class TradeArcherDataContext : DbContext
    {
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Trade> TradeHistory { get; set; }
        public DbSet<StrategyBackTestSession> StrategyBackTestSessions { get; set; }
        public DbSet<Strategy> Strategies { get; set; }
        public DbSet<BackTestTrade> BackTestTrades { get; set; }

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

            modelBuilder.Entity<Broker>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<Account>()
                .HasOne<Broker>(a => a.Broker)
                .WithMany(b => b.Accounts);

            modelBuilder.Entity<Account>()
                .HasMany<Trade>(a => a.Trades)
                .WithOne(t => t.Account)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<Trade>()
                .HasOne<Account>(t => t.Account)
                .WithMany(a => a.Trades);

            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.Price);

            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.OrderSide);

            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.Symbol);

            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.ExecutionTime);

            modelBuilder.Entity<Strategy>()
                .HasMany<StrategyBackTestSession>(s => s.Sessions)
                .WithOne(s => s.Strategy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Strategy>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<StrategyBackTestSession>()
                .HasOne<Strategy>(s => s.Strategy)
                .WithMany(s => s.Sessions);

            modelBuilder.Entity<StrategyBackTestSession>()
                .HasMany<BackTestTrade>(s => s.BackTestTrades)
                .WithOne(t => t.StrategyBackTestSession)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StrategyBackTestSession>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<StrategyBackTestSession>()
                .HasIndex(t => t.Date);

            modelBuilder.Entity<BackTestTrade>()
                .HasOne<StrategyBackTestSession>(t => t.StrategyBackTestSession)
                .WithMany(s => s.BackTestTrades);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.ExecutionTime);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.OrderSide);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.Price);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.TradePnl);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.TickerSessionPnl);

            modelBuilder.Entity<BackTestTrade>()
                .HasIndex(t => t.Symbol);

            modelBuilder.Entity<Broker>().HasData(
                new Broker { BrokerId = 1, Name = "TD Ameritrade", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow}, 
                new Broker { BrokerId = 2, Name = "ThinkOrSwim", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
                );

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedDate = DateTime.UtcNow;
                }

                ((BaseEntity)entity.Entity).ModifiedDate = DateTime.UtcNow;
            }
        }
    }

    public class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

    public class Broker : BaseEntity
    {
        public int BrokerId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; private set; } = new List<Account>();
    }

    public class Account : BaseEntity
    {
        public int AccountId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        [Display(Name = "Nickname")]
        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string DisplayName { get; set; }

        [Display(Name = "Broker")]
        [Required(ErrorMessage = "A broker is required.")]
        //[ForeignKey("BrokerId")]
        public virtual Broker Broker { get; set; }

        public virtual ICollection<Trade> Trades { get; private set; } = new List<Trade>();
    }


    public class Platform : BaseEntity
    {
        public int PlatformId { get; set; }

        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }
    }


    public class Trade : BaseEntity
    {
        //Exec Time, Spread, Side, Qty, Symbol, and Price, Order Type
        public int TradeId { get; set; }

        [Display(Name = "Order Execution Time")]
        [Required(ErrorMessage = "ExecutionTime is required.")]
        public DateTime ExecutionTime { get; set; }

        [Display(Name = "Order Expiration")]
        public DateTime? Expiration { get; set; }

        [Required(ErrorMessage = "Spread is required.")]
        public OrderSpread Spread { get; set; }

        [Display(Name = "Side")]
        [Required(ErrorMessage = "Side is required.")]
        public OrderSide OrderSide { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

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
        //[ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public bool Compare(Trade trade)
        {
            return trade.ExecutionTime == ExecutionTime
                && trade.Expiration == Expiration
                && trade.Spread == Spread
                && trade.OrderSide == OrderSide
                && trade.Quantity == Quantity
                && trade.Symbol == Symbol
                && trade.Price == Price
                && trade.NetPrice == NetPrice
                && trade.TradeType == TradeType
                && trade.OrderType == OrderType
                && trade.Strike == Strike
                && trade.PosEffect == PosEffect;
        }

        public void Update(Trade trade)
        {
            ExecutionTime = trade.ExecutionTime;
            Expiration = trade.Expiration;
            Spread = trade.Spread;
            OrderSide = trade.OrderSide;
            Quantity = trade.Quantity;
            Symbol = trade.Symbol;
            Price = trade.Price;
            NetPrice = trade.NetPrice;
            TradeType = trade.TradeType;
            OrderType = trade.OrderType;
            Strike = trade.Strike;
            PosEffect = trade.PosEffect;
        }
    }

    public class BackTestTrade : BaseEntity
    {
        public int BackTestTradeId { get; set; }

        //This is the Id from the specific Symbol Backtest report
        [Required(ErrorMessage = "SymbolTradeId is required.")]
        public int SymbolTradeId { get; set; }

        [Required(ErrorMessage = "Strategy Full Name is required.")]
        [StringLength(255, ErrorMessage = "Strategy Full Name cannot be more than 255 characters")]
        public string StrategyFullName { get; set; }

        [Required(ErrorMessage = "Symbol is required.")]
        [StringLength(10, ErrorMessage = "Symbol cannot be more than 10 characters")]
        public string Symbol { get; set; }

        [Display(Name = "Side")]
        [Required(ErrorMessage = "Side is required.")]
        public OrderSide OrderSide { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; }

        [Display(Name = "Order Execution Time")]
        [Required(ErrorMessage = "ExecutionTime is required.")]
        public DateTime ExecutionTime { get; set; }

        [Display(Name = "P/L Per Trade")]
        public double? TradePnl { get; set; }

        [Display(Name = "Running P/L Per Ticker and Session")]
        public double? TickerSessionPnl { get; set; }

        [Display(Name = "Running Position Per Ticker and Session")]
        [Required(ErrorMessage = "TickerSessionPosition is required.")]
        public double TickerSessionPosition { get; set; }

        [Display(Name = "StrategyBackTestSession")]
        [Required(ErrorMessage = "A strategy is required.")]
        public virtual StrategyBackTestSession StrategyBackTestSession { get; set; }

        public bool Compare(BackTestTrade trade)
        {
            return trade.SymbolTradeId == SymbolTradeId
                   && trade.Symbol == Symbol
                   && trade.OrderSide == OrderSide
                   && trade.ExecutionTime == ExecutionTime;
        }

        public void Update(BackTestTrade trade)
        {
            SymbolTradeId = trade.SymbolTradeId;
            Symbol = trade.Symbol;
            OrderSide = trade.OrderSide;
            Quantity = trade.Quantity;
            Price = trade.Price;
            ExecutionTime = trade.ExecutionTime;
            TradePnl = trade.TradePnl;
            TickerSessionPnl = trade.TickerSessionPnl;
            TickerSessionPosition = trade.TickerSessionPosition;
            StrategyFullName = trade.StrategyFullName;
        }
    }

    public class StrategyBackTestSession : BaseEntity
    {
        public int StrategyBackTestSessionId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot be more than 255 characters")]
        public string Description { get; set; }

        [Display(Name = "Date the Backtest was run.")]
        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Display(Name = "Strategy")]
        [Required(ErrorMessage = "A strategy is required.")]
        //[ForeignKey("AccountId")]
        public virtual Strategy Strategy { get; set; }

        public virtual ICollection<BackTestTrade> BackTestTrades { get; private set; } = new List<BackTestTrade>();
    }

    public class Strategy : BaseEntity
    {
        public int StrategyId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Name { get; set; }

        public virtual ICollection<StrategyBackTestSession> Sessions { get; private set; } = new List<StrategyBackTestSession>();
    }

    public enum OrderSpread
    {
        Unknown,
        Stock
    }

    public enum OrderSide
    {
        Unknown,
        BuyToOpen,
        SellToClose,
        SellToOpen,
        BuyToClose
    }

    public enum OrderType
    {
        Unknown,
        Limit,
        Market,
        Stop,
        StopLimit
    }

    public enum TradeType
    {
        Unknown,
        Stock,
        Future,
        Option,
        Forex
    }
}