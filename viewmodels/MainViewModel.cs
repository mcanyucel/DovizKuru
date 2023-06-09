using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DovizKuru.models;
using DovizKuru.services;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace DovizKuru.viewmodels
{
    internal class MainViewModel : ObservableObject
    {
        public IAsyncRelayCommand LoadPreferencesCommand { get => m_LoadPreferencesCommand; }
        public bool IsIdle { get => m_IsIdle; private set => SetProperty(ref m_IsIdle, value); }
        public bool ShouldQueryRates { get => m_ShouldQueryRates; private set => SetProperty(ref m_ShouldQueryRates, value); }
        public ObservableCollection<ExchangeRate> ExchangeRates { get => m_ExchangeRates; private set => SetProperty(ref m_ExchangeRates, value); }
        public ObservableCollection<Alarm> AlarmList { get => m_AlarmList; private set => SetProperty(ref m_AlarmList, value); }

        public DateTime LastUpdate { get => m_LastUpdate; private set => SetProperty(ref m_LastUpdate, value); }
        public MainViewModel(IWebService webService, IPreferenceService preferenceService)
        {
            m_WebService = webService;
            m_PreferenceService = preferenceService;

            m_LoadPreferencesCommand = new AsyncRelayCommand(LoadPreferences, LoadPreferencesCanExecute);
            m_UpdateTimer = new(QueryExchangeRates, null, Timeout.Infinite, Timeout.Infinite);
        }


        private async Task LoadPreferences()
        {
            IsIdle = false;
            ExchangeRates = (ObservableCollection<ExchangeRate>)await m_PreferenceService.LoadRateList();
            AlarmList = (ObservableCollection<Alarm>)await m_PreferenceService.LoadAlarms();
            IsIdle = true;
        }

        private void QueryExchangeRates(object? state = null) => ShouldQueryRates = true;

        public void OnExchangeRatesQueried(string sourceHTML)
        {
            ShouldQueryRates = false;
            m_WebService.UpdateRates(sourceHTML, ExchangeRates);
            LastUpdate = DateTime.Now;
        }

        public void SourcePageLoaded() => m_UpdateTimer.Change(5000, c_UpdateTimerPeriod);

        #region Command States
        private bool LoadPreferencesCanExecute() => m_IsIdle;
        #endregion

        #region Fields
        private bool m_IsIdle = true;
        private bool m_ShouldQueryRates = false;
        private DateTime m_LastUpdate;

        private readonly IWebService m_WebService;
        private readonly IPreferenceService m_PreferenceService;

        private ObservableCollection<ExchangeRate> m_ExchangeRates = new();

        private readonly IAsyncRelayCommand m_LoadPreferencesCommand;

        private readonly Timer m_UpdateTimer;
        private const int c_UpdateTimerPeriod = 15000; // 15 seconds

        private ObservableCollection<Alarm> m_AlarmList = new();
        #endregion
    }
}
