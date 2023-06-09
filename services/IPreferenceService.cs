using DovizKuru.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DovizKuru.services
{
    internal interface IPreferenceService
    {
        Task<IEnumerable<ExchangeRate>> LoadRateList();
        Task SaveRateList(IEnumerable<ExchangeRate> rates);
    }
}
