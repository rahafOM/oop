namespace LibraryManagementSystem.Models;

public class Magazine : LibraryItem
{
    public int IssueNumber { get; private set; }

    public Magazine(int id, string title, int issueNumber) : base(id, title)
    {
        if (issueNumber <= 0) throw new ArgumentException("Issue number must be positive.");
        IssueNumber = issueNumber;
    }

    public override int GetLoanPeriod() => 7;
    public override decimal CalculateLateFee(int lateDays) => Math.Max(0, lateDays) * 1m;
    public void UpdateDetails(int issueNumber)
    {
        if (issueNumber <= 0) throw new ArgumentException("Issue number must be positive.");
        IssueNumber = issueNumber;
    }
    public override string GetDetails() => base.GetDetails() + $" | Issue: {IssueNumber}";
}
