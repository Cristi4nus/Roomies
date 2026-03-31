using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

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
            using var stream = await FileSystem.OpenAppPackageFileAsync("harta.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            mapWebView.Source = new HtmlWebViewSource { Html = html };
        }

        public Task<(double Lat, double Lng)?> GetLocationAsync()
        {
            return _tcs.Task;
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("roomies-app://confirm", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;

                if (_confirmed)
                    return;

                _confirmed = true;

                var uri = new Uri(e.Url);
                var parts = System.Web.HttpUtility.ParseQueryString(uri.Query);

                if (double.TryParse(parts["lat"],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double lat)
                    &&
                    double.TryParse(parts["lng"],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double lng))
                {
                    _tcs.TrySetResult((lat, lng));

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                    });
                }
                else
                {
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
