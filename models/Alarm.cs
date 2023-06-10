using CommunityToolkit.Mvvm.ComponentModel;

namespace DovizKuru.models
{
    internal class Alarm : ObservableObject
    {
        public string Id { get; private set; }
        public string Code { get; private set; }
        public double Value { get; private set; }
        public AlarmOperator AlarmOperator{ get; private set; }
        public bool IsEnabled { get => m_IsEnabled; set => SetProperty(ref m_IsEnabled, value); }
        public Alarm(string code, double value, AlarmOperator alarmOperator)
        {
            Id = System.Guid.NewGuid().ToString();
            Code = code;
            Value = value;
            AlarmOperator = alarmOperator;
        }

        private bool m_IsEnabled = true;
    }
}
