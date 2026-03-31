namespace Roomies
{
    public partial class App : Application
    {
        public static string JwtToken { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new UserLoginPage());
        }
    }
}
