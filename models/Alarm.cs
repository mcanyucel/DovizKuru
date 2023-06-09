namespace DovizKuru.models
{
    internal record struct Alarm
    {
        public string Code { get; private set; }
        public double Value { get; private set; }
        public bool IsHigh { get; private set; }
        public bool IsEnabled { get; set; } = true;

        public Alarm(string code, double value, bool isHigh)
        {
            Code = code;
            Value = value;
            IsHigh = isHigh;
        }
    }
}
