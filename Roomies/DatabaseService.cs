using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Roomies
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Membru>().Wait();
            _db.CreateTableAsync<Notificare>().Wait();
            _db.CreateTableAsync<CererePrietenie>().Wait();
        }
        public Task<int> AddMembruAsync(Membru membru)
        {
            return _db.InsertAsync(membru);
        }

        public Task<Membru?> GetMembruByIdAsync(int id)
        {
            return _db.Table<Membru>()
                      .Where(m => m.Id == id)
                      .FirstOrDefaultAsync();
        }
        public Task<Membru?> GetMembruByEmailAsync(string email)
        {
            return _db.Table<Membru>()
                      .Where(m => m.Email == email)
                      .FirstOrDefaultAsync();
        }


        public Task<List<Membru>> GetAllMembriAsync()
        {
            return _db.Table<Membru>().ToListAsync();
        }

        public Task UpdateMembruAsync(Membru membru)
        {
            return _db.UpdateAsync(membru);
        }
        public Task<List<Notificare>> GetNotificationsForUserAsync(int userId)
        {
            return _db.Table<Notificare>()
                      .Where(n => n.UserId == userId)
                      .OrderByDescending(n => n.Data)
                      .ToListAsync();
        }
        public async Task<bool> HasPendingRequestAsync(int senderId, int receiverId)
        {
            var cerere = await _db.Table<CererePrietenie>()
                .Where(c => c.SenderId == senderId
                         && c.ReceiverId == receiverId
                         && c.Status == "Pending")
                .FirstOrDefaultAsync();

            return cerere != null;
        }
        public async Task SendFriendRequestWithNotificationAsync(int senderId, int receiverId, string mesaj)
        {
            if (await HasPendingRequestAsync(senderId, receiverId))
                throw new Exception("Există deja o cerere în așteptare!");

            var cerere = new CererePrietenie
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Mesaj = mesaj,
                Status = "Pending",
                Data = DateTime.Now
            };

            await _db.InsertAsync(cerere);
            var fromUser = await _db.Table<Membru>()
                                    .Where(m => m.Id == senderId)
                                    .FirstAsync();

            var notif = new Notificare
            {
                UserId = receiverId,
                Text = $"{fromUser.Nume} {fromUser.Prenume} ți-a trimis o cerere de prietenie: \"{mesaj}\"",
                Data = DateTime.Now
            };

            await _db.InsertAsync(notif);
        }

        public Task<List<CererePrietenie>> GetPendingRequestsForUserAsync(int userId)
        {
            return _db.Table<CererePrietenie>()
                      .Where(c => c.ReceiverId == userId && c.Status == "Pending")
                      .ToListAsync();
        }

        public async Task UpdateFriendRequestStatusAsync(int requestId, string newStatus)
        {
            var cerere = await _db.Table<CererePrietenie>()
                                  .Where(c => c.ID == requestId)
                                  .FirstOrDefaultAsync();

            if (cerere == null)
                return;

            cerere.Status = newStatus;
            await _db.UpdateAsync(cerere);

            var notif = new Notificare
            {
                UserId = cerere.SenderId,
                Text = newStatus == "Accepted"
                    ? "Cererea ta de prietenie a fost acceptată!"
                    : "Cererea ta de prietenie a fost respinsă.",
                Data = DateTime.Now
            };

            await _db.InsertAsync(notif);
        }
    }
}
