using Roomies;

namespace Roomies
{
    public partial class UserLoginPage : ContentPage
    {
        private readonly DatabaseService _db;

        public UserLoginPage()
        {
            InitializeComponent(); // OBLIGATORIU
            _db = ServiceHelper.GetService<DatabaseService>();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var membru = await _db.GetMembruByNumePrenumeAsync(InputNume.Text, InputPrenume.Text);

            if (membru == null)
            {
                await DisplayAlert("Eroare", "Utilizatorul nu există.", "OK");
                return;
            }

            bool ok = PasswordHelper.VerifyPassword(InputParola.Text, membru.ParolaHash, membru.ParolaSalt);

            if (!ok)
            {
                await DisplayAlert("Eroare", "Parolă incorectă.", "OK");
                return;
            }

            await DisplayAlert("Succes", "Autentificare reușită!", "OK");

            Application.Current.MainPage = new NavigationPage(new MainPage(membru));
        }
        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmare",
                "Sigur vrei să te deloghezi?", "Da", "Nu");

            if (!confirm)
                return;

            // Navigăm la pagina de login
            var page = ServiceHelper.GetService<LoginPage>();
            await Navigation.PushAsync(page);
        }
    }
}
