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
            var email = InputEmail.Text;
            var parola = InputParola.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(parola))
            {
                await DisplayAlertAsync("Eroare", "Completează toate câmpurile.", "OK");
                return;
            }

            var membru = await _db.GetMembruByEmailAsync(email);

            if (membru == null)
            {
                await DisplayAlertAsync("Eroare", "Utilizatorul nu există.", "OK");
                return;
            }

            bool ok = PasswordHelper.VerifyPassword(parola, membru.ParolaHash, membru.ParolaSalt);

            if (!ok)
            {
                await DisplayAlertAsync("Eroare", "Parola este incorectă.", "OK");
                return;
            }

            await DisplayAlertAsync("Succes", "Autentificare reușită!", "OK");

            var app = Application.Current;
            if (app?.Windows != null && app.Windows.Count > 0)
            {
                app.Windows[0].Page = new NavigationPage(new MainPage(membru));
            }
            else if (Navigation != null)
            {
                await Navigation.PushAsync(new MainPage(membru));
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Confirmare",
                "Sigur vrei să ieși de pe cont?", "Da", "Nu");

            if (!confirm)
                return;

            var page = ServiceHelper.GetService<LoginPage>();
            await Navigation.PushAsync(page);
        }
    }
}
