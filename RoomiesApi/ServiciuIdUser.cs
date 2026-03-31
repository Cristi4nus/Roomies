using Microsoft.AspNetCore.SignalR;

public class ServiciuIdUser : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("id")?.Value;
    }
}

