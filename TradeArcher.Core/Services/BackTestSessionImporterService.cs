using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public interface IBackTestSessionImporterService<out TFileParser>
    {
        bool Import(BackTestFileParserType parserType, byte[] fileData);
        IFileParser<TFileParser> GetFileParser(BackTestFileParserType parserType);
    }

    public class BackTestSessionImporterService : IBackTestSessionImporterService<StrategyBackTestSession>
    {
        public bool Import(BackTestFileParserType parserType, byte[] fileData)
        {
            var importSucceeded = false;

            var parser = GetFileParser(parserType);

            var session = parser.ParseFile(fileData);

            if (session != null && session.BackTestTrades.Count > 0)
            {
                using (var context = new TradeArcherDataContext())
                {
                    using (var dbTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var strategyName = session.BackTestTrades.First().StrategyFullName;

                            if (strategyName.Contains("("))
                            {
                                strategyName = strategyName.Substring(0, strategyName.IndexOf("(", StringComparison.InvariantCulture));
                            }

                            var dbStrategy = context.Strategies.Include(s => s.Sessions).ThenInclude(s => s.BackTestTrades).FirstOrDefault(s => s.Name == strategyName);

                            if (dbStrategy == null)
                            {
                                dbStrategy = new Strategy
                                {
                                    Name = strategyName
                                };
                            }

                            //try to find an existing matching session and create one if not found
                            var dbSession = dbStrategy.Sessions.FirstOrDefault(s => s.Name == $"{dbStrategy.Name}_{DateTime.UtcNow:yyyy-MM-dd}");

                            if (dbSession == null)
                            {
                                session.Name = $"{dbStrategy.Name}_{DateTime.UtcNow:yyyy-MM-dd}";
                                session.Date = DateTime.UtcNow;

                                dbStrategy.Sessions.Add(session);
                            }
                            else
                            {
                                foreach (var backTestTrade in session.BackTestTrades)
                                {
                                    //check if a trade with matching fields already exists and update or add new if it doesn't

                                    var dbBackTestTrade = dbSession.BackTestTrades.FirstOrDefault(t => t.Compare(backTestTrade));

                                    if (dbBackTestTrade != null)
                                    {
                                        //Update Existing
                                        dbBackTestTrade.Update(backTestTrade);
                                    }
                                    else
                                    {
                                        dbSession.BackTestTrades.Add(backTestTrade);
                                    }
                                }
                            }

                            if (dbStrategy.StrategyId == 0)
                            {
                                context.Strategies.Add(dbStrategy);
                            }
                            else
                            {
                                context.Strategies.Update(dbStrategy);
                            }

                            context.SaveChanges();

                            dbTransaction.Commit();

                            importSucceeded = true;
                        }
                        catch (Exception ex)
                        {
                            dbTransaction.Rollback();
                        }
                    }
                }
            }

            return importSucceeded;
        }

        public IFileParser<StrategyBackTestSession> GetFileParser(BackTestFileParserType parserType)
        {
            switch (parserType)
            {
                case BackTestFileParserType.ThinkOrSwim:
                    return new ThinkOrSwimBackTestReportParser();
                default:
                    throw new NotSupportedException($"Import of {parserType} is not supported.");
            }
        }
    }

    
    public enum BackTestFileParserType
    {
        NotSupported,
        ThinkOrSwim
    }

}