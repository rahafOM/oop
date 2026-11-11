namespace LibraryManagementSystem.Models;

public abstract class Person
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    protected Person(int id, string name, string email)
    {
        if (id <= 0) throw new ArgumentException("ID must be positive.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email is required.");
        Id = id; Name = name.Trim(); Email = email.Trim();
    }

    public void UpdateContact(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Invalid contact details.");
        Name = name.Trim(); Email = email.Trim();
    }
}
