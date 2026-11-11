using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.UI;

public class Menu
{
    private readonly LibraryService _library;
    public Menu(LibraryService library) => _library = library;

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== LIBRARY MANAGEMENT SYSTEM =====");
            Console.WriteLine("1. Login (Librarian / Member)");
            Console.WriteLine("2. Register New Member");
            Console.WriteLine("3. Browse Library");
            Console.WriteLine("4. Search for Items");
            Console.WriteLine("5. Exit");
            string choice = ConsoleHelper.ReadRequired("Choose: ");

            try
            {
                switch (choice)
                {
                    case "1": Login(); break;
                    case "2": RegisterMember(); break;
                    case "3": Browse(); break;
                    case "4": Search(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice."); ConsoleHelper.Pause(); break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); ConsoleHelper.Pause(); }
        }
    }

    private void Login()
    {
        Console.WriteLine("1. Librarian login");
        Console.WriteLine("2. Member login");
        switch (ConsoleHelper.ReadInt("Choose account type: ", 1))
        {
            case 1:
                string username = ConsoleHelper.ReadRequired("Username: ");
                string password = ConsoleHelper.ReadRequired("Password: ");
                if (_library.Login(username, password) == null)
                {
                    Console.WriteLine("Invalid login.");
                    ConsoleHelper.Pause();
                    return;
                }
                AdminMenu();
                break;
            case 2:
                int memberId = ConsoleHelper.ReadInt("Membership ID: ");
                string email = ConsoleHelper.ReadRequired("Email: ");
                var member = _library.LoginMember(memberId, email);
                if (member == null)
                {
                    Console.WriteLine("Invalid member login or inactive membership.");
                    ConsoleHelper.Pause();
                    return;
                }
                MemberMenu(member);
                break;
            default:
                Console.WriteLine("Invalid account type.");
                ConsoleHelper.Pause();
                break;
        }
    }

    private void RegisterMember()
    {
        string name = ConsoleHelper.ReadRequired("Full name: ");
        string email = ConsoleHelper.ReadRequired("Email: ");
        var member = _library.RegisterMember(name, email);
        Console.WriteLine($"Member registered. Membership ID: {member.Id}");
        ConsoleHelper.Pause();
    }

    private void Browse()
    {
        ConsoleHelper.PrintItems(_library.Items.Where(i => i.IsActive));
        ConsoleHelper.Pause();
    }

    private void Search()
    {
        string query = ConsoleHelper.ReadRequired("Enter title or item ID: ");
        if (int.TryParse(query, out int id))
        {
            var item = _library.SearchItem(id);
            Console.WriteLine(item?.GetDetails() ?? "No item found.");
        }
        else ConsoleHelper.PrintItems(_library.SearchItem(query));
        ConsoleHelper.Pause();
    }

    private void AdminMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== LIBRARIAN MENU =====");
            Console.WriteLine("1. Add item");
            Console.WriteLine("2. View all items");
            Console.WriteLine("3. View available items");
            Console.WriteLine("4. Update item title");
            Console.WriteLine("5. Deactivate item");
            Console.WriteLine("6. View members");
            Console.WriteLine("7. Deactivate member");
            Console.WriteLine("8. Set member loan limit");
            Console.WriteLine("9. Borrow item for member");
            Console.WriteLine("10. Return item");
            Console.WriteLine("11. View current loans");
            Console.WriteLine("12. View member details and history");
            Console.WriteLine("13. Pay fine");
            Console.WriteLine("14. Reports");
            Console.WriteLine("15. Logout");
            string choice = ConsoleHelper.ReadRequired("Choose: ");

            try
            {
                switch (choice)
                {
                    case "1": AddItem(); break;
                    case "2": ConsoleHelper.PrintItems(_library.Items); ConsoleHelper.Pause(); break;
                    case "3": ConsoleHelper.PrintItems(_library.AvailableItems()); ConsoleHelper.Pause(); break;
                    case "4": UpdateItem(); break;
                    case "5": _library.DeactivateItem(ConsoleHelper.ReadInt("Item ID: ")); Done(); break;
                    case "6": foreach (var m in _library.Members) Console.WriteLine(m); ConsoleHelper.Pause(); break;
                    case "7": _library.DeactivateMember(ConsoleHelper.ReadInt("Member ID: ")); Done(); break;
                    case "8": SetMemberLoanLimit(); break;
                    case "9": Borrow(); break;
                    case "10": Return(); break;
                    case "11": foreach (var l in _library.ActiveLoans()) Console.WriteLine(l); ConsoleHelper.Pause(); break;
                    case "12": History(); break;
                    case "13": PayFine(); break;
                    case "14": Reports(); break;
                    case "15": return;
                    default: Console.WriteLine("Invalid choice."); ConsoleHelper.Pause(); break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); ConsoleHelper.Pause(); }
        }
    }

    private void MemberMenu(Member member)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"===== MEMBER MENU: {member.Name} =====");
            Console.WriteLine("1. Browse library");
            Console.WriteLine("2. Search for items");
            Console.WriteLine("3. View my account and loan history");
            Console.WriteLine("4. Logout");
            string choice = ConsoleHelper.ReadRequired("Choose: ");
            switch (choice)
            {
                case "1": Browse(); break;
                case "2": Search(); break;
                case "3": ShowMemberAccount(member); break;
                case "4": return;
                default: Console.WriteLine("Invalid choice."); ConsoleHelper.Pause(); break;
            }
        }
    }

    private void ShowMemberAccount(Member member)
    {
        Console.WriteLine(member);
        Console.WriteLine($"Loan limit: {member.MaxLoans}");
        Console.WriteLine($"Total fines paid: {_library.MemberPaidTotal(member.Id):C}");
        Console.WriteLine("Loan history:");
        foreach (var loan in _library.MemberHistory(member.Id)) Console.WriteLine(loan);
        Console.WriteLine("Payments:");
        foreach (var payment in _library.MemberPayments(member.Id)) Console.WriteLine(payment);
        ConsoleHelper.Pause();
    }

    private void SetMemberLoanLimit()
    {
        int memberId = ConsoleHelper.ReadInt("Member ID: ");
        int limit = ConsoleHelper.ReadInt("New maximum loans: ");
        _library.SetMemberLoanLimit(memberId, limit);
        Done();
    }

    private void AddItem()
    {
        Console.WriteLine("1. Book  2. Magazine  3. Research paper");
        string type = ConsoleHelper.ReadRequired("Type: ");
        string title = ConsoleHelper.ReadRequired("Title: ");
        LibraryItem item = type switch
        {
            "1" => _library.AddBook(title, ConsoleHelper.ReadRequired("Author: "), ConsoleHelper.ReadInt("Pages: ")),
            "2" => _library.AddMagazine(title, ConsoleHelper.ReadInt("Issue number: ")),
            "3" => _library.AddResearchPaper(title, ConsoleHelper.ReadRequired("Researcher: "), ConsoleHelper.ReadRequired("Research field: ")),
            _ => throw new ArgumentException("Unknown item type.")
        };
        Console.WriteLine($"Added item with ID {item.Id}.");
        ConsoleHelper.Pause();
    }


    private void UpdateItem()
    {
        int id = ConsoleHelper.ReadInt("Item ID: ");
        var item = _library.SearchItem(id) ?? throw new InvalidOperationException("Item not found.");
        string title = ConsoleHelper.ReadRequired("New title: ");

        // Update type-specific information while keeping each model responsible for its own validation.
        switch (item)
        {
            case Book book:
                book.UpdateDetails(ConsoleHelper.ReadRequired("Author: "), ConsoleHelper.ReadInt("Pages: "));
                break;
            case Magazine magazine:
                magazine.UpdateDetails(ConsoleHelper.ReadInt("Issue number: "));
                break;
            case ResearchPaper paper:
                paper.UpdateDetails(ConsoleHelper.ReadRequired("Researcher: "), ConsoleHelper.ReadRequired("Research field: "));
                break;
        }

        // Update the title last so invalid type-specific details don't partially update the item.
        item.UpdateTitle(title);

        Console.WriteLine("Item details updated.");
        ConsoleHelper.Pause();
    }

    private void Borrow()
    {
        var loan = _library.BorrowItem(ConsoleHelper.ReadInt("Member ID: "), ConsoleHelper.ReadInt("Item ID: "));
        Console.WriteLine($"Loan #{loan.Id} created. Due date: {loan.DueDate:d}");
        ConsoleHelper.Pause();
    }

    private void Return()
    {
        int id = ConsoleHelper.ReadInt("Loan ID: ");
        decimal fine = _library.ReturnItem(id);
        Console.WriteLine($"Item returned. Late fee: {fine:C}");
        ConsoleHelper.Pause();
    }

    private void History()
    {
        int id = ConsoleHelper.ReadInt("Member ID: ");
        var member = _library.FindMember(id) ?? throw new InvalidOperationException("Member not found.");
        Console.WriteLine(member);
        foreach (var loan in _library.MemberHistory(id)) Console.WriteLine(loan);
        ConsoleHelper.Pause();
    }

    private void PayFine()
    {
        int memberId = ConsoleHelper.ReadInt("Member ID: ");
        decimal amount = ConsoleHelper.ReadDecimal("Payment amount: ");
        string reference = ConsoleHelper.ReadRequired("Unique payment reference: ");
        decimal remaining = _library.PayFine(memberId, amount, reference);
        Console.WriteLine($"Payment accepted. Remaining balance: {remaining:C}");
        ConsoleHelper.Pause();
    }

    private void Reports()
    {
        Console.WriteLine("\n--- REPORTS ---");
        Console.WriteLine($"Active items: {_library.Items.Count(i => i.IsActive)}");
        Console.WriteLine($"Available items: {_library.AvailableItems().Count}");
        Console.WriteLine($"Currently borrowed: {_library.ActiveLoans().Count()}");
        Console.WriteLine("\nMost borrowed:");
        foreach (var entry in _library.MostBorrowed()) Console.WriteLine($"{entry.Item.Title}: {entry.Count}");
        Console.WriteLine("\nMembers with overdue loans:");
        foreach (var member in _library.MembersWithOverdueLoans()) Console.WriteLine(member);
        Console.WriteLine("Overdue loan details:");
        foreach (var loan in _library.OverdueLoans()) Console.WriteLine(loan);
        Console.WriteLine("\nMembers with unpaid fines:");
        foreach (var member in _library.MembersWithUnpaidFines()) Console.WriteLine(member);
        Console.WriteLine($"\nTotal fines collected: {_library.TotalCollected:C}");
        Console.WriteLine($"Total outstanding fines: {_library.TotalOutstanding:C}");
        ConsoleHelper.Pause();
    }

    private static void Done() { Console.WriteLine("Operation completed."); ConsoleHelper.Pause(); }
}
