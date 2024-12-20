using MathGame.Data;
using Microsoft.Extensions.Logging;

namespace MathGame
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "game.db");

            //builder.Services.AddSingleton(s => 
            //  ActivatorUtilities.CreateInstance<GameRepository>(s, dbPath));

            builder.Services.AddSingleton(s =>
            {
                var repository = ActivatorUtilities.CreateInstance<GameRepository>(s, dbPath);
                repository.Init(); // Ensure the table is created during startup
                return repository;
            });

            return builder.Build();
        }
    }
}
