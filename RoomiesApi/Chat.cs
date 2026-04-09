using Microsoft.AspNetCore.SignalR;

namespace RoomiesApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly DatabaseService _db;

        public ChatHub(DatabaseService db)
        {
            _db = db;
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            await _db.SaveMessageAsync(int.Parse(senderId), int.Parse(receiverId), message);

            await Clients.User(receiverId).SendAsync("PrimesteMesaj", senderId, message);
        }
    }
}
