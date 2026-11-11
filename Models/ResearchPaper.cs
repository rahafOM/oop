namespace LibraryManagementSystem.Models;

public class ResearchPaper : LibraryItem
{
    public string Researcher { get; private set; }
    public string ResearchField { get; private set; }

    public ResearchPaper(int id, string title, string researcher, string researchField) : base(id, title)
    {
        if (string.IsNullOrWhiteSpace(researcher) || string.IsNullOrWhiteSpace(researchField))
            throw new ArgumentException("Researcher and research field are required.");
        Researcher = researcher.Trim();
        ResearchField = researchField.Trim();
    }

    public override int GetLoanPeriod() => 21;
    public override decimal CalculateLateFee(int lateDays) => Math.Max(0, lateDays) * 3m;
    public void UpdateDetails(string researcher, string field)
    {
        if (string.IsNullOrWhiteSpace(researcher) || string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Researcher and field are required.");
        Researcher = researcher.Trim(); ResearchField = field.Trim();
    }
    public override string GetDetails() =>
        base.GetDetails() + $" | Researcher: {Researcher} | Field: {ResearchField}";
}
