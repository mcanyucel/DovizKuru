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
        public PreferenceService(ILogService logService) => m_LogService = logService;
        public async Task<IEnumerable<ExchangeRate>> LoadRateList()
        {
            ObservableCollection<ExchangeRate> rates;
            if (!File.Exists(m_PreferenceFileName))
            {
                rates = await GenerateExchanges();
            }
            else
            {
                string dataText = string.Empty;
                await m_PreferenceFileSemaphore.WaitAsync();
                try
                {
                    dataText = await File.ReadAllTextAsync(m_PreferenceFileName);
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
            }
            return rates;
        }

        public async Task SaveRateList(IEnumerable<ExchangeRate> rates)
        {
            string dataText = await Task.Run(() => Newtonsoft.Json.JsonConvert.SerializeObject(rates));
            await m_PreferenceFileSemaphore.WaitAsync();
            try
            {
                await File.WriteAllTextAsync(m_PreferenceFileName, dataText);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while saving preference file: {e.Message}");
            }
            m_PreferenceFileSemaphore.Release();
        }

        public async Task<IEnumerable<Alarm>> LoadAlarms()
        {
            if (!File.Exists(m_AlarmsFile)) return new ObservableCollection<Alarm>();

            ObservableCollection<Alarm> alarms;
            string dataText = string.Empty;
            await m_AlarmFileSemaphore.WaitAsync();
            try
            {
                dataText = await File.ReadAllTextAsync(m_AlarmsFile);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while loading alarms file: {e.Message}");
            }
            m_AlarmFileSemaphore.Release();
            try
            {
                alarms = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<Alarm>>(dataText) ?? new());
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while deserializing alarms file: {e.Message}");
                alarms = new();
            }
            return alarms;
        }

        public async Task SaveAlarms(IEnumerable<Alarm> alarms)
        {
            string dataText = await Task.Run(() => Newtonsoft.Json.JsonConvert.SerializeObject(alarms));
            await m_AlarmFileSemaphore.WaitAsync();
            try
            {
                await File.WriteAllTextAsync(m_AlarmsFile, dataText);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while saving alarms file: {e.Message}");
            }
            m_AlarmFileSemaphore.Release();
        }
        /// <summary>
        /// Generates and saves the default exchange rate list when the application is first run.
        /// </summary>
        /// <returns></returns>
        private async Task<ObservableCollection<ExchangeRate>> GenerateExchanges()
        {
            var defaultString = @"[{""Code"":""USD"",""Name"":""Dolar"",""NewBuying"":0.0,""OldBuying"":0.0,""ChangeRate"":0.0,""ChangePercentage"":0.0,""XPath"":""/html/body/main/div/section[2]/div/div[1]/div[2]/div[1]/div/div[1]/div/div[2]/div[2]/span[2]""},{""Code"":""EUR"",""Name"":""Euro"",""NewBuying"":0.0,""OldBuying"":0.0,""ChangeRate"":0.0,""ChangePercentage"":0.0,""XPath"":""/html/body/main/div/section[2]/div/div[1]/div[2]/div[1]/div/div[2]/div/div[2]/div[2]/span[2]""},{""Code"":""XAU"",""Name"":""Altın"",""NewBuying"":0.0,""OldBuying"":0.0,""ChangeRate"":0.0,""ChangePercentage"":0.0,""XPath"":""/html/body/main/div/section[2]/div/div[1]/div[4]/div[2]/table/tbody/tr[2]/td[4]""}]";
            await m_PreferenceFileSemaphore.WaitAsync();
            try
            {
                Directory.CreateDirectory(m_AppDataPath);
                await File.WriteAllTextAsync(m_PreferenceFileName, defaultString);
            }
            catch (Exception e)
            {
                m_LogService.LogError($"Error while saving preference file: {e.Message}");
            }
            m_PreferenceFileSemaphore.Release();
            return await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<ExchangeRate>>(defaultString) ?? new());
        }


        #region Fields
        private readonly static string m_AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DovizKuru");
        private readonly string m_PreferenceFileName = Path.Combine(m_AppDataPath, "preferences.json");
        private readonly string m_AlarmsFile = Path.Combine(m_AppDataPath, "alarms.json");
        private readonly SemaphoreSlim m_PreferenceFileSemaphore = new(1, 1);
        private readonly SemaphoreSlim m_AlarmFileSemaphore = new(1, 1);
        private readonly ILogService m_LogService;
        #endregion
    }
}
