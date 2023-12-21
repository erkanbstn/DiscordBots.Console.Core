using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Common.QuestionHandler;
using DcBot.Data;
using DcBot.Data.Datas;
using DcBot.Data.Interfaces;
using DcBot.GeoUGuard.General;
using DcBot.GeoUGuard.Handler;
using DcBot.Service.Interfaces;
using DcBot.Service.Services;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DcBot.GeoUGuard.StartUp
{
    public class ConfigureStartUp
    {
        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            var libraryDirection = Path.GetDirectoryName(typeof(MessageControl).Assembly.Location);
            var configuration = new ConfigurationBuilder()
               .SetBasePath(libraryDirection)
               .AddJsonFile("appsettings.json")
               .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddSingleton<InitializeBot>();
            serviceCollection.AddSingleton<BotEvents>();
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
                (typeof(IPrefixControl), typeof(PrefixControl)),
                (typeof(IQuestionControl), typeof(QuestionControl)),
                (typeof(IMessageControl), typeof(MessageControl)),
                (typeof(IPermissionControl), typeof(PermissionControl)),
            };

            foreach (var (interfaceType, implementationType) in scopedServices)
            {
                serviceCollection.AddScoped(interfaceType, implementationType);
            }
        }
    }
}
