namespace LibraryManagementSystem.Models;

public class Book : LibraryItem
{
    public string Author { get; private set; }
    public int Pages { get; private set; }

    public Book(int id, string title, string author, int pages) : base(id, title)
    {
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required.");
        if (pages <= 0) throw new ArgumentException("Pages must be positive.");
        Author = author.Trim();
        Pages = pages;
    }

    public override int GetLoanPeriod() => 14;
    public override decimal CalculateLateFee(int lateDays) => Math.Max(0, lateDays) * 2m;
    public void UpdateDetails(string author, int pages)
    {
        if (string.IsNullOrWhiteSpace(author) || pages <= 0) throw new ArgumentException("Invalid book details.");
        Author = author.Trim(); Pages = pages;
    }
    public override string GetDetails() => base.GetDetails() + $" | Author: {Author} | Pages: {Pages}";
}
