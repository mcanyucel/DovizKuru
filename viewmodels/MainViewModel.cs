using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DovizKuru.models;
using DovizKuru.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DovizKuru.viewmodels
{
    internal class MainViewModel : ObservableObject
    {
        public IAsyncRelayCommand LoadPreferencesCommand { get => m_LoadPreferencesCommand; }
        public bool IsIdle { get => m_IsIdle; private set => SetProperty(ref m_IsIdle, value); }
        public ObservableCollection<ExchangeRate> ExchangeRates { get => m_ExchangeRates; private set => SetProperty(ref m_ExchangeRates, value); }
        public DateTime LastUpdate { get => m_LastUpdate; private set => SetProperty(ref m_LastUpdate, value); }
        public MainViewModel(IWindowService windowService, IWebService webService, IPreferenceService preferenceService)
        {
            m_WindowService = windowService;
            m_WebService = webService;
            m_PreferenceService = preferenceService;

            m_LoadPreferencesCommand = new AsyncRelayCommand(LoadPreferences, LoadPreferencesCanExecute);

            m_UpdateTimer = new(UpdateExchangeRates, null, Timeout.Infinite, Timeout.Infinite);
        }


        private async Task LoadPreferences()
        {
            IsIdle = false;

            ExchangeRates = new(await m_PreferenceService.LoadRateList());
            m_ExchangeRateDictionary = ExchangeRates.GroupBy(x => x.SourceUrl).ToDictionary(x => x.Key, x => x.ToList());

            m_UpdateTimer.Change(0, 15000);
        }

        private void UpdateExchangeRates(object? state = null)
        {
            Task.Run(async () =>
            {
                IsIdle = false;
                await m_WebService.UpdateRates(m_ExchangeRateDictionary);
                LastUpdate = DateTime.Now;
                IsIdle = true;
            });
        }


        #region Command States
        private bool LoadPreferencesCanExecute() => m_IsIdle;
        #endregion

        #region Fields
        private bool m_IsIdle = true;
        private DateTime m_LastUpdate;

        private readonly IWindowService m_WindowService;
        private readonly IWebService m_WebService;
        private readonly IPreferenceService m_PreferenceService;

        private ObservableCollection<ExchangeRate> m_ExchangeRates = new();
        private Dictionary<string, List<ExchangeRate>> m_ExchangeRateDictionary = new();

        private readonly IAsyncRelayCommand m_LoadPreferencesCommand;

        private readonly Timer m_UpdateTimer;
        #endregion
    }
}
