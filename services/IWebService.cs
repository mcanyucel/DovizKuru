using DovizKuru.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DovizKuru.services
{
    internal interface IWebService
    {
        Task UpdateRates(Dictionary<string, List<ExchangeRate>> exchangeRateDictionary);

        Task UpdateRates(string sourceHTML, IEnumerable<ExchangeRate> exchangeRates);
    }
}
