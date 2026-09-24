using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace TestProject;

public class Server
{
    public required string Name { get; set; }
    public required string IpAddress { get; set; }
    public int Port { get; set; }
    public required string GameVersion { get; set; }
    public double UsageRAM { get; set; }
    public double UsageCPU { get; set; }
    public bool PVPIsAllow { get; set; }
    public int OnlinePlayers { get; set; }
    public int CountPlayers { get; set; }
    public int MaxOnline { get; set; }
    public List<Player>? Players { get; set; }
}

public class Player
{
    public string? Login { get; set; }
    public string? Level { get; set; }
    public (double x, double y) Position { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
}

public class Lab1
{
    private static readonly JsonSerializerOptions json_options = new JsonSerializerOptions { WriteIndented = true };

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public static string GenerateRandomLogin()
    {
        var alphabet_codes = Enumerable.Range(65, 26).Concat(Enumerable.Range(97, 26)).ToList();
        var chars = alphabet_codes.Select(x => (char)x).ToList();
        var result = "";
        for (var i = 0; i < 10; i++)
        {
            result += chars[Random.Shared.Next(chars.Count)];
        }
        return result;
    }

    public static DateTime GenerateRandomLastSeen()
    {
        var startDate = new DateTime(2026, 9, 1);
        var endDate = DateTime.Now;

        var range = endDate - startDate;
        var randomDate = startDate.AddSeconds(Random.Shared.NextInt64((long)range.TotalSeconds));
        return randomDate;
    }

    public static void Main()
    {
        // players gen
        var players = new List<Player>();
        var players_generate = Random.Shared.Next(5, 10);
        if (File.Exists("players.json"))
        {
            var playersData = File.ReadAllText("players.json");
            players = JsonSerializer.Deserialize<List<Player>>(playersData) ?? [];
            players_generate = 2;
        }

        for (var _ = 0; _ < players_generate; _++)
        {
            var is_online = Random.Shared.Next(0, 2) == 1;
            var login = Enumerable.Range(0, int.MaxValue)
                .Select(_ => GenerateRandomLogin())
                .First(login => !players.Any(player => player.Login == login));
            players.Add(new Player
            {
                Login = login,
                Level = $"Level #{Random.Shared.Next(99)}",
                Position = (x: Random.Shared.NextDouble() * 180 - 90, y: Random.Shared.NextDouble() * 360 - 180),
                IsOnline = is_online,
                LastSeen = GenerateRandomLastSeen()
            });
        }
        var players_json = JsonSerializer.Serialize(players, Lab1.json_options);
        File.WriteAllText("players.json", players_json);


        // server gen
        var online_players = players.Count(player => player.IsOnline);
        var max_online = online_players;
        var server = new Server{
            Name = "CSU Game Server",
            IpAddress = GetLocalIPAddress(),
            GameVersion = "1.21.1",
        };
        if (File.Exists("server.json"))
        {
            var serverData = File.ReadAllText("server.json");
            server = JsonSerializer.Deserialize<Server>(serverData) ?? server;
            max_online = server.MaxOnline;
            if (online_players >= server.MaxOnline) { max_online = online_players; }
            Console.WriteLine("=======  LAST START SERVER DATA  =======");
            Console.WriteLine($"Name: {server.Name}");
            Console.WriteLine($"IP Address: {server.IpAddress}");
            Console.WriteLine($"Port: {server.Port}");
            Console.WriteLine($"Game Version: {server.GameVersion}");
            Console.WriteLine($"RAM Usage: {server.UsageRAM}% / 100%");
            Console.WriteLine($"CPU Usage: {server.UsageCPU}% / 100%");
            Console.WriteLine($"PVP enables: " + (server.PVPIsAllow ? "Yes" : "No"));
            Console.WriteLine($"Online players: {server.OnlinePlayers} / {server.CountPlayers}");
            Console.WriteLine($"Max Online: {server.MaxOnline}");
        }

        server = new Server
        {
            Name = "CSU Game Server",
            IpAddress = GetLocalIPAddress(),
            Port = 25565,
            GameVersion = "1.21.1",
            UsageRAM = Math.Round(Random.Shared.NextDouble() * 100, 2),
            UsageCPU = Math.Round(Random.Shared.NextDouble() * 100, 2),
            PVPIsAllow = true,
            OnlinePlayers = online_players,
            CountPlayers = players.Count,
            MaxOnline = max_online,
            Players = players
        };
        Console.WriteLine("\n======= GAME SERVER =======");
        Console.WriteLine($"Name: {server.Name}");
        Console.WriteLine($"IP Address: {server.IpAddress}");
        Console.WriteLine($"Port: {server.Port}");
        Console.WriteLine($"Game Version: {server.GameVersion}");
        Console.WriteLine($"RAM Usage: {server.UsageRAM}% / 100%");
        Console.WriteLine($"CPU Usage: {server.UsageCPU}% / 100%");
        Console.WriteLine($"PVP enables: " + (server.PVPIsAllow ? "Yes" : "No"));
        Console.WriteLine($"Online players: {server.OnlinePlayers} / {server.CountPlayers}");
        Console.WriteLine($"Max Online: {server.MaxOnline}");
        var server_json = JsonSerializer.Serialize(server, Lab1.json_options);
        File.WriteAllText("server.json", server_json);


        // я сделал это за 4 с половиной часа до уезда из города, поэтому тупо в лоб
        // если было бы чуть больше времени, сделал бы выбор действия с возможностью показать список юзеров(1) или выйти(2)
        Console.WriteLine("\n=======   PLAYERS  =======");
        var index = 1;
        foreach (Player player in players)
        {
            var index_string = $"{index}. ";
            Console.WriteLine(index_string + $"Login: {player.Login}");
            Console.WriteLine(new string(' ', index_string.Length) + $"Level: {player.Level}");
            Console.WriteLine(new string(' ', index_string.Length) + $"Position: ({player.Position.x}, {player.Position.y})");
            Console.WriteLine(new string(' ', index_string.Length) + $"Is online: " + (player.IsOnline ? "Yes" : "No"));
            Console.WriteLine(new string(' ', index_string.Length) + $"Last seen: " + (player.IsOnline ? "User is online" : player.LastSeen.ToString("dd.MM.yyyy HH:mm")));
            index++;
        }

        Console.ReadKey();
    }
}