using Microsoft.Extensions.Configuration;

namespace WebsiteStatus
{
    //this is a worker to check whether my Varuog  is up and running or not.
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IConfiguration _configuration;

        private HttpClient _httpClient;

        // Enum of delay duration
        private enum DelayDuration
        {
            FiveSeconds = 5000,
            TenSeconds = 10000,
            FifteenSeconds = 15000
        }

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient();
            _logger.LogInformation("The service worker for checking varuogm  has started.... with delay of {0} ms", (int)DelayDuration.FiveSeconds);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient.Dispose();
            //FAQ and TIP: this stopping message wont be registered or seen if we stop it thorugh visual studio but
            //will be reflected if its run and stopped from out place running as a service
            _logger.LogInformation("The service worker for checking varuogm  has stopped....");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string websiteUrl = _configuration.GetValue<string>("WebsiteUrl");

                var result = await _httpClient.GetAsync(websiteUrl);

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Varuog.xyz up and running with Status code: {StatusCode}", result.StatusCode);
                }
                else
                {
                    _logger.LogError("The website Varuog.xyz is down or needs renewal. Status code: {StatusCode}", result.StatusCode);
                }

                await Task.Delay((int)DelayDuration.FiveSeconds, stoppingToken);
            }
        }
    }
}
