using DovizKuru.views;
using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace DovizKuru.services.implementations
{
    internal class WindowService : IWindowService, IDisposable
    {
        public void ShowAlarm(string title, string message)
        {
            new ToastContentBuilder()
                .AddArgument("conversationId", 9813)
                .AddText(title)
                .AddText(message)
                .Show();
        }

        public void ShowAlarmSettingsWindow()
        {
            if (m_AlarmSettingsWindow == null)
            {
                m_AlarmSettingsWindow = new AlarmSettingsWindow();
                m_AlarmSettingsWindow.Closed += (s, e) => m_AlarmSettingsWindow = null;
                m_AlarmSettingsWindow.Show();
            }
            else
                  m_AlarmSettingsWindow.Activate();
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string title, string message)
        {
            throw new NotImplementedException();
        }

        public void ShowPreferenceWindow()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            ToastNotificationManagerCompat.Uninstall();
        }

        #region Fields
        private AlarmSettingsWindow? m_AlarmSettingsWindow;
        #endregion
    }
}
