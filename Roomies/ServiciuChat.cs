using Microsoft.AspNetCore.SignalR.Client;
using Roomies;

public class ServiciuChat
{
    public HubConnection Connection { get; private set; }

    public ServiciuChat()
    {
        Connection = new HubConnectionBuilder()
            .WithUrl("http://10.0.2.2:5137/chatHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(App.JwtToken);
            })
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        try
        {
            await Connection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("EROARE SIGNALR: " + ex);
            throw;
        }
    }

    public Task SendMessage(string senderId, string receiverId, string message)
        => Connection.InvokeAsync("SendMessage", senderId, receiverId, message);

    public Task SendLocation(string senderId, string receiverId, double lat, double lng)
        => Connection.InvokeAsync("SendMessage", senderId, receiverId, $"LOCATION:{lat},{lng}");
}