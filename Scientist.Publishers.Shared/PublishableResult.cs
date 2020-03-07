using System;
using System.Collections.Generic;
using System.Linq;
using GitHub;

namespace Scientist.Publishers.Shared
{
    public class PublishableResult<T, TClean>
    {
        public Guid ResultId => Guid.NewGuid();
        public string Timestamp => DateTime.UtcNow.ToString("O");
        public string ExperimentName { get; set; }
        public Findings Results { get; set; }
        public IReadOnlyDictionary<string, dynamic> Context { get; set; }
        public Control<T, TClean> Control { get; set; }
        public IEnumerable<Candidate<T, TClean>> Candidates { get; set; }
            
        public PublishableResult(Result<T, TClean> result)
        {
            ExperimentName = result.ExperimentName;
                
            Context = result.Contexts;
                
            Results = new Findings(result.Candidates.Count,
                result.MismatchedObservations.Count,
                result.IgnoredObservations.Count);
                
            Control = new Control<T, TClean>(result.Control);
                
            Candidates = result.Candidates
                .Select(candidate =>
                    new Candidate<T, TClean>(result.MismatchedObservations, result.IgnoredObservations, candidate))
                .ToList();
        }
    }
}