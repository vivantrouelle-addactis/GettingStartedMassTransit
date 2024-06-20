using MassTransit;

namespace GettingStartedMassTransit.Web
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly ILogger<BackgroundWorker> _logger;
        readonly IBus _bus;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                 
                await _bus.Publish(new Message { Text = $"The time is {now}" });

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
