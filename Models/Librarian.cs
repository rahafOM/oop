namespace LibraryManagementSystem.Models;

public class Librarian : Person
{
    public string Username { get; private set; }
    public string Password { get; private set; }

    public Librarian(int id, string name, string email, string username, string password)
        : base(id, name, email)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password are required.");
        Username = username.Trim(); Password = password;
    }

    public bool VerifyPassword(string password) => Password == password;
}
