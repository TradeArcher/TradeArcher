using System.Collections.Generic;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public interface IFileParser
    {
        IList<Trade> ParseFile(byte[] fileData);
    }
}