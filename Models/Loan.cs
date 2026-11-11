namespace LibraryManagementSystem.Models;

public class Loan
{
    public int Id { get; }
    public int ItemId { get; }
    public int MemberId { get; }
    public DateTime BorrowDate { get; }
    public DateTime DueDate { get; }
    public DateTime? ReturnDate { get; private set; }
    public decimal FineAmount { get; private set; }
    public bool IsReturned => ReturnDate.HasValue;

    public Loan(int id, int itemId, int memberId, DateTime borrowDate, DateTime dueDate)
    {
        Id = id; ItemId = itemId; MemberId = memberId;
        BorrowDate = borrowDate; DueDate = dueDate;
    }

    public void CompleteReturn(DateTime returnDate, decimal fine)
    {
        if (IsReturned) throw new InvalidOperationException("This loan has already been returned.");
        ReturnDate = returnDate; FineAmount = fine;
    }

    public override string ToString() =>
        $"Loan #{Id} | Item #{ItemId} | Member #{MemberId} | Borrowed: {BorrowDate:d} | Due: {DueDate:d} | Returned: {(ReturnDate.HasValue ? ReturnDate.Value.ToShortDateString() : "No")} | Fine: {FineAmount:C}";
}
