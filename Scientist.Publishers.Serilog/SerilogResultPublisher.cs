using System.Threading.Tasks;
using GitHub;
using Scientist.Publishers.Shared;
using Serilog;

namespace Scientist.Publishers.Serilog
{
    public class SerilogResultPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Log.Information("{@experiment}", new PublishableResult<T, TClean>(result));
            return Task.CompletedTask;
        }
    }
}