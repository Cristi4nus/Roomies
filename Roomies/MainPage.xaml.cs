using Microsoft.Maui.Controls.Shapes;
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
            BindingContext = _user;
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (_user == null)
            {
                await DisplayAlertAsync("Eroare", "Acest utilizator nu exista", "oke");
                return;
            }
            await Navigation.PushAsync(new ProfilePage(_user, true));
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var db = ServiceHelper.GetService<DatabaseService>();
            if (_user != null)
            {
                var refreshedUser = await db.GetMembruByIdAsync(_user.Id);
                if (refreshedUser != null)
                {
                    _user = refreshedUser;
                    BindingContext = _user;
                }
            }
            var users = await db.GetAllMembriAsync();
            UsersContainer.Children.Clear();
            foreach (var user in users)
            {
                if (_user != null && user.Id == _user.Id)
                    continue;
                var card = CreateUserCard(user);
                UsersContainer.Children.Add(card);
            }
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

            var avatarSource = string.IsNullOrWhiteSpace(user.Avatar)
                ? "utilizator.png"
                : user.Avatar;

            var avatar = new Image
            {
                Source = avatarSource,
                HeightRequest = 60,
                WidthRequest = 60,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0)
            };

            var nameLabel = new Label
            {
                Text = $"{user.Nume} {user.Prenume}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 24,
                TextColor = Colors.Black
            };

            var detailsLabel = new Label
            {
                Text ="Zona preferata:" + $"{user.ZonaPreferata}" + "\n"+"Bugetul maxim in lei:" + $"{user.BugetMaxim}" + "\n" +"Facultatea:"+ $"{user.Facultate}" + "\n"+"Stilul de viata:" + $"{user.StilDeViata}",
                FontSize = 18,
                TextColor = Colors.Black
            };

            var textStack = new VerticalStackLayout
            {
                Children = { nameLabel, detailsLabel },
                VerticalOptions = LayoutOptions.Start
            };

            var row = new HorizontalStackLayout
            {
                Spacing = 5,
                VerticalOptions = LayoutOptions.Start,
                Children = { avatar, textStack }
            };
            var addFriendButton = new Button
            {
                Text = "Add friend",
                BackgroundColor = Color.FromArgb("#4DA3FF"),
                TextColor = Colors.White,
                CornerRadius = 8,
                Padding = new Thickness(5, 5),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            addFriendButton.Clicked += async (s, e) =>
            {
                string mesaj = await DisplayPromptAsync(
                    "Trimite cerere",
                    "Scrie un mesaj scurt pentru utilizator:",
                    "Trimite",
                    "Anulează",
                    "Atentie! Asta iti este singurul mesaj! Fa-l sa conteze!",
                    maxLength: 200
                );

                if (mesaj == null)
                    return;

                var db = ServiceHelper.GetService<DatabaseService>();

                if (await db.HasPendingRequestAsync(_user.Id, user.Id))
                {
                    await DisplayAlertAsync("Atenție", "Ai deja o cerere trimisă acestui utilizator!", "OK");
                    return;
                }

                await db.SendFriendRequestWithNotificationAsync(_user.Id, user.Id, mesaj);

                await DisplayAlertAsync("Succes", "Cererea a fost trimisă!", "OK");
            };
            var border = new Border
            {
                Stroke = Color.FromArgb("#CCCCCC"),
                StrokeThickness = 1,
                Background = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 5,
                Margin = new Thickness(0, 0, 0, 10),

                Content = new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        row,
                        addFriendButton
                    }
                }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new ProfilePage(user, false));
            };

            border.GestureRecognizers.Add(tap);

            return border;
        }
    }
}
