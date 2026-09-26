/*
    ПРЕДУПРЕЖДЕНИЕ (TL;DR maybe?)
    я признаю, что мой код — говнокод
    ОДНАКО это мой говнокод и это мои ошибки, на которых я и привык учить новые языки
*/

/*
    Дополненный код с лабы 00(01) (мониторинг сервера)
    ((чтобы не переписывать структуру сервера, ведь куда проще её дополнить, удалив ненужное, например: список игроков)) 
*/

using System.Net;
using System.Net.Sockets;

namespace ServerConfig;

public enum ProblemType
{
    Error,
    Warning
}

public class ConfigProblem
{
    public ProblemType Type { get; }
    public string Message { get; }

    public ConfigProblem(ProblemType type, string message)
    {
        Type = type;
        Message = message;
    }
}

public class Lab1
{
    public static int GetRecommendedRAM(int maxPlayers)
    {
        return maxPlayers switch
        {
            <= 10 => 2,
            <= 25 => 4,
            <= 50 => 8,
            <= 75 => 12,
            <= 100 => 16,
            <= 150 => 24,
            <= 200 => 32,
            <= 300 => 48,
            _ => 64
        };
    }

    // checks
    public static ConfigProblem? CheckPort(int port)
    {
        return port < 1024 || port > 65535
            ? new ConfigProblem(ProblemType.Error, "Port must be between 1024 and 65535")
            : null;
    }
    public static ConfigProblem? CheckMaxPlayers(int maxPlayers)
    {
        return maxPlayers <= 0
            ? new ConfigProblem(ProblemType.Error, "Maximum number of players must be greater than 0")
            : null;
    }
    public static ConfigProblem? CheckMinRAM(int RAM)
    {
        return RAM < 2
            ? new ConfigProblem(ProblemType.Error, "Minimum of 2 GB of RAM is required to start the server")
            : null;
    }
    public static ConfigProblem? CheckMinRAMForPlayers(int RAM, int maxPlayers)
    {
        return maxPlayers > 50 && RAM < 8
            ? new ConfigProblem(ProblemType.Error, "Server with more than 50 players requires a minimum of 8 GB of RAM")
            : null;
    }
    public static ConfigProblem? CheckRecommendedRAM(int RAM, int maxPlayers)
    {
        var recommendedRamGb = GetRecommendedRAM(maxPlayers);
        return RAM < recommendedRamGb
            ? new ConfigProblem(ProblemType.Warning, $"For {maxPlayers} players, at least {recommendedRamGb} GB of RAM is recommended. Currently allocated: {RAM} GB")
            : null;
    }
    public static ConfigProblem? CheckMinRAMForMods(int RAM, bool modsEnabled)
    {
        return modsEnabled && RAM < 4
            ? new ConfigProblem(ProblemType.Warning, "Modifications may result in a lack of memory when allocating less than 4 GB")
            : null;
    }
    public static ConfigProblem? CheckModsOnPublic(bool isPublic, bool modsEnabled)
    {
        return (isPublic && modsEnabled)
            ? new ConfigProblem(ProblemType.Warning, "Modifications on the public server may not be applicable for some players")
            : null;
    }
    public static ConfigProblem? CheckBackups(bool isPublic, bool backupEnabled)
    {
        return (isPublic && !backupEnabled)
            ? new ConfigProblem(ProblemType.Warning, "You disabled backups for public server")
            : null;
    }

    public static List<ConfigProblem> CheckConfiguration(
        int maxPlayers,
        int RAM,
        int port,
        bool isPublic,
        bool modsEnabled,
        bool backupEnabled
    )
    {
        var logs = new List<ConfigProblem>();
        if (CheckPort(port) is ConfigProblem _port_log) { logs.Add(_port_log); }
        if (CheckMaxPlayers(maxPlayers) is ConfigProblem _max_players_log) { logs.Add(_max_players_log); }
        if (CheckMinRAM(RAM) is ConfigProblem _min_ram_log) { logs.Add(_min_ram_log); }
        if (CheckMinRAMForPlayers(RAM, maxPlayers) is ConfigProblem _min_ram_for_players_log) { logs.Add(_min_ram_for_players_log); }
        if (CheckRecommendedRAM(RAM, maxPlayers) is ConfigProblem _recommended_ram_log) { logs.Add(_recommended_ram_log); }
        if (CheckMinRAMForMods(RAM, modsEnabled) is ConfigProblem _min_ram_for_mods_log) { logs.Add(_min_ram_for_mods_log); }
        if (CheckModsOnPublic(isPublic, modsEnabled) is ConfigProblem _mods_on_public_log) { logs.Add(_mods_on_public_log); }
        if (CheckBackups(isPublic, backupEnabled) is ConfigProblem _backups_log) { logs.Add(_backups_log); }
        return logs;
    }

    public static int ReadInt(int defaultValue) {return int.TryParse(Console.ReadLine(), out var result) ? result : defaultValue;}
    public static bool ReadBool(bool defaultValue, List<string> bools)
    {
        var input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? defaultValue : bools.Contains(input.Trim().ToLower());
    }

    public static void Main()
    {
        Console.WriteLine("Server Config checking Program");
        Console.Write("Enter count of max players (default is 1): "); var maxPlayers = ReadInt(1);
        Console.Write("Enter allocated RAM (GB) (default is 2): "); var RAM = ReadInt(2);
        Console.Write("Enter game port (default is 25565): "); var port = ReadInt(25565);
        Console.Write("Is your server public [Y/n]? "); var isPublic = ReadBool(true, ["y", "yes", "1", "true", "t"]);
        Console.Write("Do you want to enable modifications [Y/n]? "); var modsEnabled = ReadBool(true, ["y", "yes", "1", "true", "t"]);
        Console.Write("Do you want to enable backups [Y/n]? "); var backupEnabled = ReadBool(true, ["y", "yes", "1", "true", "t"]);

        var logs = CheckConfiguration(maxPlayers, RAM, port, isPublic, modsEnabled, backupEnabled);
        Console.WriteLine("\n"); // noqa
        foreach (var log in logs)
        {
            string err_type = (log.Type == ProblemType.Warning) ? "WARN" : "ERROR";
            Console.WriteLine($"[{err_type}] {log.Message}");
        }
        if (logs.Any(problem => problem.Type == ProblemType.Error))
        {
            Console.WriteLine("\nAfter checking config, there was found any errors. You should fix it.");
            Console.WriteLine("Server was not started.");
        } else if (logs.Any(problem => problem.Type == ProblemType.Warning))
        {
            Console.Write("\nAfter checking config, there was found any warnings. Continue [y/N]? ");
            Console.WriteLine(
                ReadBool(false, ["y", "yes", "1", "true", "t"]) ? "Server was started." : "Server was not started."
            );
        } else
        {
            Console.WriteLine("\nAll done, have a good day!");
            Console.WriteLine("Server was started.");
        }

        Console.ReadKey(); // можно скипнуть, но я сделал «автозакрытие консоли после выполнения» (чтобы в консоли не было мусора помимо моих выводов) => в консоли нет мусора и она не закрывается сразу
    }
}