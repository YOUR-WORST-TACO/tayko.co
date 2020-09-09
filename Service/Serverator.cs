using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tayko.co.Service
{
    public class Serverator<T> : IHostedService
        where T : IHostedService
    {
        private readonly T _backgroundService;

        public Serverator(T backgroundService)
        {
            _backgroundService = backgroundService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _backgroundService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _backgroundService.StopAsync(cancellationToken);
        }
    }
}