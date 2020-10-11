using System.Collections.Generic;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public interface IFileParser<out T>
    {
        T ParseFile(byte[] fileData);
    }
}