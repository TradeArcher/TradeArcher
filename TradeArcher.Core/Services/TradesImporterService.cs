using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public interface ITradesImporterService
    {
        bool Import(FileParserType parserType, int accountId, byte[] fileData);
    }

    public class TradesImporterService : ITradesImporterService
    {
        public bool Import(FileParserType parserType, int accountId, byte[] fileData)
        {
            var importSucceeded = false;

            var parser = GetFileParser(parserType);

            var trades = parser.ParseFile(fileData);

            using (var context = new TradeArcherDataContext())
            {
                using (var dbTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var dbAccount = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);

                        foreach (var trade in trades)
                        {
                            Trade dbTrade = null;

                            //check if a trade with matching fields already exists and update or add new if it doesn't

                            dbTrade = context.TradeHistory.AsQueryable().Include(t => t.Account).ToList().FirstOrDefault(t => t.Compare(trade) && t.Account.AccountId == dbAccount.AccountId);

                            if (dbTrade != null)
                            {
                                //Update Existing
                                dbTrade.Update(trade);
                            }
                            else
                            {
                                //Add
                                dbTrade = trade;
                                dbTrade.Account = dbAccount;
                                context.TradeHistory.Add(dbTrade);
                            }
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

            return importSucceeded;
        }

        private IFileParser GetFileParser(FileParserType parserType)
        {
            switch (parserType)
            {
                case FileParserType.TDAmeritrade:
                    throw new NotSupportedException($"Import of {parserType} is not supported.");
                case FileParserType.ThinkOrSwim:
                    return new ThinkOrSwimFileParser();
                default:
                    throw new NotSupportedException($"Import of {parserType} is not supported.");
            }
        }
    }

    public enum FileParserType
    {
        NotSupported,
        ThinkOrSwim,
        TDAmeritrade
    }
}