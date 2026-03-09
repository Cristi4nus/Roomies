using Roomies;

namespace Roomies
{
    public partial class UserLoginPage : ContentPage
    {
        private readonly DatabaseService _db;

        public UserLoginPage()
        {
            InitializeComponent();
            _db = ServiceHelper.GetService<DatabaseService>();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var membru = await _db.GetMembruByNumePrenumeAsync(InputNume.Text, InputPrenume.Text);

            if (membru == null)
            {
                await DisplayAlert("Eroare", "Utilizatorul nu exista.", "OK");
                return;
            }

            bool ok = PasswordHelper.VerifyPassword(InputParola.Text, membru.ParolaHash, membru.ParolaSalt);

            if (!ok)
            {
                await DisplayAlert("Eroare", "Parola este incorecta.", "OK");
                return;
            }

            await DisplayAlert("Succes", "Autentificare reusita!", "OK");

            Application.Current.MainPage = new NavigationPage(new MainPage(membru));
        }
        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmare",
                "Sigur vrei sa iesi de pe cont?", "Da", "Nu");

            if (!confirm)
                return;
            var page = ServiceHelper.GetService<LoginPage>();
            await Navigation.PushAsync(page);
        }
    }
}
