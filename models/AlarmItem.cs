using System;

namespace DovizKuru.models
{
    internal record struct AlarmItem
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
