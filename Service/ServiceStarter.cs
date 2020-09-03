using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tayko.co.Service
{
    public class ServiceStarter<T> : IHostedService
        where T : IHostedService
    {
        private readonly T _backgroundService;

        public ServiceStarter(T backgroundService)
        {
            this._backgroundService = backgroundService;
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