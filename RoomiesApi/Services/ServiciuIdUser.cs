using Microsoft.AspNetCore.SignalR;

namespace RoomiesApi.Services
{

    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("id")?.Value
                ?? connection.User?.FindFirst("sub")?.Value;
        }
    }
}
