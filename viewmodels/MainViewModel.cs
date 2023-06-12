using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DovizKuru.models;
using DovizKuru.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DovizKuru.viewmodels
{
    internal class MainViewModel : ObservableObject
    {
        public bool AlwaysOnTop { get => m_AlwaysOnTop; set => SetProperty(ref m_AlwaysOnTop, value); }
        public static List<AlarmOperator> AlarmOperators { get => Enum.GetValues(typeof(AlarmOperator)).Cast<AlarmOperator>().ToList(); }
        public ExchangeRate? SelectedAlarmExchange { get => m_SelectedAlarmExchange; set => SetProperty(ref m_SelectedAlarmExchange, value); }
        public double SelectedAlarmValue { get => m_SelectedAlarmValue; set => SetProperty(ref m_SelectedAlarmValue, value); }
        public IAsyncRelayCommand LoadPreferencesCommand { get => m_LoadPreferencesCommand; }
        public IRelayCommand ShowAlarmWindowCommand { get => m_ShowAlarmWindowCommand; }
        public IRelayCommand AddAlarmCommand { get => m_AddAlarmCommand; }
        public IRelayCommand<string> DeleteAlarmCommand { get => m_DeleteAlarmCommand; }
        public bool IsIdle { get => m_IsIdle; private set => SetProperty(ref m_IsIdle, value); }
        public bool ShouldQueryRates { get => m_ShouldQueryRates; private set => SetProperty(ref m_ShouldQueryRates, value); }
        public ObservableCollection<ExchangeRate> ExchangeRates { get => m_ExchangeRates; private set => SetProperty(ref m_ExchangeRates, value); }
        public ObservableCollection<Alarm> AlarmList { get => m_AlarmList; private set => SetProperty(ref m_AlarmList, value); }
        public AlarmOperator SelectedAlarmOperator { get => m_SelectedAlarmOperator; set => SetProperty(ref m_SelectedAlarmOperator, value); }

        public DateTime LastUpdate { get => m_LastUpdate; private set => SetProperty(ref m_LastUpdate, value); }
        public MainViewModel(IWebService webService, IPreferenceService preferenceService, IWindowService windowService)
        {
            m_WebService = webService;
            m_PreferenceService = preferenceService;
            m_WindowService = windowService;

            m_LoadPreferencesCommand = new AsyncRelayCommand(LoadPreferences, LoadPreferencesCanExecute);
            m_ShowAlarmWindowCommand = new RelayCommand(ShowAlarmWindow);
            m_AddAlarmCommand = new RelayCommand(AddAlarm);
            m_DeleteAlarmCommand = new RelayCommand<string?>(DeleteAlarm);

            m_UpdateTimer = new(QueryExchangeRates, null, Timeout.Infinite, Timeout.Infinite);
        }


        private async Task LoadPreferences()
        {
            IsIdle = false;
            ExchangeRates = (ObservableCollection<ExchangeRate>)await m_PreferenceService.LoadRateList();
            SelectedAlarmExchange = ExchangeRates.First();

            AlarmList = (ObservableCollection<Alarm>)await m_PreferenceService.LoadAlarms();
            IsIdle = true;
        }

        private void AddAlarm()
        {
            if (SelectedAlarmExchange != null)
                AlarmList.Add(new Alarm(SelectedAlarmExchange.Code, SelectedAlarmValue, SelectedAlarmOperator));

            m_PreferenceService.SaveAlarms(AlarmList);
        }

        private void DeleteAlarm(string? alarmId)
        {
            AlarmList.Remove(AlarmList.First(alarm => alarm.Id == alarmId));
            m_PreferenceService.SaveAlarms(AlarmList);
        }

        private void ShowAlarmWindow() => m_WindowService.ShowAlarmSettingsWindow();

        private void QueryExchangeRates(object? _ = null) => ShouldQueryRates = true;

        public async Task OnExchangeRatesQueried(string sourceHTML)
        {
            ShouldQueryRates = false;
            await m_WebService.UpdateRates(sourceHTML, ExchangeRates);
            await CheckAlarms();
            LastUpdate = DateTime.Now;
        }

        public void SourcePageLoaded() => m_UpdateTimer.Change(5000, c_UpdateTimerPeriod);

        private async Task CheckAlarms()
        {
            StringBuilder sb = new();
            await Task.Run(() =>
            {

                for (int i = 0; i < AlarmList.Count; i++)
                {
                    Alarm alarm = AlarmList[i];
                    if (!alarm.IsEnabled) continue;
                    ExchangeRate? rate = ExchangeRates.FirstOrDefault(rate => rate.Code == alarm.Code);
                    if (rate == null) continue;

                    if (alarm.AlarmOperator == AlarmOperator.GreaterThan && rate.NewBuying > alarm.Value)
                    {
                        sb.AppendLine($"{rate.Code} alış fiyatı {alarm.Value} TL değerinden büyük.");
                        alarm.IsEnabled = false; // disable alarm after it is triggered
                    }
                    else if (alarm.AlarmOperator == AlarmOperator.LessThan && rate.NewBuying < alarm.Value)
                    {
                        sb.AppendLine($"{rate.Code} alış fiyatı {alarm.Value} TL değerinden küçük.");
                        alarm.IsEnabled = false; // disable alarm after it is triggered
                    }
                }

                if (sb.Length > 0)
                    m_WindowService.ShowAlarm("Alarm", sb.ToString());

            });

        }

        #region Command States
        private bool LoadPreferencesCanExecute() => m_IsIdle;
        #endregion

        #region Fields
        private bool m_IsIdle = true;
        private bool m_AlwaysOnTop = false;
        private bool m_ShouldQueryRates = false;
        private DateTime m_LastUpdate;
        private AlarmOperator m_SelectedAlarmOperator;
        private ExchangeRate? m_SelectedAlarmExchange;
        private double m_SelectedAlarmValue = 0.5;

        private readonly IWebService m_WebService;
        private readonly IPreferenceService m_PreferenceService;
        private readonly IWindowService m_WindowService;

        private ObservableCollection<ExchangeRate> m_ExchangeRates = new();

        private readonly IAsyncRelayCommand m_LoadPreferencesCommand;
        private readonly IRelayCommand m_ShowAlarmWindowCommand;
        private readonly IRelayCommand m_AddAlarmCommand;
        private readonly IRelayCommand<string> m_DeleteAlarmCommand;

        private readonly Timer m_UpdateTimer;
        private const int c_UpdateTimerPeriod = 15000; // 15 seconds

        private ObservableCollection<Alarm> m_AlarmList = new();
        #endregion
    }
}
