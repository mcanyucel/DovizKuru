namespace DovizKuru.models
{
    internal record struct Alarm
    {
        public string Id { get; private set; }
        public string Code { get; private set; }
        public double Value { get; private set; }
        public AlarmOperator AlarmOperator{ get; private set; }
        public bool IsEnabled { get; set; } = true;

        public Alarm(string code, double value, AlarmOperator alarmOperator)
        {
            Id = System.Guid.NewGuid().ToString();
            Code = code;
            Value = value;
            AlarmOperator = alarmOperator;
        }
    }
}
