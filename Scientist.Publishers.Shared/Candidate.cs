using System;
using System.Collections.Generic;
using GitHub;

namespace Scientist.Publishers.Shared
{
    public class Candidate<T, TClean>
    {
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public string Result { get; set; }
        public TClean Value { get; set; }

        public Candidate(IReadOnlyList<Observation<T, TClean>> mismatched,
            IReadOnlyList<Observation<T, TClean>> ignored, Observation<T, TClean> candidate)
        {
            Name = candidate.Name;
            Duration = candidate.Duration;
            Value = candidate.CleanedValue;
            Result = Assistant.GetCandidateResult(mismatched, ignored, candidate);
        }
    }
}