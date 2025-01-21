using System;
using System.Collections.Generic;
using System.Linq;

namespace CashflowLoan;

public class AccountingBasedUnderwriting
{
    private const decimal MIN_OPERATING_CASH_FLOW_COVERAGE = 1.2m;
    private const decimal MAX_DEBT_SERVICE_RATIO = 0.2m;
    private const int MIN_MONTHS_HISTORY = 3;
    private const decimal MIN_MONTHLY_REVENUE = 1000m;
    private const decimal MIN_CURRENT_RATIO = 1.1m;
    private const decimal TARGET_WORKING_CAPITAL_DAYS = 45m;

    public record AccountingMetrics
    {
        // Monthly snapshots
        public List<FinancialSnapshot> MonthlySnapshots { get; init; }

        // Current positions
        public decimal CurrentAssets { get; init; }
        public decimal CurrentLiabilities { get; init; }
        public decimal CashBalance { get; init; }
        public decimal ExistingDebtPayments { get; init; }

        // AR/AP details
        public List<AgingBucket> AccountsReceivable { get; init; }
        public List<AgingBucket> AccountsPayable { get; init; }

        // Revenue concentration
        public decimal TopCustomerRevenuePercentage { get; init; }
        public decimal Top5CustomerRevenuePercentage { get; init; }
    }

    public record FinancialSnapshot
    {
        public decimal Revenue { get; init; }
        public decimal GrossProfit { get; init; }
        public decimal OperatingExpenses { get; init; }
        public decimal OperatingCashFlow { get; init; }
        public decimal WorkingCapital { get; init; }
        public DateOnly Month { get; init; }
    }

    public record AgingBucket
    {
        public int DaysRange { get; init; }  // e.g., 0-30, 31-60
        public decimal Amount { get; init; }
    }

    public LoanOffer Calculate(AccountingMetrics metrics)
    {
        var rejectionReasons = new List<string>();

        // Basic eligibility checks
        if (metrics.MonthlySnapshots.Count < MIN_MONTHS_HISTORY)
        {
            rejectionReasons.Add($"Minimum {MIN_MONTHS_HISTORY} months history required");
        }

        var avgMonthlyRevenue = metrics.MonthlySnapshots.Average(x => x.Revenue);
        if (avgMonthlyRevenue < MIN_MONTHLY_REVENUE)
        {
            rejectionReasons.Add($"Minimum monthly revenue of ${MIN_MONTHLY_REVENUE:N0} required");
        }

        // Calculate key health metrics
        var currentRatio = metrics.CurrentAssets / metrics.CurrentLiabilities;
        if (currentRatio < MIN_CURRENT_RATIO)
        {
            rejectionReasons.Add("Current ratio below minimum threshold");
        }

        if (rejectionReasons.Any())
        {
            return new LoanOffer { IsApproved = false, RejectionReasons = rejectionReasons };
        }

        // Calculate maximum loan amount based on multiple approaches
        var loanAmounts = CalculatePotentialLoanAmounts(metrics);
        var maxLoan = loanAmounts.Min(); // Most conservative amount

        // Determine repayment schedule type
        var scheduleType = DetermineOptimalRepaymentSchedule(metrics);

        // Calculate risk-adjusted interest rate
        var baseRate = CalculateBaseInterestRate(metrics);

        // Calculate recommended percentage of revenue/cash flow for repayment
        var repaymentPercentage = CalculateRepaymentPercentage(metrics, maxLoan, scheduleType);

        return new LoanOffer
        {
            IsApproved = true,
            MaxLoanAmount = Math.Round(maxLoan / 100) * 100,
            RecommendedLoanAmount = Math.Round(maxLoan * 0.75m / 100) * 100,
            BaseInterestRate = baseRate,
            RepaymentPercentage = repaymentPercentage,
            RecommendedSchedule = scheduleType
        };
    }

    private decimal[] CalculatePotentialLoanAmounts(AccountingMetrics metrics)
    {
        // 1. Cash Flow Based Approach
        var avgMonthlyCashFlow = metrics.MonthlySnapshots.Average(x => x.OperatingCashFlow);
        var cashFlowBasedAmount = avgMonthlyCashFlow * 6; // 6 months of operating cash flow

        // 2. Working Capital Approach
        var currentWorkingCapital = metrics.CurrentAssets - metrics.CurrentLiabilities;
        var targetWorkingCapital = (metrics.MonthlySnapshots.Average(x => x.Revenue) / 30) * TARGET_WORKING_CAPITAL_DAYS;
        var workingCapitalGap = Math.Max(targetWorkingCapital - currentWorkingCapital, 0);

        // 3. Revenue Multiple Approach
        var annualRevenue = metrics.MonthlySnapshots.Average(x => x.Revenue) * 12;
        var revenueBasedAmount = annualRevenue * 0.2m; // 20% of annual revenue

        // 4. Debt Service Capacity
        var monthlyFreeCashFlow = avgMonthlyCashFlow - metrics.ExistingDebtPayments;
        var debtServiceBasedAmount = (monthlyFreeCashFlow * 12 * MAX_DEBT_SERVICE_RATIO) / 0.15m; // Assuming 15% annual cost

        return new[] { cashFlowBasedAmount, workingCapitalGap, revenueBasedAmount, debtServiceBasedAmount };
    }

