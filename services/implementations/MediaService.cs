using System;
using System.Windows.Media;

namespace DovizKuru.services.implementations
{
    internal class MediaService : IMediaService
    {
        public MediaService(ILogService logService) => m_LogService = logService;

        public void PlayAlarm()
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    m_MediaPlayer.Open(new Uri(ALARM_SOUND_PATH, UriKind.Relative));
                    m_MediaPlayer.Play();
                });
            }
            catch (Exception ex)
            {
                m_LogService.LogError($"Failed to play alarm file: {ex.Message}");
            }
        }

        #region Fields
        private readonly MediaPlayer m_MediaPlayer = new();
        private const string ALARM_SOUND_PATH = @"assets\bell.wav";
        private readonly ILogService m_LogService;
        #endregion
    }
}
