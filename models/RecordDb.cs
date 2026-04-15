using SQLite;

namespace pawledger.models;

public class RecordDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = "";

    public string Category { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}