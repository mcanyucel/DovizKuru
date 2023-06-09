using CommunityToolkit.Mvvm.ComponentModel;

namespace DovizKuru.models
{
    internal class ExchangeRate : ObservableObject
    {
        public string Code { get => m_Code; }
        public string Name { get => m_Name; }
        public double NewBuying { get => m_NewBuying; private set => SetProperty(ref m_NewBuying, value); }
        public double OldBuying { get => m_OldBuying; private set => SetProperty(ref m_OldBuying, value); }
        public double ChangeRate { get => m_ChangeRate; private set => SetProperty(ref m_ChangeRate, value); }
        public double ChangePercentage { get => m_ChangePercentage; private set => SetProperty(ref m_ChangePercentage, value); }
        public string SourceUrl { get => m_SourceUrl; }
        public string XPath { get => m_XPath; }

        public ExchangeRate(string name, string code, string sourceUrl, string xPath)
        {
            m_Name = name;
            m_Code = code;
            m_XPath = xPath;
            m_SourceUrl = sourceUrl;
        }

        public void Update(double newBuying)
        {
            OldBuying = NewBuying;
            NewBuying = newBuying;
            ChangeRate = NewBuying - OldBuying;
            ChangePercentage = ChangeRate / OldBuying * 100;
        }

        #region Fields
        private readonly string m_Code;
        private readonly string m_Name;
        private readonly string m_SourceUrl;
        private readonly string m_XPath;
        private double m_NewBuying;
        private double m_OldBuying;
        private double m_ChangeRate;
        private double m_ChangePercentage;
        #endregion
    }
}
