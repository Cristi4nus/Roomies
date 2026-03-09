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
            var navPage = new NavigationPage();
            var db = ServiceHelper.GetService<DatabaseService>();
            var membri = db.GetAllMembriAsync().Result;

            if (membri.Count == 0)
            {
                navPage.PushAsync(ServiceHelper.GetService<LoginPage>());
            }
            else
            {
                navPage.PushAsync(ServiceHelper.GetService<UserLoginPage>());
            }

            return new Window(navPage);
        }
    }
}
