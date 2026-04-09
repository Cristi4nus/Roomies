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
            _db.CreateTableAsync<AlarmaFiltre>().Wait();
        }

        public async Task InitializeTablesAsync()
        {
            await _db.CreateTableAsync<AlarmaFiltre>();
        }

        public async Task<List<Notificare>> GetNotificariForUserAsync(int userId)
        {
            var client =new HttpClient();
            var response = await  client.GetAsync($"http://10.0.2.2:5137/api/notificari/{userId}");
             return await response.Content.ReadFromJsonAsync<List<Notificare>>();
        }
        //initializare lista prieteni
        public async Task<List<Membru>> GetPrieteniAsync(int userId)
        {
            var client = new HttpClient();
            var raspuns = await client.GetAsync($"http://10.0.2.2:5137/api/prieteni/{userId}");
            return await raspuns.Content.ReadFromJsonAsync<List<Membru>>();
        }
        //adaugare alarma in caz ca utilizatorul nu sia gasit persoana compatibila
        public async Task AddAlarmAsync(AlarmaFiltre alarm)
        {
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync("http://10.0.2.2:5137/api/alarme/add", alarm);
            response.EnsureSuccessStatusCode();
        }

        public Task<List<AlarmaFiltre>> GetAlarmsForUserAsync(int userId)
        {
           return _db.Table<AlarmaFiltre>().Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task AddFriendshipAsync(int user1Id, int user2Id)
        {
            var client = new HttpClient();
            var response = await client.PostAsync($"http://10.0.2.2:5137/api/prieteni/add?user1={user1Id}&user2={user2Id}",null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Membru>> GetAllMembriAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://10.0.2.2:5137/api/membri");
            return await response.Content.ReadFromJsonAsync<List<Membru>>();
        }

        public async Task<Membru?> GetMembruByIdAsync(int id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/membri/{id}");
                return await response.Content.ReadFromJsonAsync<Membru>();
        }

        public async Task<List<CererePrietenie>> GetPendingRequestsForUserAsync(int userId)
        {
             var client = new HttpClient();
             var response = await client.GetAsync($"http://10.0.2.2:5137/api/cereri/pending/{userId}");
             return await response.Content.ReadFromJsonAsync<List<CererePrietenie>>();
        }

        public async Task UpdateFriendRequestStatusAsync(int requestId, string statusNou)
        {
             var client = new HttpClient();
             var body = new { requestId = requestId, status = statusNou };
             var response = await client.PostAsJsonAsync("http://10.0.2.2:5137/api/cereri/update", body);
             response.EnsureSuccessStatusCode();
        }

        public async Task<bool> UpdateMembruAsync(Membru membru)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync($"http://10.0.2.2:5137/api/membri/{membru.Id}", membru);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContAsync(int userId)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync($"http://10.0.2.2:5137/api/membri/{userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AreFriendsAsync(int user1Id, int user2Id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"http://10.0.2.2:5137/api/prieteni/arefriends?user1={user1Id}&user2={user2Id}");
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> HasPendingRequestAsync(int senderId, int receiverId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync( $"http://10.0.2.2:5137/api/cereri/haspending?senderId={senderId}&receiverId={receiverId}");
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        //daca cineva a trimis deja  o cerere de prietenie atunci da un warning
        public async Task<bool> HasReversePendingRequestAsync(int senderId, int receiverId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                 $"http://10.0.2.2:5137/api/cereri/hasreverse?senderId={senderId}&receiverId={receiverId}");
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
                "http://10.0.2.2:5137/api/cereri/send", cerere);
            response.EnsureSuccessStatusCode();
        }
        
        
        //initializare conversatie 
        public async Task<List<Mesaj>> GetConversationAsync(int user1, int user2)
        {
            var client = new HttpClient();
            var response = await client.GetAsync( $"http://10.0.2.2:5137/api/mesaje/{user1}/{user2}");
            return await response.Content.ReadFromJsonAsync<List<Mesaj>>();
        }
    }
}