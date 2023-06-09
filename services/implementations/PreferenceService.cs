using DovizKuru.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DovizKuru.services.implementations
{
    internal class PreferenceService : IPreferenceService
    {
        public PreferenceService(ILogService logService)
        {
            m_LogService = logService;
        }
        public async Task<IEnumerable<ExchangeRate>> LoadRateList()
        {
            ObservableCollection<ExchangeRate> rates;
            string dataText = string.Empty;
            await m_PreferenceFileSemaphore.WaitAsync();
            try
            {
                dataText = await File.ReadAllTextAsync(c_PreferenceFileName);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while loading preference file: {e.Message}");
            }
            m_PreferenceFileSemaphore.Release();
            try
            {
                rates = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<ExchangeRate>>(dataText) ?? new());
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while deserializing preference file: {e.Message}");
                rates = new();
            }
            return rates;
        }

        public async Task SaveRateList(IEnumerable<ExchangeRate> rates)
        {
            string dataText = await Task.Run(() => Newtonsoft.Json.JsonConvert.SerializeObject(rates));
            await m_PreferenceFileSemaphore.WaitAsync();
            try
            {
                await File.WriteAllTextAsync(c_PreferenceFileName, dataText);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while saving preference file: {e.Message}");
            }
            m_PreferenceFileSemaphore.Release();
        }


        #region Fields
        private const string c_PreferenceFileName = "preferences.json";
        private readonly SemaphoreSlim m_PreferenceFileSemaphore = new(1, 1);
        private readonly ILogService m_LogService;
        #endregion
    }
}
