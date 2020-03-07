using System.Threading.Tasks;
using GitHub;
using Microsoft.Extensions.Logging;
using Scientist.Publishers.Shared;

namespace Scientist.Publishers.ILogger
{
    public class ILoggerResultPublisher : IResultPublisher
    {
        private readonly ILogger<ILoggerResultPublisher> _logger;

        public ILoggerResultPublisher(ILogger<ILoggerResultPublisher> logger) => _logger = logger;
        
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            _logger.LogInformation("{@experiment}", new PublishableResult<T, TClean>(result));
            return Task.CompletedTask;
        }
    }
}