namespace Roomies
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // IMPORTANT: NavigationPage pentru PushAsync
            var navPage = new NavigationPage();

            // Luăm serviciul DB
            var db = ServiceHelper.GetService<DatabaseService>();

            // Verificăm dacă există deja un utilizator
            var membri = db.GetAllMembriAsync().Result;

            if (membri.Count == 0)
            {
                // Nu există profil → mergem la pagina de creare profil
                navPage.PushAsync(ServiceHelper.GetService<LoginPage>());
            }
            else
            {
                // Există profil → mergem la pagina de login
                navPage.PushAsync(ServiceHelper.GetService<UserLoginPage>());
            }

            return new Window(navPage);
        }
    }
}
