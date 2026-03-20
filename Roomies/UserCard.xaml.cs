namespace Roomies
{
    public partial class UserCard : ContentView
    {
        public Membru User { get; private set; }

        public event EventHandler ActionClicked;
        public event EventHandler CardTapped;

        public UserCard()
        {
            InitializeComponent();
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => CardTapped?.Invoke(this, EventArgs.Empty);
            RootBorder.GestureRecognizers.Add(tap);
            ActionButton.Clicked += (s, e) => ActionClicked?.Invoke(this, EventArgs.Empty);
        }

        public void LoadUser(Membru user, string buttonText)
        {
            User = user;

            AvatarImage.Source = string.IsNullOrWhiteSpace(user.Avatar)
                ? "utilizator.png"
                : user.Avatar;

            NameLabel.Text = $"{user.Nume} {user.Prenume}";

            DetailsLabel.Text =
                "Gen: " + user.Gen + "\n" +
                "Zona preferata: " + user.ZonaPreferata + "\n" +
                "Bugetul maxim in lei: " + user.BugetMaxim + "\n" +
                "Stilul de viata: " + user.StilDeViata + "\n" +
                "Preferinte:\n" + user.PreferinteDeTrai;

            ActionButton.Text = buttonText;
        }
    }
}
