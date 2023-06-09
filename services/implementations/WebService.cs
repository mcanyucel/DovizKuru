using DovizKuru.models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DovizKuru.services.implementations
{
    internal class WebService : IWebService
    {
        public WebService(ILogService logService)
        {
            m_LogService = logService;

            m_HttpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });
        }

        public async Task UpdateRates(Dictionary<string, List<ExchangeRate>> exchangeRateDictionary)
        {
            
            foreach (string url in exchangeRateDictionary.Keys)
            {
                try
                {
                    var siteHtml = await m_HttpClient.GetStringAsync(url);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(siteHtml);
                    foreach (var exchangeRate in exchangeRateDictionary[url])
                    {
                        try
                        {
                            var valueString = htmlDoc.DocumentNode.SelectSingleNode(exchangeRate.XPath)?.InnerText;
                            if (double.TryParse(valueString, out double valueDouble))
                                exchangeRate.Update(valueDouble);
                            else
                                exchangeRate.Update(-1d);
                        }
                        catch (Exception ex)
                        {
                            m_LogService.LogError($"Error while updating rate {exchangeRate.Name}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_LogService.LogError($"Error while downloading {url}: {ex.Message}");
                }

            }
        }

        #region Fields
        private readonly ILogService m_LogService;

        private readonly HttpClient m_HttpClient;
        #endregion
    }
}
