using DovizKuru.services;
using DovizKuru.services.implementations;
using DovizKuru.viewmodels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace DovizKuru
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ServiceProvider = ConfigureServices();
            this.InitializeComponent();
        }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainViewModel>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IWebService, WebService>();
            services.AddSingleton<IPreferenceService, PreferenceService>();
            return services.BuildServiceProvider();
        }
        public IServiceProvider ServiceProvider { get; }
        public new static App Current => (App)Application.Current;
    }
}
