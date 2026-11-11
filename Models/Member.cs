namespace LibraryManagementSystem.Models;

public class Member : Person
{
    public bool IsActive { get; private set; } = true;
    public int MaxLoans { get; private set; }
    public decimal OutstandingFine { get; internal set; }

    public Member(int id, string name, string email, int maxLoans = 3) : base(id, name, email)
    {
        if (maxLoans <= 0) throw new ArgumentException("Maximum loans must be positive.");
        MaxLoans = maxLoans;
    }

    public void SetMaxLoans(int limit)
    {
        if (limit <= 0) throw new ArgumentException("Maximum loans must be positive.");
        MaxLoans = limit;
    }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public override string ToString() =>
        $"Member #{Id} | {Name} | {Email} | Active: {IsActive} | Fine due: {OutstandingFine:C}";
}
