using Roomies;
using System;
using System.Collections.Generic;

namespace Roomies
{
    public partial class ProfilePage : ContentPage
    {
        private readonly DatabaseService _db;
        private Membru _user;
        private List<CheckBox> zonaCheckboxes = new();
        private List<CheckBox> traiCheckboxes = new();

        public ProfilePage(Membru user)
        {
            InitializeComponent();

            _db = ServiceHelper.GetService<DatabaseService>();
            _user = user;

            GenerateZonaCheckboxes();
            GeneratePreferinteCheckboxes();
            LoadUserData();
        }

        private void LoadUserData()
        {
            InputNume.Text = _user.Nume;
            InputPrenume.Text = _user.Prenume;
            InputVarsta.Text = _user.Varsta.ToString();
            pickerGen.SelectedItem = _user.Gen;
            InputFacultate.Text = _user.Facultate;
            InputTelefon.Text = _user.NumarTelefon;
            InputBuget.Text = _user.BugetMaxim.ToString();
            InputPerioada.Text = _user.PerioadaDeSedere;
            pickerStil.SelectedItem = _user.StilDeViata;
            editorDescriere.Text = _user.Descriere;
            InputEmail.Text = _user.Email;

            // Zone preferate
            var zoneSelectate = _user.ZonaPreferata?.Split(", ");

            for (int i = 0; i < zonaCheckboxes.Count; i++)
                if (zoneSelectate?.Contains(Optiuni.ZonePreferate[i]) == true)
                    zonaCheckboxes[i].IsChecked = true;

            // Preferinte de trai
            var traiSelectat = _user.PreferinteDeTrai?.Split(", ");

            for (int i = 0; i < traiCheckboxes.Count; i++)
                if (traiSelectat?.Contains(Optiuni.PreferinteDeTrai[i]) == true)
                    traiCheckboxes[i].IsChecked = true;
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

        private async void OnUpdateProfileClicked(object sender, EventArgs e)
        {
            if (!int.TryParse(InputVarsta.Text, out var varsta) ||
                !int.TryParse(InputBuget.Text, out var buget))
            {
                await DisplayAlert("Eroare", "Vârsta și bugetul trebuie să fie numere.", "OK");
                return;
            }

            _user.Nume = InputNume.Text;
            _user.Prenume = InputPrenume.Text;
            _user.Varsta = varsta;
            _user.Gen = pickerGen.SelectedItem?.ToString();
            _user.Facultate = InputFacultate.Text;
            _user.NumarTelefon = InputTelefon.Text;
            _user.ZonaPreferata = GetSelectedZona();
            _user.BugetMaxim = buget;
            _user.PerioadaDeSedere = InputPerioada.Text;
            _user.StilDeViata = pickerStil.SelectedItem?.ToString();
            _user.PreferinteDeTrai = GetSelectedPreferinte();
            _user.Descriere = editorDescriere.Text;
            _user.Email = InputEmail.Text;

            await _db.UpdateMembruAsync(_user);

            await DisplayAlert("Succes", "Profil actualizat!", "OK");
            await Navigation.PopAsync();
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
