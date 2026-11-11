using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services;

public class LibraryService
{
    private readonly List<LibraryItem> _items = new();
    private readonly List<Member> _members = new();
    private readonly List<Librarian> _librarians = new();
    private readonly List<Loan> _loans = new();
    private readonly List<Payment> _payments = new();

    public IReadOnlyList<LibraryItem> Items => _items;
    public IReadOnlyList<Member> Members => _members;
    public IReadOnlyList<Loan> Loans => _loans;
    public IReadOnlyList<Payment> Payments => _payments;

    private int _nextItemId = 1, _nextMemberId = 1, _nextLoanId = 1, _nextPaymentId = 1;

    public LibraryService()
    {
        _librarians.Add(new Librarian(1, "System Librarian", "admin@library.local", "admin", "admin123"));
        AddSampleData();
    }

    private void AddSampleData()
    {
        AddBook("Clean Code", "Robert C. Martin", 464);
        AddMagazine("Science Today", 12);
        AddResearchPaper("Artificial Intelligence in Education", "Sara Ahmad", "Computer Science");
        RegisterMember("Demo Member", "member@example.com");
    }

    public Librarian? Login(string username, string password) =>
        _librarians.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                        && x.VerifyPassword(password));

    public Member? LoginMember(int memberId, string email) =>
        _members.FirstOrDefault(x => x.Id == memberId && x.IsActive
                                     && x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public Member RegisterMember(string name, string email, int maxLoans = 3)
    {
        if (_members.Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A member with this email already exists.");
        var member = new Member(_nextMemberId, name, email, maxLoans);
        _members.Add(member);
        _nextMemberId++;
        return member;
    }

    public LibraryItem AddBook(string title, string author, int pages)
    {
        var item = new Book(_nextItemId, title, author, pages); _items.Add(item); _nextItemId++; return item;
    }
    public LibraryItem AddMagazine(string title, int issue)
    {
        var item = new Magazine(_nextItemId, title, issue); _items.Add(item); _nextItemId++; return item;
    }
    public LibraryItem AddResearchPaper(string title, string researcher, string field)
    {
        var item = new ResearchPaper(_nextItemId, title, researcher, field); _items.Add(item); _nextItemId++; return item;
    }

    // Overloading: search by ID or by title.
    public LibraryItem? SearchItem(int id) => _items.FirstOrDefault(x => x.Id == id);
    public List<LibraryItem> SearchItem(string title) =>
        _items.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    public Member? FindMember(int id) => _members.FirstOrDefault(x => x.Id == id);
    public decimal MemberPaidTotal(int memberId) => _payments.Where(p => p.MemberId == memberId).Sum(p => p.Amount);
    public IEnumerable<Payment> MemberPayments(int memberId) => _payments.Where(p => p.MemberId == memberId);
    public void SetMemberLoanLimit(int memberId, int limit)
    {
        var member = FindMember(memberId) ?? throw new InvalidOperationException("Member not found.");
        member.SetMaxLoans(limit);
    }

    public List<LibraryItem> AvailableItems() => _items.Where(x => x.IsActive && x.IsAvailable).ToList();

    public void UpdateItemTitle(int id, string title)
    {
        var item = SearchItem(id) ?? throw new InvalidOperationException("Item not found.");
        item.UpdateTitle(title);
    }

    public void DeactivateItem(int id)
    {
        var item = SearchItem(id) ?? throw new InvalidOperationException("Item not found.");
        if (!item.IsAvailable) throw new InvalidOperationException("Return the item before deactivating it.");
        item.Deactivate();
    }

    public void DeactivateMember(int id)
    {
        var member = FindMember(id) ?? throw new InvalidOperationException("Member not found.");
        if (_loans.Any(l => l.MemberId == id && !l.IsReturned))
            throw new InvalidOperationException("Member has active loans and cannot be deactivated.");
        member.Deactivate();
    }

    public Loan BorrowItem(int memberId, int itemId, DateTime? borrowDate = null)
    {
        var member = FindMember(memberId) ?? throw new InvalidOperationException("Member not found.");
        var item = SearchItem(itemId) ?? throw new InvalidOperationException("Item not found.");

        if (!member.IsActive) throw new InvalidOperationException("Membership is inactive.");
        if (member.OutstandingFine > 0) throw new InvalidOperationException("Member has unpaid fines.");
        if (!item.IsActive || !item.IsAvailable) throw new InvalidOperationException("Item is not available.");
        int activeLoans = _loans.Count(l => l.MemberId == memberId && !l.IsReturned);
        if (activeLoans >= member.MaxLoans) throw new InvalidOperationException("Member reached the loan limit.");

        DateTime start = (borrowDate ?? DateTime.Today).Date;
        var loan = new Loan(_nextLoanId, item.Id, member.Id, start, start.AddDays(item.GetLoanPeriod()));
        _loans.Add(loan);
        _nextLoanId++;
        item.SetAvailability(false);
        return loan;
    }

    public decimal ReturnItem(int loanId, DateTime? returnDate = null)
    {
        var loan = _loans.FirstOrDefault(x => x.Id == loanId)
                   ?? throw new InvalidOperationException("Loan not found.");
        if (loan.IsReturned) throw new InvalidOperationException("Loan already returned.");

        var item = SearchItem(loan.ItemId)!;
        DateTime returned = (returnDate ?? DateTime.Today).Date;
        if (returned < loan.BorrowDate)
            throw new InvalidOperationException("Return date cannot be earlier than the borrow date.");
        int lateDays = Math.Max(0, (returned - loan.DueDate).Days);
        decimal fine = item.CalculateLateFee(lateDays);
        loan.CompleteReturn(returned, fine);
        item.SetAvailability(item.IsActive);
        var member = FindMember(loan.MemberId)!;
        member.OutstandingFine += fine;
        return fine;
    }

    public decimal PayFine(int memberId, decimal amount, string reference)
    {
        var member = FindMember(memberId) ?? throw new InvalidOperationException("Member not found.");
        if (amount <= 0 || amount > member.OutstandingFine)
            throw new InvalidOperationException("Payment must be positive and cannot exceed the outstanding balance.");
        if (_payments.Any(p => p.Reference.Equals(reference, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("This payment reference has already been used.");

        var payment = new Payment(_nextPaymentId, memberId, amount, reference);
        _payments.Add(payment);
        _nextPaymentId++;
        member.OutstandingFine -= amount;
        return member.OutstandingFine;
    }

    public IEnumerable<Loan> MemberHistory(int memberId) => _loans.Where(l => l.MemberId == memberId);
    public IEnumerable<Loan> ActiveLoans() => _loans.Where(l => !l.IsReturned);
    public IEnumerable<Loan> OverdueLoans(DateTime? date = null) =>
        _loans.Where(l => !l.IsReturned && l.DueDate < (date ?? DateTime.Today).Date);

    public IEnumerable<Member> MembersWithOverdueLoans(DateTime? date = null)
    {
        var overdueMemberIds = OverdueLoans(date).Select(l => l.MemberId).Distinct().ToHashSet();
        return _members.Where(m => overdueMemberIds.Contains(m.Id));
    }

    public IEnumerable<Member> MembersWithUnpaidFines() => _members.Where(m => m.OutstandingFine > 0);
    public decimal TotalCollected => _payments.Sum(p => p.Amount);

    public IEnumerable<(LibraryItem Item, int Count)> MostBorrowed(int take = 5) =>
        _loans.GroupBy(l => l.ItemId)
              .Select(g => (Item: SearchItem(g.Key)!, Count: g.Count()))
              .OrderByDescending(x => x.Count).Take(take);

    public decimal TotalOutstanding => _members.Sum(m => m.OutstandingFine);
}
