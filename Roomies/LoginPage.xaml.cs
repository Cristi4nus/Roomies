using Roomies;
using System;
using System.Collections.Generic;

namespace Roomies
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _db;
        private List<CheckBox> zonaCheckboxes = new();
        private List<CheckBox> traiCheckboxes = new();

        public LoginPage()
        {
            InitializeComponent();

            // Luăm serviciul din DI prin helper
            _db = ServiceHelper.GetService<DatabaseService>();

            GenerateZonaCheckboxes();
            GeneratePreferinteCheckboxes();
        }

        private void GenerateZonaCheckboxes()
        {
            foreach (var zona in Optiuni.ZonePreferate)
            {
                var cb = new CheckBox();
                zonaCheckboxes.Add(cb);

                zonaContainer.Children.Add(new HorizontalStackLayout
                {
                    Children =
                    {
                        cb,
                        new Label { Text = zona, VerticalOptions = LayoutOptions.Center }
                    }
                });
            }
        }

        private void GeneratePreferinteCheckboxes()
        {
            foreach (var pref in Optiuni.PreferinteDeTrai)
            {
                var cb = new CheckBox();
                traiCheckboxes.Add(cb);

                traiContainer.Children.Add(new HorizontalStackLayout
                {
                    Children =
                    {
                        cb,
                        new Label { Text = pref, VerticalOptions = LayoutOptions.Center }
                    }
                });
            }
        }

        private async void OnCreateProfileClicked(object sender, EventArgs e)
        {
            // Minimal required fields
            if (string.IsNullOrWhiteSpace(InputNume.Text) ||
                string.IsNullOrWhiteSpace(InputPrenume.Text) ||
                string.IsNullOrWhiteSpace(InputParola.Text))
            {
                await DisplayAlert("Eroare", "Completează numele, prenumele și parola.", "OK");
                return;
            }

            // Validate numeric inputs
            if (!int.TryParse(InputVarsta?.Text, out var varsta))
            {
                await DisplayAlert("Eroare", "Introduceți o vârstă validă.", "OK");
                return;
            }

            if (!int.TryParse(InputBuget?.Text, out var buget))
            {
                await DisplayAlert("Eroare", "Introduceți un buget valid.", "OK");
                return;
            }

            try
            {
                PasswordHelper.CreatePasswordHash(InputParola.Text, out byte[] hash, out byte[] salt);

                var membru = new Membru
                {
                    Nume = InputNume.Text,
                    Prenume = InputPrenume.Text,
                    Varsta = varsta,
                    Gen = pickerGen.SelectedItem?.ToString(),
                    Facultate = InputFacultate.Text,
                    NumarTelefon = InputTelefon.Text,
                    ZonaPreferata = GetSelectedZona(),
                    BugetMaxim = buget,
                    PerioadaDeSedere = InputPerioada.Text,
                    StilDeViata = pickerStil.SelectedItem?.ToString(),
                    PreferinteDeTrai = GetSelectedPreferinte(),
                    Descriere = editorDescriere.Text,
                    Email = InputEmail.Text,
                    ParolaHash = hash,
                    ParolaSalt = salt
                };

                await _db.AddMembruAsync(membru);

                await DisplayAlert("Succes", "Profil creat!", "OK");

                var page = ServiceHelper.GetService<UserLoginPage>();
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                // Log or show friendly message instead of crashing
                await DisplayAlert("Eroare internă", "A intervenit o eroare. Încercați din nou.", "OK");
                // optional: Debug.WriteLine(ex);
            }
        }               

        private string GetSelectedZona()
        {
            var selected = new List<string>();

            for (int i = 0; i < zonaCheckboxes.Count; i++)
                if (zonaCheckboxes[i].IsChecked)
                    selected.Add(Optiuni.ZonePreferate[i]);

            return string.Join(", ", selected);
        }

        private string GetSelectedPreferinte()
        {
            var selected = new List<string>();

            for (int i = 0; i < traiCheckboxes.Count; i++)
                if (traiCheckboxes[i].IsChecked)
                    selected.Add(Optiuni.PreferinteDeTrai[i]);

            return string.Join(", ", selected);
        }
    }
}
