namespace LibraryManagementSystem.Models;

public abstract class LibraryItem
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    protected LibraryItem(int id, string title)
    {
        if (id <= 0) throw new ArgumentException("Item ID must be positive.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        Id = id;
        Title = title.Trim();
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        Title = title.Trim();
    }

    public void SetAvailability(bool value) => IsAvailable = value;
    public void Deactivate() { IsActive = false; IsAvailable = false; }

    public abstract int GetLoanPeriod();
    public abstract decimal CalculateLateFee(int lateDays);

    public virtual string GetDetails() =>
        $"ID: {Id} | Title: {Title} | Available: {IsAvailable} | Active: {IsActive}";
}
