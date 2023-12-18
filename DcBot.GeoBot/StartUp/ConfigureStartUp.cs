using DcBot.Common.MessageHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Data;
using DcBot.Data.Datas;
using DcBot.Data.Interfaces;
using DcBot.GeoBot.BotCommon;
using DcBot.GeoBot.BotHandler;
using DcBot.Service.Interfaces;
using DcBot.Service.Services;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace DcBot.GeoBot.StartUp
{
    public class ConfigureStartUp
    {
        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            var libraryDirection = Path.GetDirectoryName(typeof(GeoBotCommands).Assembly.Location);
            var configuration = new ConfigurationBuilder()
               .SetBasePath(libraryDirection)
               .AddJsonFile("appsettings.json")
               .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddSingleton<InitializeConfig>();
            serviceCollection.AddSingleton<BotEventHandler>();
            serviceCollection.AddSingleton<MessageControl>();
            serviceCollection.AddSingleton<PrefixControl>();
            serviceCollection.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            }));
            serviceCollection.AddSingleton<OnReadyHandler>();
            serviceCollection.AddScoped<AppDbContext>();
            AddScopedServices(serviceCollection);
            serviceCollection.AddTransient<Program>();

            return serviceCollection.BuildServiceProvider();
        }
        private static void AddScopedServices(IServiceCollection serviceCollection)
        {
            var scopedServices = new List<(Type InterfaceType, Type ImplementationType)>
            {
                (typeof(IDcServerDal), typeof(EFDcServerDal)),
                (typeof(IDcServerService), typeof(DcServerService)),
            };

            foreach (var (interfaceType, implementationType) in scopedServices)
            {
                serviceCollection.AddScoped(interfaceType, implementationType);
            }
        }
    }
}
