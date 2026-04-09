using Roomies;
using System.Net.Http.Json;

namespace Roomies
{
    public partial class UserLoginPage : ContentPage
    {
        private DatabaseService _db;

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

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                "http://10.0.2.2:5137/api/ControllerAutentificare/login",
                new { Email = email, Parola = parola });

            if (!response.IsSuccessStatusCode)
            {
                var mesaj = await response.Content.ReadAsStringAsync();
                if (mesaj.Contains("EMAIL_NECONFIRMAT"))
                    await DisplayAlertAsync("Cont neconfirmat",
                        "Te rugăm să confirmi adresa de email înainte de a te loga. Verifică inbox-ul.", "OK");
                else
                    await DisplayAlertAsync("Eroare", "Email sau parolă greșită.", "OK");
                return;
            }

            var rezultat = await response.Content.ReadFromJsonAsync<LoginResponse>();
            App.JwtToken = rezultat.token;
            await DisplayAlertAsync("Succes", "Autentificare reușită!", "OK");
            Application.Current.MainPage = new NavigationPage(new MainPage(rezultat.user));
        }

        private void OnCreateAccountClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public Membru user { get; set; }
    }
}
