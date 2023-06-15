namespace DovizKuru.services
{
    internal interface IWindowService
    {
        public void ShowMessage(string title, string message);
        public void ShowError(string message);
        public void ShowPreferenceWindow();
        public void ShowAlarmSettingsWindow();
        public void ShowAlarm(string title, string message);
    }
}
