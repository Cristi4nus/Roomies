using Microsoft.Maui.Controls.Shapes;
using Roomies;

namespace Roomies
{
    public partial class MainPage : ContentPage
    {
        private Membru _user;
        private FiltreMembri _filtre;
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(Membru user) : this()
        {
            _user = user;
            BindingContext = _user;
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (_user == null)
            {
                await DisplayAlertAsync("Eroare", "Acest utilizator nu mai exista", "oke");
                return;
            }
            await Navigation.PushAsync(new ProfilePage(_user, true));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshUsers();
        }

        private async void RefreshUsers()
        {
            var db = ServiceHelper.GetService<DatabaseService>();
            var users = await db.GetAllMembriAsync();

            if (_user != null)
            {
                var refreshedUser = await db.GetMembruByIdAsync(_user.Id);
                if (refreshedUser != null)
                {
                    _user = refreshedUser;
                    BindingContext = _user;
                }
            }
            if (_filtre != null)
            {
                if (!string.IsNullOrEmpty(_filtre.Gen))
                    users = users.Where(u => u.Gen == _filtre.Gen).ToList();

                if (!string.IsNullOrEmpty(_filtre.ZonaPreferata))
                    users = users.Where(u => u.ZonaPreferata == _filtre.ZonaPreferata).ToList();

                if (_filtre.BugetMin.HasValue)
                    users = users.Where(u => u.BugetMaxim >= _filtre.BugetMin &&
                                             u.BugetMaxim <= _filtre.BugetMax).ToList();

                if (!string.IsNullOrEmpty(_filtre.StilDeViata))
                    users = users.Where(u => u.StilDeViata == _filtre.StilDeViata).ToList();

                if (!string.IsNullOrEmpty(_filtre.PreferintaDeTrai))
                    users = users.Where(u => u.PreferinteDeTrai != null &&
                                             u.PreferinteDeTrai.Contains(_filtre.PreferintaDeTrai))
                                 .ToList();
            }
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
            var filterPage = new PaginaFiltre(_user);
            await Navigation.PushAsync(filterPage);

            filterPage.Disappearing += (s, args) =>
            {
                _filtre = filterPage.Filtre;
                RefreshUsers();
            };
        }


        private async void OnNotificationsClicked(object sender, EventArgs e)
        {
            if (_user == null)
            {
                await DisplayAlertAsync("Eroare", "Nu exista utilizatorul curent", "OK");
                return;
            }
            await Navigation.PushAsync(new NotificationsPage(_user));
        }
        private View CreateUserCard(Membru user)
        {
            var card = new UserCard();
            card.LoadUser(user, "Add friend");

            var db = ServiceHelper.GetService<DatabaseService>();

            card.CardTapped += async (s, e) =>
            {
                await Navigation.PushAsync(new ProfilePage(user, false));
            };

            card.ActionClicked += async (s, e) =>
            {
                if (await db.AreFriendsAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Info", "Sunteți deja prieteni!", "OK");
                    return;
                }

                if (await db.HasReversePendingRequestAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Atentie",
                        "Acest utilizator ți-a trimis deja o cerere de prietenie! Verifică notificările.",
                        "OK");
                    return;
                }
                if (await db.HasPendingRequestAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Atentie",
                        "Ai deja o cerere trimisă către acest utilizator!",
                        "OK");
                    return;
                }

                string mesaj = await DisplayPromptAsync(
                    "Trimite cerere",
                    "Scrie un mesaj scurt pentru utilizator:",
                    "Trimite",
                    "Anulează",
                    "Atentie! Asta îți este singurul mesaj! Fă-l să conteze!",
                    maxLength: 200
                );

                if (mesaj == null)
                    return;

                await db.SendFriendRequestWithNotificationAsync(_user.Id, user.Id, mesaj);

                await DisplayAlertAsync("Succes", "Cererea a fost trimisă!", "OK");
            };

            return card;
        }
        private async void OnFriendListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaPrieteni(_user));
        }

    }
}
