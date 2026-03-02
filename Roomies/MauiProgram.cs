using Microsoft.Extensions.Logging;

namespace Roomies
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

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "roomies.db3");

            builder.Services.AddSingleton(new DatabaseService(dbPath));

            // Pagini cu constructori custom
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<UserLoginPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
