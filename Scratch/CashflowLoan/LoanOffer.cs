namespace CashflowLoan;

public record LoanOffer
{
    public decimal MaxLoanAmount { get; init; }
    public decimal RecommendedLoanAmount { get; init; }
    public decimal BaseInterestRate { get; init; }
    public decimal RepaymentPercentage { get; init; }
    public bool IsApproved { get; init; }
    public List<string> RejectionReasons { get; init; }
    public RepaymentSchedule RecommendedSchedule { get; init; }
}