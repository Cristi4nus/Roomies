using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Roomies
{
    public partial class PaginaHarta : ContentPage
    {
        private readonly TaskCompletionSource<(double Lat, double Lng)?> _tcs;
        private bool _confirmed = false;

        public PaginaHarta()
        {
            InitializeComponent();
            _tcs = new TaskCompletionSource<(double Lat, double Lng)?>();

            _ = LoadMapAsync();
        }

        private async Task LoadMapAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("harta.html");
                using var reader = new StreamReader(stream);
                var html = await reader.ReadToEndAsync();

                var source = new HtmlWebViewSource { Html = html };

                #if ANDROID
                                source.BaseUrl = "file:///android_asset/";
                #elif IOS || MACCATALYST
                        source.BaseUrl = Foundation.NSBundle.MainBundle.BundlePath;
                #endif

                mapWebView.Source = source;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HARTA] Eroare: {ex.Message}");
                await DisplayAlertAsync("Eroare", $"Nu s-a putut incarca harta: {ex.Message}", "OK");
            }
        }

        public Task<(double Lat, double Lng)?> GetLocationAsync()
        {
            return _tcs.Task;
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("roomies-app://", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                if (_confirmed) return;

                try
                {
                    var uri = new Uri(e.Url);
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

                    if (double.TryParse(query["lat"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(query["lng"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lng))
                    {
                        _confirmed = true;
                        _tcs.TrySetResult((lat, lng));
                        MainThread.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Eroare parsare: {ex.Message}");
                    _tcs.TrySetResult(null);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!_tcs.Task.IsCompleted)
                _tcs.TrySetResult(null);
        }
    }
}