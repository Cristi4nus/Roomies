using Microsoft.Maui.Controls.Shapes;
using Roomies.Models;

namespace Roomies
{
    public partial class MainPage : ContentPage
    {
        private Membru _user;
        private FiltreMembri _filtre;

        private bool _isActive = false;

        public MainPage(Membru user)
        {
            InitializeComponent();
            _user = user;
            BindingContext = _user;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _isActive = true;

            if (_user == null)
            {
                Application.Current.MainPage = new NavigationPage(new UserLoginPage());
                return;
            }

            RefreshUsers();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isActive = false;
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (!_isActive || _user == null)
                return;

            var page = new ProfilePage(_user, true);

            page.Disappearing += async (s, args) =>
            {
                if (!_isActive) return;

                var db = ServiceHelper.GetService<DatabaseService>();
                var refreshed = await db.GetMembruByIdAsync(_user.Id);

                if (refreshed != null)
                {
                    _user = refreshed;
                    BindingContext = _user;
                }

                RefreshUsers();
            };

            if (Navigation != null)
                await Navigation.PushAsync(page);
        }

        private async void RefreshUsers()
        {
            if (!_isActive || _user == null)
                return;

            var db = ServiceHelper.GetService<DatabaseService>();
            var users = await db.GetAllMembriAsync();

            if (!_isActive) return;

            var refreshedUser = await db.GetMembruByIdAsync(_user.Id);
            if (refreshedUser != null)
            {
                _user = refreshedUser;
                BindingContext = _user;
            }

            if (_filtre != null)
            {
                if (!string.IsNullOrWhiteSpace(_filtre.Gen))
                    users = users.Where(u =>
                        u.Gen != null &&
                        u.Gen.Equals(_filtre.Gen, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                if (!string.IsNullOrWhiteSpace(_filtre.ZonaPreferata))
                    users = users.Where(u =>
                        u.ZonaPreferata != null &&
                        u.ZonaPreferata
                            .Split(',')
                            .Select(z => z.Trim().ToLower())
                            .Contains(_filtre.ZonaPreferata.ToLower())
                    ).ToList();

                if (_filtre.BugetMin.HasValue && _filtre.BugetMax.HasValue)
                    users = users.Where(u =>
                        u.BugetMaxim != null &&
                        u.BugetMaxim >= _filtre.BugetMin.Value &&
                        u.BugetMaxim <= _filtre.BugetMax.Value
                    ).ToList();

                if (!string.IsNullOrWhiteSpace(_filtre.StilDeViata))
                    users = users.Where(u =>
                        u.StilDeViata != null &&
                        u.StilDeViata.Equals(_filtre.StilDeViata, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                if (!string.IsNullOrWhiteSpace(_filtre.PreferintaDeTrai))
                    users = users.Where(u =>
                        u.PreferinteDeTrai != null &&
                        u.PreferinteDeTrai
                            .Split(',')
                            .Select(p => p.Trim().ToLower())
                            .Contains(_filtre.PreferintaDeTrai.ToLower())
                    ).ToList();
            }

            if (!_isActive) return;
            //golim containerul inainte sa reafisam userii compatibili
            UsersContainer.Children.Clear();

            foreach (var user in users)
            {
                if (_user != null && user.Id == _user.Id)
                    continue;

                UsersContainer.Children.Add(CreateUserCard(user));
            }
        }

        private async void OnFilterClicked(object sender, EventArgs e)
        {
            if (!_isActive || _user == null)
                return;

            var filterPage = new PaginaFiltre(_user);
            await Navigation.PushAsync(filterPage);
            var filtre = await filterPage.GetFiltreAsync();

            _filtre = filtre;
            RefreshUsers();
        }



        private async void OnNotificationsClicked(object sender, EventArgs e)
        {
            if (!_isActive || _user == null)
                return;
            if (Navigation != null)
                await Navigation.PushAsync(new NotificationsPage(_user));
        }

        private View CreateUserCard(Membru user)
        {
            var card = new UserCard();
            card.LoadUser(user, "Cerere prietenie");

            var db = ServiceHelper.GetService<DatabaseService>();

            card.CardTapped += async (s, e) =>
            {
                if (!_isActive) return;

                if (Navigation != null)
                    await Navigation.PushAsync(new ProfilePage(user, false));
            };

            card.ActionClicked += async (s, e) =>
            {
                if (!_isActive) return;

                if (await db.AreFriendsAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Info", "Sunteti deja prieteni", "OK");
                    return;
                }

                if (await db.HasReversePendingRequestAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Atentie","Acest utilizator tia trimis deja o cerere de prietenie! Verifică notificarile.","OK");
                    return;
                }

                if (await db.HasPendingRequestAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Atentie","Ai deja o cerere trimisă către acest utilizator!","OK");
                    return;
                }

                string mesaj = await DisplayPromptAsync(
                    "Trimite cerere",
                    "Scrie un mesaj la cerere",
                    "Trimite",
                    "Anuleaza",
                    "Atentie! Asta iti este singurul mesaj!",
                    maxLength: 200
                );

                if (mesaj == null)
                    return;

                await db.SendFriendRequestWithNotificationAsync(_user.Id, user.Id, mesaj);

                await DisplayAlertAsync("Succes", "Cererea a fost trimisa!", "OK");
            };

            return card;
        }

        private async void OnFriendListClicked(object sender, EventArgs e)
        {
            if (!_isActive || _user == null)
                return;

            if (Navigation != null)
                await Navigation.PushAsync(new ListaPrieteni(_user));
        }
    }
}