    private RepaymentSchedule DetermineOptimalRepaymentSchedule(AccountingMetrics metrics)
    {
        // Check for seasonality
        var hasSeasonality = DetectSeasonality(metrics.MonthlySnapshots);
        if (hasSeasonality)
            return RepaymentSchedule.SeasonallyAdjusted;

        // Check cash flow stability
        var cashFlowVariability = CalculateCashFlowVariability(metrics.MonthlySnapshots);
        if (cashFlowVariability > 0.3m)
            return RepaymentSchedule.FixedPercentageOfRevenue;

        // Check working capital efficiency
        var workingCapitalTurnover = CalculateWorkingCapitalTurnover(metrics.MonthlySnapshots);
        if (workingCapitalTurnover > 6) // High efficiency
            return RepaymentSchedule.FixedPercentageOfCashFlow;

        return RepaymentSchedule.FixedMonthlyPayment;
    }

    private decimal CalculateBaseInterestRate(AccountingMetrics metrics)
    {
        var baseRate = 0.08m; // Start with 8% base rate

        // Adjust for customer concentration risk
        if (metrics.TopCustomerRevenuePercentage > 0.2m)
            baseRate += 0.02m;

        // Adjust for AR quality
        var badARPercentage = metrics.AccountsReceivable
            .Where(x => x.DaysRange > 90)
            .Sum(x => x.Amount) / metrics.AccountsReceivable.Sum(x => x.Amount);
        if (badARPercentage > 0.1m)
            baseRate += 0.015m;

        // Adjust for operating margins
        var avgOperatingMargin = metrics.MonthlySnapshots.Average(x =>
            (x.Revenue - x.OperatingExpenses) / x.Revenue);
        if (avgOperatingMargin > 0.15m)
            baseRate -= 0.01m;

        return Math.Max(baseRate, 0.05m); // Minimum 5% rate
    }

    private decimal CalculateRepaymentPercentage(AccountingMetrics metrics, decimal loanAmount, RepaymentSchedule schedule)
    {
        switch (schedule)
        {
            case RepaymentSchedule.FixedPercentageOfRevenue:
                return Math.Min(0.1m, loanAmount / (metrics.MonthlySnapshots.Average(x => x.Revenue) * 12));

            case RepaymentSchedule.FixedPercentageOfCashFlow:
                return Math.Min(0.2m, loanAmount / (metrics.MonthlySnapshots.Average(x => x.OperatingCashFlow) * 12));

            case RepaymentSchedule.SeasonallyAdjusted:
                // Base percentage that will be adjusted up/down based on seasonal patterns
                return 0.15m;

            default:
                return 0;
        }
    }

    private bool DetectSeasonality(List<FinancialSnapshot> snapshots)
    {
        // Simple seasonality detection - look for consistent patterns in revenue
        if (snapshots.Count < 12)
            return false;

        var monthlyAverages = snapshots
            .GroupBy(x => x.Month.Month)
            .Select(g => new { Month = g.Key, AvgRevenue = g.Average(x => x.Revenue) })
            .OrderBy(x => x.Month)
            .ToList();

        var overall_avg = monthlyAverages.Average(x => x.AvgRevenue);
        var significant_variations = monthlyAverages
            .Count(x => Math.Abs(x.AvgRevenue - overall_avg) / overall_avg > 0.2m);

        return significant_variations >= 4; // If 4+ months show >20% variation from mean
    }

    private decimal CalculateCashFlowVariability(List<FinancialSnapshot> snapshots)
    {
        var cashFlows = snapshots.Select(x => x.OperatingCashFlow).ToList();
        var avg = cashFlows.Average();
        var variance = cashFlows.Sum(x => Math.Pow((double)(x - avg), 2)) / cashFlows.Count;
        var stdDev = (decimal)Math.Sqrt((double)variance);
        return stdDev / avg; // Coefficient of variation
    }

    private decimal CalculateWorkingCapitalTurnover(List<FinancialSnapshot> snapshots)
    {
        var avgRevenue = snapshots.Average(x => x.Revenue);
        var avgWorkingCapital = snapshots.Average(x => x.WorkingCapital);
        return avgWorkingCapital != 0 ? (avgRevenue * 12) / avgWorkingCapital : 0;
    }
}