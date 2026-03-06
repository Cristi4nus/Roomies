using Roomies;

namespace Roomies
{
    public partial class MainPage : ContentPage
    {
        private Membru _user;
        public MainPage()
        {
            InitializeComponent();
        }
        public MainPage(Membru user) : this()
        {
            _user = user;
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (_user == null)
            {
                await DisplayAlert("Eroare", "Nu există utilizator încărcat.", "OK");
                return;
            }

            await Navigation.PushAsync(new ProfilePage(_user));
        }
    }
}
