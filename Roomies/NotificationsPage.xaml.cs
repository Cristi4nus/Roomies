using Microsoft.Maui.Controls.Shapes;
using Roomies.Models;

namespace Roomies
{
    public partial class NotificationsPage : ContentPage
    {
        private Membru _user;
        private bool _isActive = false;

        public NotificationsPage(Membru user)
        {
            InitializeComponent();
            _user = user;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _isActive = true;
            _ = LoadNotifications();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isActive = false;
        }

        private async Task LoadNotifications()
        {
            if (!_isActive) return;

            var db = ServiceHelper.GetService<DatabaseService>();

            NotificationsContainer.Children.Clear();

            var notifications = await db.GetNotificariForUserAsync(_user.Id);

            if (!_isActive) return;

            foreach (var notif in notifications)
            {
                NotificationsContainer.Children.Add(
                    new Border
                    {
                        Stroke = Colors.LightGray,
                        StrokeThickness = 1,
                        Background = Colors.White,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Padding = 10,
                        Margin = new Thickness(0, 0, 0, 10),
                        Content = new Label
                        {
                            Text = notif.Text,
                            FontSize = 16,
                            TextColor = Colors.Black
                        }
                    }
                );
            }

            var pendingRequests = await db.GetPendingRequestsForUserAsync(_user.Id);

            if (!_isActive) return;

            foreach (var req in pendingRequests)
            {
                var sender = await db.GetMembruByIdAsync(req.SenderId);

                if (sender == null) continue;

                var acceptButton = new Button
                {
                    Text = "Accepta",
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White,
                    CornerRadius = 8
                };

                var rejectButton = new Button
                {
                    Text = "Respinge",
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    CornerRadius = 8
                };

                acceptButton.Clicked += async (s, e) =>
                {
                    await db.UpdateFriendRequestStatusAsync(req.Id, "Accepted");
                    await db.AddFriendshipAsync(req.SenderId, req.ReceiverId);
                    await DisplayAlertAsync("Succes", "Ai acceptat cererea!", "OK");
                    await LoadNotifications();
                };

                rejectButton.Clicked += async (s, e) =>
                {
                    await db.UpdateFriendRequestStatusAsync(req.Id, "Rejected");
                    await DisplayAlertAsync("Respins", "Ai respins cererea.", "OK");
                    await LoadNotifications();
                };

                NotificationsContainer.Children.Add(
                    new Border
                    {
                        Stroke = Colors.LightGray,
                        StrokeThickness = 1,
                        Background = Colors.White,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Padding = 10,
                        Margin = new Thickness(0, 0, 0, 10),
                        Content = new VerticalStackLayout
                        {
                            Spacing = 5,
                            Children =
                            {
                                new Label { Text = $"{sender.Nume} {sender.Prenume} ți-a trimis o cerere:", FontAttributes = FontAttributes.Bold },
                                new Label { Text = $"Mesaj: \"{req.Mesaj}\"" },
                                new Label { Text = $"Trimisă la: {req.Data:dd MMM yyyy, HH:mm}" },
                                new HorizontalStackLayout { Spacing = 10, Children = { acceptButton, rejectButton } }
                            }
                        }
                    }
                );
            }
        }
    }
}