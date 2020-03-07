using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub;
using Serilog;

namespace Scientist.Publishers.Serilog
{
    public class SerilogResultPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Log.Information("{@experiment}", new SerilogResult<T, TClean>(result));
            return Task.CompletedTask;
        }
        
        private class SerilogResult<T, TClean>
        {
            public Guid ResultId => Guid.NewGuid();
            public string Timestamp => DateTime.UtcNow.ToString("O");
            public string ExperimentName { get; set; }
            public ResultSummary Results { get; set; }
            public IReadOnlyDictionary<string, dynamic> Context { get; set; }
            public Control<T, TClean> Control { get; set; }
            public IEnumerable<Candidate<T, TClean>> Candidates { get; set; }
            
            public SerilogResult(Result<T, TClean> result)
            {
                ExperimentName = result.ExperimentName;
                
                Context = result.Contexts;
                
                Results = new ResultSummary(result.Candidates.Count,
                    result.MismatchedObservations.Count,
                    result.IgnoredObservations.Count);
                
                Control = new Control<T, TClean>(result.Control);
                
                Candidates = result.Candidates
                    .Select(candidate =>
                        new Candidate<T, TClean>(result.MismatchedObservations, result.IgnoredObservations, candidate))
                    .ToList();
            }
        }

        private class ResultSummary
        {
            public int Matched { get; set; }
            public int Mismatched { get; set; }
            public int Ignored { get; set; }

            public ResultSummary(int numCandidates, int numMismatched, int numIgnored)
            {
                Matched = numCandidates - numMismatched - numIgnored;
                Mismatched = numMismatched;
                Ignored = numIgnored;
            }
        }

        private class Control<T, TClean>
        {
            public TimeSpan Duration { get; set; }
            public TClean Value { get; set; }

            public Control(Observation<T, TClean> control)
            {
                Duration = control.Duration;
                Value = control.CleanedValue;
            }
        }

        private class Candidate<T, TClean>
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
                Result = GetCandidateResult(mismatched, ignored, candidate);
            }
        }
        
        private static string GetCandidateResult<T, TClean>(IReadOnlyList<Observation<T, TClean>> mismatched,
            IReadOnlyList<Observation<T, TClean>> ignored, Observation<T, TClean> candidate)
        {
            if (ignored.Any(observation => observation.Name == candidate.Name))
                return "ignored";
            
            return mismatched.Any(observation => observation.Name == candidate.Name) ? "mismatch" : "match";
        }
    }

}