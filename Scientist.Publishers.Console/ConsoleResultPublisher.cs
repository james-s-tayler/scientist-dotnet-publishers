using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub;
using Newtonsoft.Json;

namespace Scientist.Publishers.Console
{
    public class ConsoleResultPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            var originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = result.Mismatched ? ConsoleColor.Red : ConsoleColor.Green;

            var resultId = Guid.NewGuid();
            var context = JsonConvert.SerializeObject(result.Contexts);
            var numCandidates = result.Candidates.Count;
            var numMismatched = result.MismatchedObservations.Count;
            var numIgnored = result.IgnoredObservations.Count;
            var numMatched = numCandidates - numMismatched - numIgnored;
            
            var matchStats = new Dictionary<string, int>
            {
                {"matched", numMatched},
                {"mismatched", numMismatched},
                {"ignored", numIgnored}
            };
            
            System.Console.WriteLine($"{resultId} experiment_name: {result.ExperimentName}");
            System.Console.WriteLine($"{resultId} results: {GetValueAsString(matchStats)}");
            System.Console.WriteLine($"{resultId} context: {string.Join(", ", context)}");
            System.Console.WriteLine($"{resultId} control_duration: {result.Control.Duration}");
            System.Console.WriteLine($"{resultId} control_value: {GetValueAsString(result.Control.CleanedValue)}");

            foreach (var candidate in result.Candidates)
            {
                System.Console.WriteLine($"{resultId} candidate_name: {candidate.Name}");
                System.Console.WriteLine($"{resultId} candidate_result: {GetCandidateResult(result.MismatchedObservations, result.IgnoredObservations, candidate)}");
                System.Console.WriteLine($"{resultId} candidate_duration: {candidate.Duration}");
                System.Console.WriteLine($"{resultId} candidate_value: {GetValueAsString(candidate.CleanedValue)}");
            }

            System.Console.ForegroundColor = originalColor;
            return Task.CompletedTask;
        }

        private static string GetCandidateResult<T, TClean>(IReadOnlyList<Observation<T, TClean>> mismatched,
            IReadOnlyList<Observation<T, TClean>> ignored, Observation<T, TClean> candidate)
        {
            if (ignored.Any(observation => observation.Name == candidate.Name))
                return "ignored";
            
            return mismatched.Any(observation => observation.Name == candidate.Name) ? "mismatch" : "match";
        }

        private static string GetValueAsString<T>(T t)
        {
            if (t is ValueType)
            {
                return t.ToString();
            }

            return JsonConvert.SerializeObject(t);
        }
    }
}