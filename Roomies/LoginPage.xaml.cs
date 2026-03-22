using Roomies;
using System;
using System.Collections.Generic;

namespace Roomies
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _db;
        private readonly List<CheckBox> zonaCheckboxes = new();
        private readonly List<CheckBox> traiCheckboxes = new();
        private string _selectedAvatar = "utilizator.png";

        public LoginPage()
        {
            InitializeComponent();

            _db = ServiceHelper.GetService<DatabaseService>();

            GenerateZonaCheckboxes();
            GeneratePreferinteCheckboxes();
        }

        private void OnAvatarTapped(object sender, TappedEventArgs e)
        {
            _selectedAvatar = e.Parameter.ToString();
            SelectedAvatarImage.Source = _selectedAvatar;
        }

        private void GenerateZonaCheckboxes()
        {
            zonaContainer.Children.Clear();
            zonaCheckboxes.Clear();

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
            traiContainer.Children.Clear();
            traiCheckboxes.Clear();

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
            if (string.IsNullOrWhiteSpace(InputNume.Text) ||
                string.IsNullOrWhiteSpace(InputPrenume.Text) ||
                string.IsNullOrWhiteSpace(InputParola.Text)||
                string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                await DisplayAlertAsync("Eroare", "Completeaza numele, prenumele,emailul si parola.", "okay!");
                return;
            }
            var existing = await _db.GetMembruByEmailAsync(InputEmail.Text);
            if (existing != null)
            {
                await DisplayAlertAsync("Eroare", "Acest email este deja folosit. Alege altul.", "ok!");
                return;
            }

            if (!int.TryParse(InputVarsta?.Text, out var varsta))
            {
                await DisplayAlertAsync("Eroare", "Introduceti o varsta valida.", "splendid!");
                return;
            }

            if (!int.TryParse(InputBuget?.Text, out var buget))
            {
                await DisplayAlertAsync("Eroare", "Introduceti un buget valid.", "super!");
                return;
            }

            PasswordHelper.CreatePasswordHash(InputParola.Text, out byte[] hash, out byte[] salt);

            var membru = new Membru
            {
                Avatar = _selectedAvatar,
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

            await _db.AdaugaMembruAsync(membru);

            await DisplayAlertAsync("Succes", "Profil creat,totu bine!", "Okay!");

            await Navigation.PushAsync(ServiceHelper.GetService<UserLoginPage>());
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
