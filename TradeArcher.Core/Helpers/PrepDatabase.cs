using System;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Helpers
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            using (TradeArcherDataContext context = new TradeArcherDataContext())
            {
                //Migrate the Database
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("already"))
                    {
                        throw;
                    }
                }
            }
        }
    }
}