namespace LibraryManagementSystem.Models;

public class Payment
{
    public int Id { get; }
    public int MemberId { get; }
    public decimal Amount { get; }
    public DateTime PaymentDate { get; }
    public string Reference { get; }

    public Payment(int id, int memberId, decimal amount, string reference)
    {
        if (amount <= 0) throw new ArgumentException("Payment must be greater than zero.");
        if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Payment reference is required.");
        Id = id; MemberId = memberId; Amount = amount;
        Reference = reference.Trim(); PaymentDate = DateTime.Now;
    }

    public override string ToString() =>
        $"Payment #{Id} | Member #{MemberId} | {Amount:C} | {PaymentDate:g} | Ref: {Reference}";
}
