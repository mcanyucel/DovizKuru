using DovizKuru.viewmodels;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                string html = await webView.ExecuteScriptAsync("document.documentElement.outerHTML;");
                html = Regex.Unescape(html);
                html = html.AsSpan()[1..^1].ToString();
                m_MainViewModel?.OnExchangeRatesQueried(html);
            });
        }

        private async void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            string html = await webView.ExecuteScriptAsync("document.documentElement.outerHTML;");
            html = Regex.Unescape(html);
            html = html.AsSpan()[1..^1].ToString();
            m_MainViewModel?.OnExchangeRatesQueried(html);
            m_MainViewModel?.SourcePageLoaded();
        }
    }
}
