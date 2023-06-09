using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DovizKuru.models;
using DovizKuru.services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DovizKuru.viewmodels
{
    internal class MainViewModel : ObservableObject
    {
        public IAsyncRelayCommand LoadPreferencesCommand { get => m_LoadPreferencesCommand; }
        public bool IsIdle { get => m_IsIdle; private set => SetProperty(ref m_IsIdle, value); }
        public ObservableCollection<ExchangeRate> ExchangeRates { get => m_ExchangeRates; private set => SetProperty(ref m_ExchangeRates, value); }
        public MainViewModel(IWindowService windowService, IWebService webService, IPreferenceService preferenceService)
        {
            m_WindowService = windowService;
            m_WebService = webService;
            m_PreferenceService = preferenceService;

            m_LoadPreferencesCommand = new AsyncRelayCommand(LoadPreferences, LoadPreferencesCanExecute);
        }


        private async Task LoadPreferences()
        {
            IsIdle = false;

            ExchangeRates = new(await m_PreferenceService.LoadRateList());

            IsIdle = true;

            await m_PreferenceService.SaveRateList(ExchangeRates);
        }


        #region Command States
        private bool LoadPreferencesCanExecute() => m_IsIdle;
        #endregion

        #region Fields
        private bool m_IsIdle = true;


        private readonly IWindowService m_WindowService;
        private readonly IWebService m_WebService;
        private readonly IPreferenceService m_PreferenceService;

        private ObservableCollection<ExchangeRate> m_ExchangeRates = new();


        private readonly IAsyncRelayCommand m_LoadPreferencesCommand;
        #endregion
    }
}
