using Dapper;
using Microsoft.Data.Sqlite;
using RoomiesApi.Models;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=roomies_api.db";

    public DatabaseService()
    {
        using var connection = new SqliteConnection(_connectionString);

        connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Membri (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Avatar TEXT,
            Nume TEXT,
            Prenume TEXT,
            Varsta INTEGER,
            Gen TEXT,
            Facultate TEXT,
            NumarTelefon TEXT,
            ZonaPreferata TEXT,
            BugetMaxim INTEGER,
            PerioadaDeSedere TEXT,
            StilDeViata TEXT,
            PreferinteDeTrai TEXT,
            Descriere TEXT,
            Email TEXT NOT NULL UNIQUE,
            ParolaHash BLOB,
            ParolaSalt BLOB
        );
    ");

        connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Prieteni (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            User1Id INTEGER NOT NULL,
            User2Id INTEGER NOT NULL
        );
    ");

        connection.Execute(@"
        CREATE TABLE IF NOT EXISTS CereriPrietenie (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SenderId INTEGER NOT NULL,
            ReceiverId INTEGER NOT NULL,
            Mesaj TEXT,
            Status TEXT,
            Data TEXT
            );
            ");
            connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Notificari (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            Text TEXT NOT NULL,
            Data TEXT NOT NULL
            );
            ");
            connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Mesaje (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SenderId INTEGER NOT NULL,
            ReceiverId INTEGER NOT NULL,
            Text TEXT NOT NULL,
            Data TEXT NOT NULL
            );
            ");

    }


    public async Task<Membru> GetMembruByEmailAsync(string email)
    {
        using var connection = new SqliteConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<Membru>(
            "SELECT * FROM Membri WHERE Email = @Email",
            new { Email = email });
    }
    public async Task<Membru?> GetMembruByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<Membru>(
            "SELECT * FROM Membri WHERE Id = @Id",
            new { Id = id });
    }


    public async Task AddMembruAsync(Membru membru)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(@"
        INSERT INTO Membri 
        (Avatar, Nume, Prenume, Varsta, Gen, Facultate, NumarTelefon, ZonaPreferata,
         BugetMaxim, PerioadaDeSedere, StilDeViata, PreferinteDeTrai, Descriere,
         Email, ParolaHash, ParolaSalt)
        VALUES 
        (@Avatar, @Nume, @Prenume, @Varsta, @Gen, @Facultate, @NumarTelefon, @ZonaPreferata,
         @BugetMaxim, @PerioadaDeSedere, @StilDeViata, @PreferinteDeTrai, @Descriere,
         @Email, @ParolaHash, @ParolaSalt)",
            membru);
    }

    public async Task AddPrietenAsync(int user1, int user2)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(
            "INSERT INTO Prieteni (User1Id, User2Id) VALUES (@u1, @u2)",
            new { u1 = user1, u2 = user2 });
    }

    public async Task RemovePrietenAsync(int user1, int user2)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(
            @"DELETE FROM Prieteni 
          WHERE (User1Id = @u1 AND User2Id = @u2)
             OR (User1Id = @u2 AND User2Id = @u1)",
            new { u1 = user1, u2 = user2 });
    }

    public async Task<List<int>> GetPrieteniIdsAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var ids = await connection.QueryAsync<int>(
            @"SELECT 
            CASE 
                WHEN User1Id = @id THEN User2Id 
                ELSE User1Id 
            END
          FROM Prieteni
          WHERE User1Id = @id OR User2Id = @id",
            new { id = userId });

        return ids.ToList();
    }
    public async Task<List<Membru>> GetAllMembriAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryAsync<Membru>("SELECT * FROM Membri");
        return result.ToList();
    }
    public async Task AddNotificationAsync(int userId, string text)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(
            @"INSERT INTO Notificari (UserId, Text, Data)
          VALUES (@u, @t, @d)",
            new { u = userId, t = text, d = DateTime.Now }
        );
    }

    public async Task<List<Notificare>> GetNotificationsForUserAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryAsync<Notificare>(
            @"SELECT * FROM Notificari 
          WHERE UserId = @id 
          ORDER BY Data DESC",
            new { id = userId });

        return result.ToList();
    }


    public async Task UpdateMembruAsync(Membru membru)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(@"
        UPDATE Membri SET
            Avatar = @Avatar,
            Nume = @Nume,
            Prenume = @Prenume,
            Varsta = @Varsta,
            Gen = @Gen,
            Facultate = @Facultate,
            NumarTelefon = @NumarTelefon,
            ZonaPreferata = @ZonaPreferata,
            BugetMaxim = @BugetMaxim,
            PerioadaDeSedere = @PerioadaDeSedere,
            StilDeViata = @StilDeViata,
            PreferinteDeTrai = @PreferinteDeTrai,
            Descriere = @Descriere,
            Email = @Email
        WHERE Id = @Id",
            membru);
    }

    public async Task<List<CererePrietenie>> GetPendingRequestsForUserAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryAsync<CererePrietenie>(
            @"SELECT * FROM CereriPrietenie 
          WHERE ReceiverId = @id AND Status = 'Pending'",
            new { id = userId });

        return result.ToList();
    }
    public async Task<bool> HasPendingRequestAsync(int senderId, int receiverId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryFirstOrDefaultAsync<CererePrietenie>(
            @"SELECT * FROM CereriPrietenie 
          WHERE SenderId = @s AND ReceiverId = @r AND Status = 'Pending'",
            new { s = senderId, r = receiverId });

        return result != null;
    }
    public async Task<bool> HasReversePendingRequestAsync(int senderId, int receiverId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryFirstOrDefaultAsync<CererePrietenie>(
            @"SELECT * FROM CereriPrietenie 
          WHERE SenderId = @r AND ReceiverId = @s AND Status = 'Pending'",
            new { s = senderId, r = receiverId });

        return result != null;
    }
    public async Task SendFriendRequestAsync(CererePrietenie cerere)
    {
        using var connection = new SqliteConnection(_connectionString);

        cerere.Status = "Pending";
        cerere.Data = DateTime.Now;

        await connection.ExecuteAsync(
            @"INSERT INTO CereriPrietenie (SenderId, ReceiverId, Mesaj, Status, Data)
          VALUES (@SenderId, @ReceiverId, @Mesaj, @Status, @Data)",
            cerere
        );
        await AddNotificationAsync(cerere.ReceiverId, $"Ai primit o cerere de prietenie de la userul {cerere.SenderId}");

    }
    public async Task UpdateFriendRequestStatusAsync(int requestId, string status)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(
            @"UPDATE CereriPrietenie 
          SET Status = @st 
          WHERE Id = @id",
            new { st = status, id = requestId }
        );
    }
    public async Task<bool> AreFriendsAsync(int user1, int user2)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryFirstOrDefaultAsync<Prieteni>(
            @"SELECT * FROM Prieteni 
          WHERE (User1Id = @u1 AND User2Id = @u2)
             OR (User1Id = @u2 AND User2Id = @u1)",
            new { u1 = user1, u2 = user2 }
        );

        return result != null;
    }
    public async Task SaveMessageAsync(int senderId, int receiverId, string text)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO Mesaje (SenderId, ReceiverId, Text, Data) VALUES (@s, @r, @t, @d)",
            new { s = senderId, r = receiverId, t = text, d = DateTime.Now });
    }

    public async Task<List<Mesaj>> GetConversationAsync(int user1, int user2)
    {
        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryAsync<Mesaj>(
            @"SELECT * FROM Mesaje 
          WHERE (SenderId = @u1 AND ReceiverId = @u2)
             OR (SenderId = @u2 AND ReceiverId = @u1)
          ORDER BY Data ASC",
            new { u1 = user1, u2 = user2 });
        return result.ToList();
    }

}
