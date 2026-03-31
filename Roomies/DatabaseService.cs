using SQLite;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Roomies
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeTablesAsync()
        {
            await _db.CreateTableAsync<AlarmaFiltre>();
        }

        public async Task<List<Notificare>> GetNotificationsForUserAsync(int userId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/notificari/{userId}");

            if (!response.IsSuccessStatusCode)
                return new List<Notificare>();

            return await response.Content.ReadFromJsonAsync<List<Notificare>>();
        }


        public async Task<List<Membru>> GetFriendsAsync(int userId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/prieteni/{userId}");

            if (!response.IsSuccessStatusCode)
                return new List<Membru>();

            return await response.Content.ReadFromJsonAsync<List<Membru>>();
        }

        public Task<int> AddAlarmAsync(AlarmaFiltre alarm)
        {
            return _db.InsertAsync(alarm);
        }

        public Task<List<AlarmaFiltre>> GetAlarmsForUserAsync(int userId)
        {
            return _db.Table<AlarmaFiltre>()
                      .Where(a => a.UserId == userId)
                      .ToListAsync();
        }

        public async Task AddFriendshipAsync(int user1Id, int user2Id)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(
                $"http://10.0.2.2:5137/api/prieteni/add?user1={user1Id}&user2={user2Id}",
                null
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Membru>> GetAllMembriAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://10.0.2.2:5137/api/membri");

            if (!response.IsSuccessStatusCode)
                return new List<Membru>();

            return await response.Content.ReadFromJsonAsync<List<Membru>>();
        }

        public async Task<Membru?> GetMembruByIdAsync(int id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/membri/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Membru>();
        }

        public async Task<List<CererePrietenie>> GetPendingRequestsForUserAsync(int userId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/cereri/pending/{userId}");

            if (!response.IsSuccessStatusCode)
                return new List<CererePrietenie>();

            return await response.Content.ReadFromJsonAsync<List<CererePrietenie>>();
        }

        public async Task UpdateFriendRequestStatusAsync(int requestId, string statusNou)
        {
            var client = new HttpClient();

            var body = new
            {
                requestId = requestId,
                status = statusNou
            };

            var response = await client.PostAsJsonAsync(
                "http://10.0.2.2:5137/api/cereri/update",
                body
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> UpdateMembruAsync(Membru membru)
        {

            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(
                $"http://10.0.2.2:5137/api/membri/{membru.Id}",
                membru
            );

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> AreFriendsAsync(int user1Id, int user2Id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"http://10.0.2.2:5137/api/prieteni/arefriends?user1={user1Id}&user2={user2Id}"
            );

            if (!response.IsSuccessStatusCode)
                return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> HasPendingRequestAsync(int senderId, int receiverId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"http://10.0.2.2:5137/api/cereri/haspending?senderId={senderId}&receiverId={receiverId}"
            );

            if (!response.IsSuccessStatusCode)
                return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> HasReversePendingRequestAsync(int senderId, int receiverId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"http://10.0.2.2:5137/api/cereri/hasreverse?senderId={senderId}&receiverId={receiverId}"
            );

            if (!response.IsSuccessStatusCode)
                return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task SendFriendRequestWithNotificationAsync(int senderId, int receiverId, string mesaj)
        {
            var client = new HttpClient();

            var cerere = new CererePrietenie
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Mesaj = mesaj
            };

            var response = await client.PostAsJsonAsync(
                "http://10.0.2.2:5137/api/cereri/send",
                cerere
            );

            response.EnsureSuccessStatusCode();
        }
        public async Task<List<Mesaj>> GetConversationAsync(int user1, int user2)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"http://10.0.2.2:5137/api/mesaje/{user1}/{user2}");

            if (!response.IsSuccessStatusCode)
                return new List<Mesaj>();

            return await response.Content.ReadFromJsonAsync<List<Mesaj>>();
        }
    }
}
