using Roomies;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using Roomies.Models;

namespace Roomies
{
    public partial class LoginPage : ContentPage
    {
        private readonly List<CheckBox> zonaCheckboxes = new();
        private readonly List<CheckBox> traiCheckboxes = new();
        private string _selectedAvatar = "utilizator.png";

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (zonaCheckboxes.Count == 0)
                GenerateZonaCheckboxes();

            if (traiCheckboxes.Count == 0)
                GeneratePreferinteCheckboxes();
        }

        private void OnAvatarTapped(object sender, TappedEventArgs avatar)
        {
            _selectedAvatar = avatar.Parameter.ToString();
            SelectedAvatarImage.Source = _selectedAvatar;
        }

        private void GenerateZonaCheckboxes()
        {
            zonaContainer.Children.Clear();
            zonaCheckboxes.Clear();

            foreach (var zona in Optiuni.ZonePreferate)
            {
                var box = new CheckBox();
                zonaCheckboxes.Add(box);

                zonaContainer.Children.Add(new HorizontalStackLayout
                {
                    Children =
                    {
                        box, new Label { Text = zona, VerticalOptions = LayoutOptions.Center }
                    }
                });
            }
        }

        private void GeneratePreferinteCheckboxes()
        {
            traiContainer.Children.Clear();
            traiCheckboxes.Clear();

            foreach (var preferinte in Optiuni.PreferinteDeTrai)
            {
                var box = new CheckBox();
                traiCheckboxes.Add(box);

                traiContainer.Children.Add(new HorizontalStackLayout
                {
                    Children =
                    {
                        box, new Label { Text = preferinte, VerticalOptions = LayoutOptions.Center }
                    }
                });
            }
        }
        //validare pentru campuri la login
        private async void OnCreateProfileClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputNume.Text) ||
                string.IsNullOrWhiteSpace(InputPrenume.Text) ||
                string.IsNullOrWhiteSpace(InputParola.Text) ||
                string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                await DisplayAlertAsync("Eroare", "Completeaza numele, prenumele, emailul și parola.", "Ok");
                return;
            }

            if (!int.TryParse(InputVarsta?.Text, out var varsta))
            {
                await DisplayAlertAsync("Eroare", "Introdu o varsta valida.", "Ok");
                return;
            }

            if (!int.TryParse(InputBuget?.Text, out var buget))
            {
                await DisplayAlertAsync("Eroare", "Introdu un buget valid.", "Ok");
                return;
            }

            var request = new CerereRegister
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
                Parola = InputParola.Text
            };

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                "http://10.0.2.2:5137/api/ControllerAutentificare/register",
                request
            );
            //eroare daca campurile goale nu sunt completate
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Atentie", "Toate campurile sunt obligatorii", "Ok");
                return;
            }

            await DisplayAlertAsync("Succes", "Profil creat,confirma contul folosind emailul trimis", "Ok");

            Application.Current.MainPage = new NavigationPage(new UserLoginPage());
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
            var selectate = new List<string>();

            for (int i = 0; i < traiCheckboxes.Count; i++)
                if (traiCheckboxes[i].IsChecked)
                    selectate.Add(Optiuni.PreferinteDeTrai[i]);

            return string.Join(", ", selectate);
        }
    }
}
