using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DovizKuru.services.implementations
{
    internal class LogService : ILogService
    {
        public void LogError(string message)
        {
            Task.Run(async () =>
            {
                await m_LogFileSemaphore.WaitAsync();
                try
                {
                    await File.AppendAllTextAsync(m_LogFilePath, $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] [ERROR] {message}\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while writing to log file: {e.Message}");
                }
            });
        }

        private readonly string m_LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DovizKuru", "DovizKuru.log");
        private readonly SemaphoreSlim m_LogFileSemaphore = new(1, 1);
    }
}
