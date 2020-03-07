namespace Scientist.Publishers.Shared
{
    public class Findings
    {
        public int Matched { get; set; }
        public int Mismatched { get; set; }
        public int Ignored { get; set; }

        public Findings(int numCandidates, int numMismatched, int numIgnored)
        {
            Matched = numCandidates - numMismatched - numIgnored;
            Mismatched = numMismatched;
            Ignored = numIgnored;
        }
    }
}