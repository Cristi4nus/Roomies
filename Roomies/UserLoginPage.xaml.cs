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
        }
    }
}
