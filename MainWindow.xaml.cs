using DovizKuru.viewmodels;
using System;
using System.Text.RegularExpressions;

namespace DovizKuru
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel? m_MainViewModel;
        public MainWindow()
        {
            m_MainViewModel = App.Current.ServiceProvider.GetService(typeof(MainViewModel)) as MainViewModel;

            DataContext = m_MainViewModel;
            m_MainViewModel!.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.ShouldQueryRates) && m_MainViewModel.ShouldQueryRates)
                    QueryExchangeRates();
            };
            InitializeComponent();
        }

        private void QueryExchangeRates()
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                // the value can be get directly with javascript, but it is not reliable
                string html = ProcessHTML(await webView.ExecuteScriptAsync("document.documentElement.outerHTML;"));
                m_MainViewModel?.OnExchangeRatesQueried(html);
            });
        }

        private async void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);
            // the value can be get directly with javascript, but it is not reliable
            string html = ProcessHTML(await webView.ExecuteScriptAsync("document.documentElement.outerHTML;"));
            m_MainViewModel?.OnExchangeRatesQueried(html);
            m_MainViewModel?.SourcePageLoaded();
        }

        private static string ProcessHTML(string html) => Regex.Unescape(html).AsSpan()[1..^1].ToString();
    }
}
