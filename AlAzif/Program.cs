// See https://aka.ms/new-console-template for more information

using AlAzif;
using AlAzif.Configuration;
using AlAzif.Extensions;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var app = Host.CreateDefaultBuilder(args)
    .UseAlAzif()
    .ConfigureServices(services =>
    {
        services.AddLavalink();
    })
    .Build();

app.Run();