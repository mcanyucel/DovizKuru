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
        public double ChangeAmount { get => m_ChangeAmount; private set => SetProperty(ref m_ChangeAmount, value); }
        public double ChangePercentage { get => m_ChangePercentage; private set => SetProperty(ref m_ChangePercentage, value); }

        public ExchangeRate(string name, string code)
        {
            m_Name = name;
            m_Code = code;
        }

        public void Update(double newBuying)
        {
            OldBuying = NewBuying;
            NewBuying = newBuying;
            ChangeRate = NewBuying - OldBuying;
            ChangeAmount = ChangeRate * 100;
            ChangePercentage = ChangeAmount / OldBuying;
        }


        #region Fields
        private readonly string m_Code;
        private readonly string m_Name;
        private double m_NewBuying;
        private double m_OldBuying;
        private double m_ChangeRate;
        private double m_ChangeAmount;
        private double m_ChangePercentage;
        #endregion
    }
}
