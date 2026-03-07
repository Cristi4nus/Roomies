using System;
using System.Collections.Generic;
using System.Text;
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

            // Încearcă să adauge coloana Avatar dacă nu există deja
            try
            {
                _db.ExecuteAsync("ALTER TABLE Membru ADD COLUMN Avatar TEXT").Wait();
            }
            catch
            {
                // Dacă dă eroare, înseamnă că Avatar există deja → ignorăm
            }
        }

        public Task<int> AddMembruAsync(Membru membru)
        {
            return _db.InsertAsync(membru);
        }

        public Task<Membru?> GetMembruByNumePrenumeAsync(string nume, string prenume)
        {
            return _db.Table<Membru>()
                      .Where(m => m.Nume == nume && m.Prenume == prenume)
                      .FirstOrDefaultAsync();
        }

        public Task<List<Membru>> GetAllMembriAsync()
        {
            return _db.Table<Membru>().ToListAsync();
        }

        public async Task UpdateMembruAsync(Membru membru)
        {
            await _db.UpdateAsync(membru);
        }
    }
}
