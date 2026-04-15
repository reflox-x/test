using pawledger.models;
using SQLite;

namespace pawledger.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database != null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "pawledger.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<RecordDb>();
        await _database.CreateTableAsync<PlanDb>();
    }

    // Record

    public async Task<List<RecordDb>> GetRecordsAsync()
    {
        await Init();
        return await _database!.Table<RecordDb>()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> AddRecordAsync(RecordDb record)
    {
        await Init();
        return await _database!.InsertAsync(record);
    }

    public async Task<int> DeleteRecordAsync(RecordDb record)
    {
        await Init();
        return await _database!.DeleteAsync(record);
    }

    // Plan

    public async Task<List<PlanDb>> GetPlansAsync()
    {
        await Init();
        return await _database!.Table<PlanDb>().ToListAsync();
    }

    public async Task<int> AddPlanAsync(PlanDb plan)
    {
        await Init();
        return await _database!.InsertAsync(plan);
    }

    public async Task<int> UpdatePlanAsync(PlanDb plan)
    {
        await Init();
        return await _database!.UpdateAsync(plan);
    }

    public async Task<int> DeletePlanAsync(PlanDb plan)
    {
        await Init();
        return await _database!.DeleteAsync(plan);
    }
}