using Microsoft.Extensions.Logging;

namespace Roomies
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
#if ANDROID
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnableJS", (handler, view) =>
            {
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
            });
#endif

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseService>(s =>
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "roomies.db3");
                return new DatabaseService(dbPath);
            });

            builder.Services.AddSingleton<UserLoginPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<ServiciuChat>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}