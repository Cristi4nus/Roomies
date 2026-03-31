namespace Roomies
{
    public partial class PaginaFiltre : ContentPage
    {
        public FiltreMembri Filtre { get; private set; } = new();
        private Membru _user;
        private TaskCompletionSource<FiltreMembri> _tcs;


        public PaginaFiltre(Membru user)
        {
            InitializeComponent();
            _user = user;
            _tcs = new TaskCompletionSource<FiltreMembri>();
        }
        public Task<FiltreMembri> GetFiltreAsync()
        {
            return _tcs.Task;
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            Filtre.Gen = GenPicker.SelectedItem as string;
            Filtre.ZonaPreferata = ZonaPicker.SelectedItem as string;
            Filtre.StilDeViata = StilPicker.SelectedItem as string;
            Filtre.PreferintaDeTrai = PreferintePicker.SelectedItem as string;

            if (BugetPicker.SelectedItem is string buget)
            {
                switch (buget)
                {
                    case "0 - 250": Filtre.BugetMin = 0; Filtre.BugetMax = 250; break;
                    case "250 - 500": Filtre.BugetMin = 250; Filtre.BugetMax = 500; break;
                    case "500 - 750": Filtre.BugetMin = 500; Filtre.BugetMax = 750; break;
                    case "750 - 1000": Filtre.BugetMin = 750; Filtre.BugetMax = 1000; break;
                    case "1000+": Filtre.BugetMin = 1001; Filtre.BugetMax = int.MaxValue; break;
                }
            }

            _tcs.TrySetResult(Filtre);
            await Navigation.PopAsync();
        }

        private async void OnAddAlarmClicked(object sender, EventArgs e)
        {
            var db = ServiceHelper.GetService<DatabaseService>();

            var alarm = new AlarmaFiltre
            {
                UserId = _user.Id,
                Gen = GenPicker.SelectedItem?.ToString(),
                Zona = ZonaPicker.SelectedItem?.ToString(),
                Buget = BugetPicker.SelectedItem?.ToString(),
                StilViata = StilPicker.SelectedItem?.ToString(),
                Preferinte = PreferintePicker.SelectedItem?.ToString()
            };

            await db.AddAlarmAsync(alarm);

            await DisplayAlertAsync("Succes", "Alarma a fost salvată!", "OK");
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (!_tcs.Task.IsCompleted)
                _tcs.TrySetResult(null);
        }
        private async void OnResetClicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult(null);
            await Navigation.PopAsync();
        }
    }
}
