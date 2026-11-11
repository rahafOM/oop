using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.UI;

public static class ConsoleHelper
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            Console.WriteLine("This field is required.");
        }
    }

    public static int ReadInt(string prompt, int min = 1)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min) return value;
            Console.WriteLine($"Enter a valid number (minimum {min}).");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = 0.01m)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= min) return value;
            Console.WriteLine($"Enter a valid amount (minimum {min}).");
        }
    }

    public static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    public static void PrintItems(IEnumerable<LibraryItem> items)
    {
        foreach (var item in items) Console.WriteLine(item.GetDetails());
    }
}
