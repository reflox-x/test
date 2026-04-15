using SQLite;

namespace pawledger.models;

public class PlanDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public decimal TargetAmount { get; set; }

    public decimal SavedAmount { get; set; }

    public string Deadline { get; set; } = "";

    [Ignore]
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