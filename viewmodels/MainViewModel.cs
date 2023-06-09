﻿using AutoUpdater;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DovizKuru.models;
using DovizKuru.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DovizKuru.viewmodels
{
    internal class MainViewModel : ObservableObject
    {
        public bool AlwaysOnTop { get => m_AlwaysOnTop; set => SetProperty(ref m_AlwaysOnTop, value); }
        public bool IsSourcePageLoaded { get => m_IsSourcePageLoaded; set => SetProperty(ref m_IsSourcePageLoaded, value); }
        public bool IsAlarmHistoryOpen { get => m_IsAlarmHistoryOpen; set => SetProperty(ref m_IsAlarmHistoryOpen, value); }
        public bool ShouldReloadPage { get => m_ShouldReloadPage; private set => SetProperty(ref m_ShouldReloadPage, value); }
        public static List<AlarmOperator> AlarmOperators { get => Enum.GetValues(typeof(AlarmOperator)).Cast<AlarmOperator>().ToList(); }
        public ExchangeRate? SelectedAlarmExchange { get => m_SelectedAlarmExchange; set => SetProperty(ref m_SelectedAlarmExchange, value); }
        public double SelectedAlarmValue { get => m_SelectedAlarmValue; set => SetProperty(ref m_SelectedAlarmValue, value); }
        public IAsyncRelayCommand LoadPreferencesCommand { get => m_LoadPreferencesCommand; }
        public IRelayCommand ShowAlarmWindowCommand { get => m_ShowAlarmWindowCommand; }
        public IRelayCommand AddAlarmCommand { get => m_AddAlarmCommand; }
        public IRelayCommand<string> DeleteAlarmCommand { get => m_DeleteAlarmCommand; }
        public IRelayCommand CloseAlarmHistoryCommand { get => m_CloseAlarmHistoryCommand; }
        public IRelayCommand<AlarmItem> RemoveAlarmItemCommand { get => m_RemoveAlarmItemCommand; }
        public bool IsIdle { get => m_IsIdle; private set => SetProperty(ref m_IsIdle, value); }
        public bool ShouldQueryRates { get => m_ShouldQueryRates; private set => SetProperty(ref m_ShouldQueryRates, value); }
        public ObservableCollection<ExchangeRate> ExchangeRates { get => m_ExchangeRates; private set => SetProperty(ref m_ExchangeRates, value); }
        public ObservableCollection<Alarm> AlarmList { get => m_AlarmList; private set => SetProperty(ref m_AlarmList, value); }
        public AlarmOperator SelectedAlarmOperator { get => m_SelectedAlarmOperator; set => SetProperty(ref m_SelectedAlarmOperator, value); }
        public ObservableCollection<AlarmItem> AlarmHistory { get => m_AlarmHistory; }

        public DateTime LastUpdate { get => m_LastUpdate; private set => SetProperty(ref m_LastUpdate, value); }
        public MainViewModel(IWebService webService, IPreferenceService preferenceService, IWindowService windowService, IMediaService mediaService)
        {
            m_WebService = webService;
            m_PreferenceService = preferenceService;
            m_WindowService = windowService;
            m_MediaService = mediaService;

            m_LoadPreferencesCommand = new AsyncRelayCommand(LoadPreferences, LoadPreferencesCanExecute);
            m_ShowAlarmWindowCommand = new RelayCommand(ShowAlarmWindow);
            m_AddAlarmCommand = new RelayCommand(AddAlarm);
            m_DeleteAlarmCommand = new RelayCommand<string?>(DeleteAlarm);
            m_CloseAlarmHistoryCommand = new RelayCommand(() => IsAlarmHistoryOpen = false);
            m_RemoveAlarmItemCommand = new RelayCommand<AlarmItem>(RemoveAlarmItem);

            m_Commands = new IRelayCommand[] { m_LoadPreferencesCommand, m_ShowAlarmWindowCommand, m_AddAlarmCommand, m_DeleteAlarmCommand };
            m_AsyncCommands = new IAsyncRelayCommand[] { m_LoadPreferencesCommand };

            m_UpdateTimer = new(QueryExchangeRates, null, Timeout.Infinite, Timeout.Infinite);
            m_ReloadTimer = new(ReloadSystem, null, Timeout.Infinite, Timeout.Infinite);

            var executingAssemblyName = Assembly.GetExecutingAssembly().GetName();
            m_UpdateEngine = new(executingAssemblyName.Name!, executingAssemblyName.Version!.ToString(), "https://software.mustafacanyucel.com/update");
        }

        private void RemoveAlarmItem(AlarmItem item) => AlarmHistory.Remove(item);

        private async Task LoadPreferences()
        {
            IsIdle = false;
            ExchangeRates = (ObservableCollection<ExchangeRate>)await m_PreferenceService.LoadRateList();
            SelectedAlarmExchange = ExchangeRates.First();

            AlarmList = (ObservableCollection<Alarm>)await m_PreferenceService.LoadAlarms();

            var hasUpdate = await m_UpdateEngine.CheckForUpdateAsync();
            if (hasUpdate)
            {
                var shouldUpdate = m_WindowService.ShowUpdateWindowDialog();
                if (shouldUpdate)
                {
                    var downloaded = await m_UpdateEngine.DownloadAndRunUpdate();
                    if (downloaded)
                        Application.Current.Shutdown();
                    else
                        m_WindowService.ShowMessage("Hata", "Güncelleme indirilemedi.");
                }
            }

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

        public void SourcePageLoaded()
        {
            IsSourcePageLoaded = true;
            ShouldReloadPage = false;
            m_UpdateTimer.Change(5000, c_UpdateTimerPeriod);
            m_ReloadTimer.Change(c_ReloadTimerPeriod, Timeout.Infinite);
        }

        private void ReloadSystem(object? _ = null)
        {
            m_UpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsSourcePageLoaded = false;
            ShouldReloadPage = true;
        }

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
                    if (rate == null || rate.NewBuying == -1) continue;

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
                {
                    var alarmText = sb.ToString().Remove(sb.Length - 2); // remove last new line
                    m_WindowService.ShowAlarm("Alarm", alarmText);
                    m_MediaService.PlayAlarm();
                    App.Current.Dispatcher.Invoke(async () =>
                    {
                        AlarmHistory.Add(new AlarmItem { Time = DateTime.Now, Message = alarmText });
                        UpdateCommandStates();
                        await m_PreferenceService.SaveAlarms(AlarmList);
                    });
                }

            });

        }

        #region Command States
        private void UpdateCommandStates()
        {
            foreach (var command in m_Commands)
                command.NotifyCanExecuteChanged();

            foreach (var command in m_AsyncCommands)
                command.NotifyCanExecuteChanged();
        }
        private bool LoadPreferencesCanExecute() => m_IsIdle;
        #endregion

        #region Fields
        private bool m_IsIdle = true;
        private bool m_IsSourcePageLoaded = false;
        private bool m_AlwaysOnTop = false;
        private bool m_ShouldQueryRates = false;
        private bool m_IsAlarmHistoryOpen = false;
        private bool m_ShouldReloadPage = false;
        private DateTime m_LastUpdate;
        private AlarmOperator m_SelectedAlarmOperator;
        private ExchangeRate? m_SelectedAlarmExchange;
        private double m_SelectedAlarmValue = 0.5;

        private readonly IWebService m_WebService;
        private readonly IPreferenceService m_PreferenceService;
        private readonly IWindowService m_WindowService;
        private readonly IMediaService m_MediaService;

        private ObservableCollection<ExchangeRate> m_ExchangeRates = new();

        private readonly IAsyncRelayCommand m_LoadPreferencesCommand;
        private readonly IRelayCommand m_ShowAlarmWindowCommand;
        private readonly IRelayCommand m_AddAlarmCommand;
        private readonly IRelayCommand<string> m_DeleteAlarmCommand;
        private readonly IRelayCommand m_CloseAlarmHistoryCommand;
        private readonly IRelayCommand<AlarmItem> m_RemoveAlarmItemCommand;
        private readonly IRelayCommand[] m_Commands;
        private readonly IAsyncRelayCommand[] m_AsyncCommands;

        private readonly Timer m_UpdateTimer;
        private readonly Timer m_ReloadTimer;
        private const int c_UpdateTimerPeriod = 15 * 1000; // 15 seconds
        private const int c_ReloadTimerPeriod = 15 * 60 * 1000; // 15 minutes

        private ObservableCollection<Alarm> m_AlarmList = new();
        private readonly ObservableCollection<AlarmItem> m_AlarmHistory = new();

        private readonly UpdateEngine m_UpdateEngine;
        #endregion
    }
}
