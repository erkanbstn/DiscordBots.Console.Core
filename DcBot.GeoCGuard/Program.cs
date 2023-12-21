using DcBot.Data;
using DcBot.GeoCGuard.General;
using DcBot.GeoCGuard.StartUp;
using Microsoft.Extensions.DependencyInjection;
public class Program
{
    private readonly BotEvents _botEventHandlers;
    public Program(BotEvents botEventHandlers)
    {
        _botEventHandlers = botEventHandlers ?? throw new ArgumentNullException(nameof(botEventHandlers));
    }
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureStartUp.ConfigureServices();

        var program = serviceProvider.GetRequiredService<Program>();

        //using (var scope = serviceProvider.CreateScope())
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //    await context.Database.EnsureCreatedAsync();
        //}

        await program.RunAsync();
        await Task.Delay(-1);
    }

    public async Task RunAsync()
    {
        await _botEventHandlers.InitializeHandlers();
    }
}