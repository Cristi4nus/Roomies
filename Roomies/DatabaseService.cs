using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.Linq;

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
            _db.CreateTableAsync<Prietenie>().Wait();
            _db.CreateTableAsync<AlarmaFiltre>().Wait();
        }
        //adaugan membru si verificam daca se potriveste cu o alarma existenta, daca exista trimitem o notificare utilizatorului 
        public async Task<int> AdaugaMembruAsync(Membru membru)
        {
            var id = await _db.InsertAsync(membru);

            var toateAlarmele = await _db.Table<AlarmaFiltre>().ToListAsync();

            foreach (var alarma in toateAlarmele)
            {
                if (PotrivireAlarma(membru, alarma))
                {
                    var notificare = new Notificare
                    {
                        UserId = alarma.UserId,
                        Text = $"Un nou utilizator se potrivește cu alarmele tale: {membru.Nume} {membru.Prenume}",
                        Data = DateTime.Now
                    };

                    await _db.InsertAsync(notificare);
                }
            }

            return id;
        }
        private bool PotrivireAlarma(Membru membru, AlarmaFiltre alarma)
        {
            return (alarma.Gen == null || membru.Gen == alarma.Gen) &&
                   (alarma.Zona == null || membru.ZonaPreferata == alarma.Zona) &&
                   (alarma.Buget == null || membru.BugetMaxim.ToString() == alarma.Buget) &&
                   (alarma.StilViata == null || membru.StilDeViata == alarma.StilViata) &&
                   (alarma.Preferinte == null || membru.PreferinteDeTrai == alarma.Preferinte);
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
                .Where(cerere => cerere.SenderId == senderId
                         && cerere.ReceiverId == receiverId
                         && cerere.Status == "Pending")
                .FirstOrDefaultAsync();

            return cerere != null;
        }
        public async Task RemoveFriendshipAsync(int user1, int user2)
        {
            var friendship = await _db.Table<Prietenie>()
                .Where(p =>
                    (p.User1Id == user1 && p.User2Id == user2) ||
                    (p.User1Id == user2 && p.User2Id == user1))
                .FirstOrDefaultAsync();

            if (friendship != null)
                await _db.DeleteAsync(friendship);
        }

        public async Task AddFriendshipAsync(int user1, int user2)
        {
            var pr = new Prietenie
            {
                User1Id = user1,
                User2Id = user2
            };

            await _db.InsertAsync(pr);
        }
        public async Task<List<Membru>> GetFriendsAsync(int userId)
        {
            var friendships = await _db.Table<Prietenie>()
                .Where(p => p.User1Id == userId || p.User2Id == userId)
                .ToListAsync();

            var friendIds = friendships.Select(p =>
                p.User1Id == userId ? p.User2Id : p.User1Id).ToList();

            var friends = new List<Membru>();

            foreach (var id in friendIds)
                friends.Add(await GetMembruByIdAsync(id));

            return friends;
        }


        public async Task SendFriendRequestWithNotificationAsync(int senderId, int receiverId, string mesaj)
        {
            if (await HasPendingRequestAsync(senderId, receiverId))
                throw new Exception("Exista deja o cerere in asteptare!");

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

            var notificare = new Notificare
            {
                UserId = receiverId,
                Text = $"{fromUser.Nume} {fromUser.Prenume} ti-a trimis o cerere de prietenie: \"{mesaj}\"",
                Data = DateTime.Now
            };

            await _db.InsertAsync(notificare);
        }
        public async Task<bool> AreFriendsAsync(int user1, int user2)
        {
            var friendship = await _db.Table<Prietenie>()
                .Where(p =>
                    (p.User1Id == user1 && p.User2Id == user2) ||
                    (p.User1Id == user2 && p.User2Id == user1))
                .FirstOrDefaultAsync();

            return friendship != null;
        }
        public async Task<bool> HasReversePendingRequestAsync(int user1, int user2)
        {
            var req = await _db.Table<CererePrietenie>()
                .Where(r => r.SenderId == user2 &&
                            r.ReceiverId == user1 &&
                            r.Status == "Pending")
                .FirstOrDefaultAsync();

            return req != null;
        }


        public Task<List<CererePrietenie>> GetPendingRequestsForUserAsync(int userId)
        {
            return _db.Table<CererePrietenie>()
                      .Where(c => c.ReceiverId == userId && c.Status == "Pending")
                      .ToListAsync();
        }

        public async Task UpdateFriendRequestStatusAsync(int requestId, string StatusNou)
        {
            var cerere = await _db.Table<CererePrietenie>()
                                  .Where(c => c.ID == requestId)
                                   .FirstOrDefaultAsync();

            if (cerere == null)
                return;

            cerere.Status = StatusNou;
            await _db.UpdateAsync(cerere);

            var notif = new Notificare
            {
                UserId = cerere.SenderId,
                Text = StatusNou == "Accepted"
                    ? "Cererea ta de prietenie a fost acceptata!"
                    : "Cererea ta de prietenie a fost respinsa.",
                Data = DateTime.Now
            };

            await _db.InsertAsync(notif);
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

    }
}
