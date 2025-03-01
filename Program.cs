// docker pull redis

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.ConfigureServices(s => {
    s.AddHybridCache();
    s.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "127.0.0.1:6379";
    });
    s.AddScoped<ICompanyServicesProvider, CompanyServicesProvider>();
});
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using (IHost host = builder.Build())
    {
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        var provider = host.Services.GetRequiredService<ICompanyServicesProvider>();
    
        lifetime.ApplicationStarted.Register(async () =>
        {
            Console.WriteLine("Started");
            Console.ReadLine();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var count = 0;
            while (true)
            {
                var companyId = ++count % 2 + 1;
                watch.Restart();
                IEnumerable<string?> services = null;
                try
                {
                    services = await provider.GetServicesForCompanyAsync(companyId, default);
                } catch {}
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if (services != null)
                {
                    Console.WriteLine($"Services:   {string.Join(",", services)}");
                }
                Console.WriteLine($"Elapsed:    {elapsedMs:N0} ms");
                Console.ReadLine();
            }
        });
        lifetime.ApplicationStopping.Register(() =>
        {
            Console.WriteLine("Stopping firing");
            Console.WriteLine("Stopping end");
        });
        lifetime.ApplicationStopped.Register(() =>
        {
            Console.WriteLine("Stopped firing");
            Console.WriteLine("Stopped end");
        });
    
        host.Start();
    
        // Listens for Ctrl+C.
        host.WaitForShutdown();
    }
