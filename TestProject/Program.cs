using System;

namespace TestProject;

class Lab0
{
    public static void Main()
    {
        Console.WriteLine(
            "Hello and welcome to the Amazing Digital f*ckingsh*t! "
            + "You should tell me some information about you"
        );

        Console.Write("Enter your nickname: ");
        string name = Console.ReadLine();

        Console.Write("Enter your e-mail: ");
        string email = Console.ReadLine();

        Console.Write("Enter your age (if its not number, it would be 0): ");
        string buffer = Console.ReadLine();
        bool success = int.TryParse(buffer, out int age);

        int rand_lvl = Random.Shared.Next(99);
        int max_xp = 100 * rand_lvl / 10;
        int xp = Random.Shared.Next(1, max_xp);

        Console.Clear();
        Console.WriteLine($"Good! There is your Profile:\n");
        Console.WriteLine($"╔ Nickname: {name}");
        Console.WriteLine($"╠ E-mail: {email}");
        Console.WriteLine($"╠ LVL: {rand_lvl}");
        Console.WriteLine($"╠ XP: {xp} / {max_xp}");
        Console.WriteLine($"╠ E-mail: {email}");
        Console.WriteLine($"╚ Age: {age}");

        Console.ReadKey();
    }
}