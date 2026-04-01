namespace pawledger.Services;

public static class LedgerService
{
    public static decimal IncomeTotal { get; set; } = 5000;
    public static decimal ExpenseTotal { get; set; } = 2000;

    public static List<RecordItem> Records { get; set; } = new()
    {
        new RecordItem { Category = "Food", Amount = 100, Type = "Expense" },
        new RecordItem { Category = "Pet", Amount = 100, Type = "Expense" }
    };

    public static decimal Balance => IncomeTotal - ExpenseTotal;

    public static void AddRecord(decimal amount, string type, string category)
    {
        Records.Add(new RecordItem
        {
            Amount = amount,
            Type = type,
            Category = category
        });

        if (type == "Income")
            IncomeTotal += amount;
        else
            ExpenseTotal += amount;
    }
}

public class RecordItem
{
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public string Type { get; set; } = "";
}