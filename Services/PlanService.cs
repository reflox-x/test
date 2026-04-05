namespace pawledger.Services;

public static class PlanService
{
    public static List<PlanItem> Plans { get; set; } = new()
    {
        new PlanItem
        {
            Title = "Travel Fund",
            TargetAmount = 10000,
            SavedAmount = 5000,
            Deadline = "2026/06/01"
        },
        new PlanItem
        {
            Title = "New Laptop",
            TargetAmount = 15000,
            SavedAmount = 3000,
            Deadline = "2026/07/01"
        },
        new PlanItem
        {
            Title = "Emergency Fund",
            TargetAmount = 20000,
            SavedAmount = 2000,
            Deadline = "2026/08/01"
        }
    };

    public static void AddPlan(string title, decimal targetAmount, string deadline)
    {
        Plans.Add(new PlanItem
        {
            Title = title,
            TargetAmount = targetAmount,
            SavedAmount = 0,
            Deadline = deadline
        });
    }

    public static void AddSavings(PlanItem plan, decimal amount)
    {
        plan.SavedAmount += amount;

        if (plan.SavedAmount > plan.TargetAmount)
        {
            plan.SavedAmount = plan.TargetAmount;
        }
    }

    public static void DeletePlan(PlanItem plan)
    {
        if (Plans.Contains(plan))
        {
            Plans.Remove(plan);
        }
    }

    public static decimal TotalSaved => Plans.Sum(p => p.SavedAmount);

    public static decimal TotalTarget => Plans.Sum(p => p.TargetAmount);

    public static double OverallProgress
    {
        get
        {
            if (TotalTarget == 0)
                return 0;

            return (double)(TotalSaved / TotalTarget);
        }
    }
}

public class PlanItem
{
    public string Title { get; set; } = "";
    public decimal TargetAmount { get; set; }
    public decimal SavedAmount { get; set; }
    public string Deadline { get; set; } = "";

    public double Progress
    {
        get
        {
            if (TargetAmount == 0)
                return 0;

            return (double)(SavedAmount / TargetAmount);
        }
    }
}