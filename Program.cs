using WebsiteStatus;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning).Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(@"C:\temp\workerservice\LogFile.txt")
    .CreateLogger();

try
{
    Log.Information("Starting up the service");
    IHost host = Host.CreateDefaultBuilder(args).
        UseWindowsService()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
    .UseSerilog()
    .Build();

    await host.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "There was a problem starting the service");
    return;
}
finally
{
    Log.CloseAndFlush();
}