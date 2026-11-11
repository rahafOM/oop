using LibraryManagementSystem.Services;
using LibraryManagementSystem.UI;

namespace LibraryManagementSystem;

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var library = new LibraryService();
        var menu = new Menu(library);

        Console.WriteLine("Welcome to the Library Management System");
        Console.WriteLine("Demo librarian login: admin / admin123");
        Console.WriteLine("A demo member is also created with ID 1.");
        ConsoleHelper.Pause();
        menu.Run();
    }
}
