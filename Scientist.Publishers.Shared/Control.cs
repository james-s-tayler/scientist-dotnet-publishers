using System;
using GitHub;

namespace Scientist.Publishers.Shared
{
    public class Control<T, TClean>
    {
        public TimeSpan Duration { get; set; }
        public TClean Value { get; set; }

        public Control(Observation<T, TClean> control)
        {
            Duration = control.Duration;
            Value = control.CleanedValue;
        }
    }
}