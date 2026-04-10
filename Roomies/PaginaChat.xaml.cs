using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Roomies.Models;
using Roomies;

namespace Roomies
{
    public partial class PaginaChat : ContentPage
    {
        private readonly ServiciuChat _chat;
        private readonly Membru _me;
        private readonly Membru prieten;
        private bool _istoricIncarcat = false;

        public PaginaChat(Membru me, Membru friend)
        {
            InitializeComponent();

            _chat = ServiceHelper.GetService<ServiciuChat>();
            _me = me;
            prieten = friend;

            _chat.Connection.On<string, string>("PrimesteMesaj", async (fromId, message) =>
            {
                if (fromId == prieten.Id.ToString())
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        if (message.StartsWith("LOCATION:"))
                            AddLocationMessage($"{prieten.Nume} {prieten.Prenume}", message);
                        else
                            await AddTextMessage($"{prieten.Nume} {prieten.Prenume}", message, Colors.Black);
                    });
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _chat.Connection.Remove("PrimesteMesaj");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (_chat.Connection.State == HubConnectionState.Connected)
                    await _chat.Connection.StopAsync();
                await _chat.StartAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Eroare", ex.ToString(), "OK");
            }

            if (_istoricIncarcat) return;
            _istoricIncarcat = true;

            var db = ServiceHelper.GetService<DatabaseService>();
            var istoric = await db.GetConversationAsync(_me.Id, prieten.Id);

            foreach (var msg in istoric)
            {
                if (msg.Text.StartsWith("LOCATION:"))
                {
                    var sender = msg.SenderId == _me.Id ? "Eu" : $"{prieten.Nume} {prieten.Prenume}";
                    AddLocationMessage(sender, msg.Text);
                }
                else
                {
                    if (msg.SenderId == _me.Id)
                        await AddTextMessage("Eu", msg.Text, Colors.Blue);
                    else
                        await AddTextMessage($"{prieten.Nume} {prieten.Prenume}", msg.Text, Colors.Black);
                }
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var text = MessageEntry.Text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            await _chat.SendMessage(_me.Id.ToString(), prieten.Id.ToString(), text);
            await AddTextMessage("Eu", text, Colors.Blue);
            MessageEntry.Text = "";
        }

        private async void OnSendLocationClicked(object sender, EventArgs e)
        {
            var harta = new PaginaHarta();
            await Navigation.PushAsync(harta);
            var result = await harta.GetLocationAsync();

            if (result == null) return;

            await _chat.SendLocation(
                _me.Id.ToString(),
                prieten.Id.ToString(),
                result.Value.Lat,
                result.Value.Lng
            );

            AddLocationMessage("Eu", $"LOCATION:{result.Value.Lat},{result.Value.Lng}");
        }

        private async Task AddTextMessage(string sender, string text, Color color)
        {
            MessagesContainer.Children.Add(new Label
            {
                Text = $"{sender}: {text}",
                TextColor = color
            });
            await scrollView.ScrollToAsync(MessagesContainer, ScrollToPosition.End, true);
        }

        private void AddLocationMessage(string sender, string raw)
        {
            var coords = raw.Replace("LOCATION:", "").Split(',');
            double lat = double.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            double lng = double.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);

            var button = new Button
            {
                Text = $"{sender} a trimis o locație",
                BackgroundColor = Colors.LightGray
            };

            button.Clicked += (s, e) =>
            {
                var url = $"https://www.google.com/maps?q={lat},{lng}";
                Launcher.OpenAsync(url);
            };

            MessagesContainer.Children.Add(button);
        }
    }
}