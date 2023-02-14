using LudoGame;
using LudoLib.Enums;
using LudoLib.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

// Builder Setup

var builder = new ConfigurationBuilder(); // setting up the configuration for a .NET Core application.

// Establish a connection to the configuration file.
builder.SetBasePath(Directory.GetCurrentDirectory()) // sets the base path of the configuration files.
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // load the JSON file to the configuration object.
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables();

// Serilog Setup

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext() // extra information to our logging.
    .ReadFrom.Configuration(builder.Build()) // read configuration from resulting build e.g, appsettings file
    .WriteTo.Console()
    .CreateLogger();

// Host Setup

var host = Host.CreateDefaultBuilder()
    .UseSerilog()
    .ConfigureServices((ctx, services) =>
    {
        services.AddTransient<IApplication, Application>();
    })
    .Build();

var app = ActivatorUtilities.CreateInstance<Application>(host.Services);
app.Run();

IBoard fourPlayerBoard = Factory.CreateFourPlayerBoard();
fourPlayerBoard.AddPlayer("A", BoardLayer.First);
fourPlayerBoard.AddPlayer("B", BoardLayer.Second);
fourPlayerBoard.AddPlayer("C", BoardLayer.Third);
fourPlayerBoard.AddPlayer("D", BoardLayer.Fourth);

IGenerator generator = Factory.CreateFourPlayerLudoGenerator(fourPlayerBoard);
generator.PlayGame();
