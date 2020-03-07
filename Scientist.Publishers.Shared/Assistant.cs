using System;
using System.Collections.Generic;
using System.Linq;
using GitHub;

namespace Scientist.Publishers.Shared
{
    public static class Assistant
    {
        public static string GetCandidateResult<T, TClean>(IReadOnlyList<Observation<T, TClean>> mismatched,
            IReadOnlyList<Observation<T, TClean>> ignored, Observation<T, TClean> candidate)
        {
            if (ignored.Any(observation => observation.Name == candidate.Name))
                return "ignored";
            
            return mismatched.Any(observation => observation.Name == candidate.Name) ? "mismatch" : "match";
        }
    }
}