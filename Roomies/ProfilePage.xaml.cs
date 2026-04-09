using Roomies;
using System;
using System.Collections.Generic;

namespace Roomies
{
    public partial class ProfilePage : ContentPage
    {
        private readonly DatabaseService _db;
        private Membru _user;
        private bool _isOwnProfile;

        private List<CheckBox> zonaCheckboxes = new();
        private List<CheckBox> traiCheckboxes = new();

        public ProfilePage(Membru user, bool isOwnProfile = false)
        {
            InitializeComponent();

            _db = ServiceHelper.GetService<DatabaseService>();
            _user = user;
            _isOwnProfile = isOwnProfile;

            GenerateZonaCheckboxes();
            GeneratePreferinteCheckboxes();
            LoadUserData();
            ApplyEditMode();
        }

        private void ApplyEditMode()
        {
            if (_isOwnProfile)
                return;

            SaveButton.IsVisible = false;
            DeleteAccountButton.IsVisible = false;
            InputNume.IsReadOnly = true;
            InputPrenume.IsReadOnly = true;
            InputVarsta.IsReadOnly = true;
            pickerGen.IsEnabled = false;
            InputFacultate.IsReadOnly = true;
            InputTelefon.IsReadOnly = true;
            InputBuget.IsReadOnly = true;
            InputPerioada.IsReadOnly = true;
            pickerStil.IsEnabled = false;
            editorDescriere.IsReadOnly = true;
            InputEmail.IsVisible = false;
            foreach (var cb in zonaCheckboxes)
                cb.IsEnabled = false;
            foreach (var cb in traiCheckboxes)
                cb.IsEnabled = false;
            SelectedAvatarImage.IsEnabled = false;
            LogOut.IsVisible = false;
        }

        private void LoadUserData()
        {
            SelectedAvatarImage.Source = _user.Avatar ?? "utilizator.png";
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

            // ✅ FIX: Adaugă .Select(z => z.Trim()) pentru a elimina spațiile după virgulă
            var zoneSelectate = _user.ZonaPreferata?.Split(",").Select(z => z.Trim()).ToList();
            for (int i = 0; i < zonaCheckboxes.Count; i++)
                if (zoneSelectate?.Contains(Optiuni.ZonePreferate[i]) == true)
                    zonaCheckboxes[i].IsChecked = true;

            // ✅ FIX: Adaugă .Select(p => p.Trim()) pentru a elimina spațiile după virgulă
            var traiSelectat = _user.PreferinteDeTrai?.Split(",").Select(p => p.Trim()).ToList();
            for (int i = 0; i < traiCheckboxes.Count; i++)
                if (traiSelectat?.Contains(Optiuni.PreferinteDeTrai[i]) == true)
                    traiCheckboxes[i].IsChecked = true;
        }

        private void OnAvatarTapped(object sender, TappedEventArgs e)
        {
            if (!_isOwnProfile)
                return;
            string avatar = e.Parameter.ToString();
            _user.Avatar = avatar;
            SelectedAvatarImage.Source = avatar;
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Confirmare", "Sigur vrei să te deconectezi?", "Da", "Nu");
            if (!confirm)
                return;
            App.JwtToken = null;
            Application.Current.MainPage = new NavigationPage(new UserLoginPage());
        }

        private async void OnDeleteAccountClicked(object sender, EventArgs e)
        {
            if (!_isOwnProfile)
                return;

            bool confirm = await DisplayAlertAsync(
                "Confirmare", "Sigur vrei sa iti stergi contul ?",
                "Da", "Nu, anuleaza");

            if (!confirm)
                return;

            var success = await _db.DeleteContAsync(_user.Id);

            if (!success)
            {
                await DisplayAlertAsync("Eroare", "Nu s-a putut șterge contul.", "OK");
                return;
            }

            App.JwtToken = null;
            Application.Current.MainPage = new NavigationPage(new UserLoginPage());
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

        private async void OnUpdateProfileClicked(object sender, EventArgs e)
        {
            if (!_isOwnProfile)
                return;

            if (!int.TryParse(InputVarsta.Text, out var varsta) ||
                !int.TryParse(InputBuget.Text, out var buget))
            {
                await DisplayAlertAsync("Eroare", "Varsta si bugetul trebuie sa fie numere.", "OK");
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
            await DisplayAlertAsync("Succes", "Profil actualizat!", "OK");
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