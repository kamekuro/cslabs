using ServerConfig;
namespace ServerConfig.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 2)]
    [InlineData(11, 4)]
    [InlineData(25, 4)]
    [InlineData(26, 8)]
    [InlineData(50, 8)]
    [InlineData(51, 12)]
    [InlineData(75, 12)]
    [InlineData(76, 16)]
    [InlineData(100, 16)]
    [InlineData(101, 24)]
    [InlineData(150, 24)]
    [InlineData(151, 32)]
    [InlineData(200, 32)]
    [InlineData(201, 48)]
    [InlineData(300, 48)]
    [InlineData(301, 64)]
    public void GetRecommendedRAM_ReturnsCorrectRAM(int maxPlayers, int expected)
    {
        var result = Lab1.GetRecommendedRAM(maxPlayers);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1023)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void CheckPort_ReturnsErrorForInvalidPort(int port)
    {
        var result = Lab1.CheckPort(port);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Error, result.Type);
        Assert.Equal("Port must be between 1024 and 65535", result.Message);
    }

    [Theory]
    [InlineData(1024)]
    [InlineData(25565)]
    [InlineData(65535)]
    public void CheckPort_ReturnsNullForValidPort(int port)
    {
        var result = Lab1.CheckPort(port);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CheckMaxPlayers_ReturnsErrorForInvalidValue(int maxPlayers)
    {
        var result = Lab1.CheckMaxPlayers(maxPlayers);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Error, result.Type);
        Assert.Equal("Maximum number of players must be greater than 0", result.Message);
    }

    [Fact]
    public void CheckMaxPlayers_ReturnsNullForValidValue()
    {
        var result = Lab1.CheckMaxPlayers(1);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void CheckMinRAM_ReturnsErrorWhenRAMIsLessThan2(int RAM)
    {
        var result = Lab1.CheckMinRAM(RAM);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Error, result.Type);
        Assert.Equal("Minimum of 2 GB of RAM is required to start the server", result.Message);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void CheckMinRAM_ReturnsNullWhenRAMIsAtLeast2(int RAM)
    {
        var result = Lab1.CheckMinRAM(RAM);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(4, 51)]
    [InlineData(7, 100)]
    [InlineData(1, 51)]
    public void CheckMinRAMForPlayers_ReturnsErrorWhenMoreThan50PlayersAndLessThan8GB(int RAM, int maxPlayers)
    {
        var result = Lab1.CheckMinRAMForPlayers(RAM, maxPlayers);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Error, result.Type);
        Assert.Equal(
            "Server with more than 50 players requires a minimum of 8 GB of RAM",
            result.Message
        );
    }

    [Theory]
    [InlineData(8, 51)]
    [InlineData(4, 50)]
    [InlineData(2, 1)]
    public void CheckMinRAMForPlayers_ReturnsNullWhenConfigurationIsValid(int RAM, int maxPlayers)
    {
        var result = Lab1.CheckMinRAMForPlayers(RAM, maxPlayers);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(1, 10, 2)]
    [InlineData(3, 25, 4)]
    [InlineData(7, 50, 8)]
    public void CheckRecommendedRAM_ReturnsWarningWhenRAMIsBelowRecommended(int RAM, int maxPlayers, int recommendedRAM)
    {
        var result = Lab1.CheckRecommendedRAM(RAM, maxPlayers);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Warning, result.Type);
        Assert.Equal(
            $"For {maxPlayers} players, at least {recommendedRAM} GB of RAM is recommended. Currently allocated: {RAM} GB",
            result.Message
        );
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(2, 10)]
    [InlineData(4, 25)]
    [InlineData(8, 50)]
    public void CheckRecommendedRAM_ReturnsNullWhenRAMIsEnough(int RAM, int maxPlayers)
    {
        var result = Lab1.CheckRecommendedRAM(RAM, maxPlayers);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(2, true)]
    [InlineData(1, true)]
    public void CheckMinRAMForMods_ReturnsWarningWhenModsEnabledAndRAMIsBelow4(int RAM, bool modsEnabled)
    {
        var result = Lab1.CheckMinRAMForMods(RAM, modsEnabled);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Warning, result.Type);
        Assert.Equal(
            "Modifications may result in a lack of memory when allocating less than 4 GB",
            result.Message
        );
    }

    [Theory]
    [InlineData(4, true)]
    [InlineData(2, false)]
    [InlineData(8, false)]
    public void CheckMinRAMForMods_ReturnsNullWhenConditionIsNotMet(int RAM, bool modsEnabled)
    {
        var result = Lab1.CheckMinRAMForMods(RAM, modsEnabled);

        Assert.Null(result);
    }

    [Fact]
    public void CheckModsOnPublic_ReturnsWarningWhenPublicServerHasMods()
    {
        var result = Lab1.CheckModsOnPublic(true, true);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Warning, result.Type);
        Assert.Equal(
            "Modifications on the public server may not be applicable for some players",
            result.Message
        );
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void CheckModsOnPublic_ReturnsNullWhenConditionIsNotMet(bool isPublic, bool modsEnabled)
    {
        var result = Lab1.CheckModsOnPublic(isPublic, modsEnabled);

        Assert.Null(result);
    }

    [Fact]
    public void CheckBackups_ReturnsWarningWhenPublicServerHasNoBackups()
    {
        var result = Lab1.CheckBackups(true, false);

        Assert.NotNull(result);
        Assert.Equal(ProblemType.Warning, result.Type);
        Assert.Equal(
            "You disabled backups for public server",
            result.Message
        );
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void CheckBackups_ReturnsNullWhenConditionIsNotMet(bool isPublic, bool backupEnabled)
    {
        var result = Lab1.CheckBackups(isPublic, backupEnabled);

        Assert.Null(result);
    }

    [Fact]
    public void CheckConfiguration_ReturnsAllProblems()
    {
        var result = Lab1.CheckConfiguration(
            maxPlayers: 100,
            RAM: 1,
            port: 80,
            isPublic: true,
            modsEnabled: true,
            backupEnabled: false
        );

        Assert.Equal(7, result.Count);

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Error &&
            problem.Message == "Port must be between 1024 and 65535");

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Error &&
            problem.Message == "Minimum of 2 GB of RAM is required to start the server");

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Error &&
            problem.Message == "Server with more than 50 players requires a minimum of 8 GB of RAM");

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Warning &&
            problem.Message.Contains("at least 16 GB of RAM is recommended"));

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Warning &&
            problem.Message == "Modifications may result in a lack of memory when allocating less than 4 GB");

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Warning &&
            problem.Message == "Modifications on the public server may not be applicable for some players");

        Assert.Contains(result, problem =>
            problem.Type == ProblemType.Warning &&
            problem.Message == "You disabled backups for public server");
    }

    [Fact]
    public void CheckConfiguration_ReturnsEmptyListForValidConfiguration()
    {
        var result = Lab1.CheckConfiguration(
            maxPlayers: 10,
            RAM: 2,
            port: 25565,
            isPublic: false,
            modsEnabled: false,
            backupEnabled: true
        );

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("-10", -10)]
    [InlineData("0", 0)]
    public void ReadInt_ReturnsParsedValue(string input, int expected)
    {
        Console.SetIn(new StringReader(input));

        var result = Lab1.ReadInt(100);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("abc", 100)]
    [InlineData("", 100)]
    [InlineData("   ", 100)]
    public void ReadInt_ReturnsDefaultValueWhenInputIsInvalid(string input, int expected)
    {
        Console.SetIn(new StringReader(input));

        var result = Lab1.ReadInt(100);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("y", true)]
    [InlineData("yes", true)]
    [InlineData("1", true)]
    [InlineData("true", true)]
    [InlineData("t", true)]
    [InlineData("Y", true)]
    [InlineData(" YES ", true)]
    public void ReadBool_ReturnsTrueForTrueInput(string input, bool expected)
    {
        Console.SetIn(new StringReader(input));

        var result = Lab1.ReadBool(false, ["y", "yes", "1", "true", "t"]);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("n", false)]
    [InlineData("no", false)]
    [InlineData("false", false)]
    [InlineData("abc", false)]
    public void ReadBool_ReturnsFalseForFalseInput(string input, bool expected)
    {
        Console.SetIn(new StringReader(input));

        var result = Lab1.ReadBool(true, ["y", "yes", "1", "true", "t"]);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ReadBool_ReturnsDefaultValueForEmptyInput()
    {
        Console.SetIn(new StringReader(""));

        var result = Lab1.ReadBool(true, ["y", "yes", "1", "true", "t"]);

        Assert.True(result);
    }
}