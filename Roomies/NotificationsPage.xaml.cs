using Microsoft.Maui.Controls.Shapes;

namespace Roomies
{
    public partial class NotificationsPage : ContentPage
    {
        private Membru _user;

        public NotificationsPage(Membru user)
        {
            InitializeComponent();
            _user = user;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var db = ServiceHelper.GetService<DatabaseService>();
            var notifications = await db.GetNotificationsForUserAsync(_user.Id);

            NotificationsContainer.Children.Clear();

            foreach (var notif in notifications)
            {
                var border = new Border
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
                };

                NotificationsContainer.Children.Add(border);
            }
            var pendingRequests = await db.GetPendingRequestsForUserAsync(_user.Id);

            foreach (var req in pendingRequests)
            {
                var sender = await db.GetMembruByIdAsync(req.SenderId);

                var nameLabel = new Label
                {
                    Text = $"{sender.Nume} {sender.Prenume} ți-a trimis o cerere:",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    TextColor = Colors.Black
                };

                var messageLabel = new Label
                {
                    Text = $"Mesaj: \"{req.Mesaj}\"",
                    FontSize = 16,
                    TextColor = Colors.DarkGray
                };

                var acceptButton = new Button
                {
                    Text = "Acceptă",
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Padding = new Thickness(10, 5)
                };

                var rejectButton = new Button
                {
                    Text = "Respinge",
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Padding = new Thickness(10, 5)
                };

                acceptButton.Clicked += async (s, e) =>
                {
                    await db.UpdateFriendRequestStatusAsync(req.ID, "Accepted");
                    await DisplayAlertAsync("Succes", "Ai acceptat cererea!", "OK");
                    OnAppearing();
                };

                rejectButton.Clicked += async (s, e) =>
                {
                    await db.UpdateFriendRequestStatusAsync(req.ID, "Rejected");
                    await DisplayAlertAsync("Respins", "Ai respins cererea.", "OK");
                    OnAppearing();
                };

                var buttonRow = new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children = { acceptButton, rejectButton }
                };

                var border = new Border
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
                            nameLabel,
                            messageLabel,
                            buttonRow
                        }
                    }
                };

                NotificationsContainer.Children.Add(border);
            }
        }
    }
}
