using LazerMusicExporter;
using LazerMusicExporter.Configuration;
using LazerMusicExporter.IO;
using LazerMusicExporter.OsuRealm;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        });

        services.AddHostedService<ExportService>();
        
        services.AddSingleton<IExportSettings, ExportSettings>();
        services.AddSingleton<IOsuRealmFactory, OsuRealmFactory>();
        services.AddTransient<IOsuFiles, OsuFiles>();
        services.AddTransient<ISessionBuilder, SessionBuilder>();
        services.AddTransient<IBeatmapProvider, BeatmapProvider>();
        services.AddTransient<IBeatmapSetExporter, BeatmapSetExporter>();
        services.AddTransient<IBeatmapExporter, BeatmapExporter>();
        services.AddTransient<ITransactionConsumer, TransactionConsumer>();
        services.AddTransient<IFileWriter, WindowsFileWriter>();
        services.AddTransient<IMetadataStringProvider, MetadataStringProvider>();
        services.AddTransient<IBackgroundFileProvider, BackgroundFileProvider>();
        services.AddTransient<IMetadataWriter, AudioMetadataWriter>();
    })
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json");
    })
    .Build();


await host.RunAsync();