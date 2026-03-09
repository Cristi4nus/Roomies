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
                await DisplayAlert("Eroare", "Acest utilizator nu exista", "oke");
                return;
            }

            await Navigation.PushAsync(new ProfilePage(_user));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var db = ServiceHelper.GetService<DatabaseService>();
            if (_user != null)
            {
                var refreshedUser = await db.GetMembruByNumePrenumeAsync(_user.Nume, _user.Prenume);
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
        private View CreateUserCard(Membru user)
        {
            var avatarSource = string.IsNullOrWhiteSpace(user.Avatar)
                ? "utilizator.png"
                : user.Avatar;

            var avatar = new Image
            {
                Source = avatarSource,
                HeightRequest = 70,
                WidthRequest = 70,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0)
            };

            var nameLabel = new Label
            {
                Text = $"{user.Nume} {user.Prenume}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                TextColor = Colors.Black
            };

            var detailsLabel = new Label
            {
                Text = $"{user.Varsta} ani • {user.Facultate}",
                FontSize = 14,
                TextColor = Colors.DarkGray
            };

            var textStack = new VerticalStackLayout
            {
                Children = { nameLabel, detailsLabel },
                VerticalOptions = LayoutOptions.Center
            };

            var frame = new Frame
            {
                CornerRadius = 12,
                Padding = 10,
                BackgroundColor = Color.FromArgb("#FFFFFF"),
                BorderColor = Color.FromArgb("#CCCCCC"),
                HasShadow = true,
                Content = new HorizontalStackLayout
                {
                    Spacing = 15,
                    Children = { avatar, textStack }
                }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new ProfilePage(user));
            };

            frame.GestureRecognizers.Add(tap);

            return frame;
        }

    }
}
