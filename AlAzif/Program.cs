// See https://aka.ms/new-console-template for more information

using AlAzif;
using AlAzif.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services
            .AddOptions<AlAzifConfig>()
            .Bind(ctx.Configuration.GetSection("AlAzif"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHostedService<AlAzifBot>();
    })
    .Build();

app.Run();