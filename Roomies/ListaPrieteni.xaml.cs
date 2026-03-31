using Roomies;

namespace Roomies
{
    public partial class ListaPrieteni : ContentPage
    {
        private readonly DatabaseService _db;
        private readonly Membru _user;

        public ListaPrieteni(Membru user)
        {
            InitializeComponent();
            _db = ServiceHelper.GetService<DatabaseService>();
            _user = user;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var friends = await _db.GetFriendsAsync(_user.Id);

            FriendsContainer.Children.Clear();

            foreach (var friend in friends)
            {
                var card = new UserCard();
                card.LoadUser(friend, "Message");

                card.CardTapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new ProfilePage(friend, false));
                };

                card.ActionClicked += async (s, e) =>
                {
                    await Navigation.PushAsync(new PaginaChat(_user, friend));
                };

                FriendsContainer.Children.Add(card);
            }
        }
    }
}
