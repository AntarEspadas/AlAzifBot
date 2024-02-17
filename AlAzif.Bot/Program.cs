// See https://aka.ms/new-console-template for more information

using AlAzif.Bot.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAlAzif();

var app = builder.Build();

app.Run();