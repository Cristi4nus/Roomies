namespace Roomies
{
    public partial class PaginaFiltre : ContentPage
    {
        public FiltreMembri Filtre { get; private set; } = new();

        public PaginaFiltre()
        {
            InitializeComponent();
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
                    case "0 - 250":
                        Filtre.BugetMin = 0;
                        Filtre.BugetMax = 250;
                        break;

                    case "250 - 500":
                        Filtre.BugetMin = 250;
                        Filtre.BugetMax = 500;
                        break;

                    case "500 - 750":
                        Filtre.BugetMin = 500;
                        Filtre.BugetMax = 750;
                        break;

                    case "750 - 1000":
                        Filtre.BugetMin = 750;
                        Filtre.BugetMax = 1000;
                        break;

                    case "1000+":
                        Filtre.BugetMin = 1000;
                        Filtre.BugetMax = int.MaxValue;
                        break;
                }
            }


            await Navigation.PopAsync();
        }
    }
}
