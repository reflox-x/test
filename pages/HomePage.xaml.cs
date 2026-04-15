using pawledger.Services;
using pawledger.models;

namespace pawledger.pages;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService? _databaseService;
    private DateTime _selectedDate = DateTime.Now;
    private readonly HomeChartDrawable _chartDrawable = new();

    public HomePage()
    {
        InitializeComponent();

        _databaseService = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<DatabaseService>();

        BalanceChartView.Drawable = _chartDrawable;

        string savedDate = Preferences.Default.Get("home_selected_date", "");
        if (!string.IsNullOrWhiteSpace(savedDate) && DateTime.TryParse(savedDate, out DateTime parsedDate))
        {
            _selectedDate = parsedDate;
        }

        HomeDatePicker.Date = _selectedDate;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLanguage();
        ApplyMonthTitle();
        ApplyThemeImage();
        await RefreshDataAsync();
    }

    private void ApplyLanguage()
    {
        AppTitleLabel.Text = LanguageService.GetText("AppTitle");
        IncomeTitleLabel.Text = LanguageService.GetText("Income");
        ExpenseTitleLabel.Text = LanguageService.GetText("Expense");
        BalanceTitleSpan.Text = $"{LanguageService.GetText("Balance")}  ";
        SummaryTitleLabel.Text = LanguageService.GetText("Summary");
        RecentRecordsTitleLabel.Text = LanguageService.GetText("RecentRecords");
        TotalTitleLabel.Text = LanguageService.GetText("Total");
        FoodTitleLabel.Text = LanguageService.GetText("Food");
        PetTitleLabel.Text = LanguageService.GetText("Pet");
    }

    private void ApplyThemeImage()
    {
        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        ThemeBannerImage.Source = isDark ? "fox_night.png" : "fox_day.png";
    }

    private void ApplyMonthTitle()
    {
        bool isChinese = Preferences.Default.Get("language", "English") == "Chinese";

        if (isChinese)
            MonthTitleButton.Text = $"{_selectedDate.Year}年{_selectedDate.Month}月";
        else
            MonthTitleButton.Text = _selectedDate.ToString("MMMM yyyy", new System.Globalization.CultureInfo("en-US"));
    }

    private async Task RefreshDataAsync()
    {
        string currency = Preferences.Default.Get("currency", "¥");

        if (_databaseService == null)
            return;

        List<RecordDb> allRecords = await _databaseService.GetRecordsAsync();

        var records = allRecords
            .Where(r => r.CreatedAt.Year == _selectedDate.Year &&
                        r.CreatedAt.Month == _selectedDate.Month)
            .ToList();

        decimal incomeTotal = records
            .Where(r => r.Type == "Income")
            .Sum(r => r.Amount);

        decimal expenseTotal = records
            .Where(r => r.Type == "Expense")
            .Sum(r => r.Amount);

        decimal balance = incomeTotal - expenseTotal;

        decimal foodTotal = records
            .Where(r => r.Category == "Food")
            .Sum(r => r.Amount);

        decimal petTotal = records
            .Where(r => r.Category == "Pet")
            .Sum(r => r.Amount);

        IncomeLabel.Text = $"{currency}{incomeTotal}";
        ExpenseLabel.Text = $"{currency}{expenseTotal}";
        BalanceLabel.Text = $"{currency}{balance}";

        TotalLabel.Text = $"{currency}{incomeTotal}";
        FoodLabel.Text = $"{currency}{foodTotal}";
        PetLabel.Text = $"{currency}{petTotal}";

        var recentItems = records
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new RecentRecordItem
            {
                Id = r.Id,
                Category = string.IsNullOrWhiteSpace(r.Category) ? "Other" : r.Category,
                Type = r.Type,
                AmountText = $"{currency}{r.Amount}",
                AmountColor = r.Type == "Income" ? "#2A7B74" : "#C96A59",
                DateText = r.CreatedAt.ToString("yyyy/MM/dd")
            })
            .ToList();

        RecentRecordsCollectionView.ItemsSource = recentItems;

        int daysInMonth = DateTime.DaysInMonth(_selectedDate.Year, _selectedDate.Month);

        var incomeSeries = Enumerable.Range(1, daysInMonth)
            .Select(day => (float)records
                .Where(r => r.CreatedAt.Day == day && r.Type == "Income")
                .Sum(r => r.Amount))
            .ToList();

        var expenseSeries = Enumerable.Range(1, daysInMonth)
            .Select(day => (float)records
                .Where(r => r.CreatedAt.Day == day && r.Type == "Expense")
                .Sum(r => r.Amount))
            .ToList();

        _chartDrawable.IncomePoints = incomeSeries;
        _chartDrawable.ExpensePoints = expenseSeries;
        _chartDrawable.DaysInMonth = daysInMonth;
        BalanceChartView.Invalidate();
    }

    private async void OnAddRecordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddRecordPage));
    }

    private void OnChangeDateClicked(object sender, EventArgs e)
    {
        HomeDatePicker.Focus();
    }

    private async void OnHomeDateSelected(object sender, DateChangedEventArgs e)
    {
        _selectedDate = e.NewDate ?? DateTime.Now;
        Preferences.Default.Set("home_selected_date", _selectedDate.ToString("yyyy-MM-dd"));

        ApplyMonthTitle();
        await RefreshDataAsync();
    }

    private async void OnDeleteRecordClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
            return;

        if (sender is not Button button || button.BindingContext is not RecentRecordItem selectedItem)
            return;

        bool confirm = await DisplayAlertAsync(
            "Delete Record",
            $"Are you sure you want to delete this {selectedItem.Category} record?",
            "Yes",
            "No");

        if (!confirm)
            return;

        var records = await _databaseService.GetRecordsAsync();
        var recordToDelete = records.FirstOrDefault(r => r.Id == selectedItem.Id);

        if (recordToDelete != null)
        {
            await _databaseService.DeleteRecordAsync(recordToDelete);
            await RefreshDataAsync();

            await DisplayAlertAsync("Deleted", "Record deleted successfully.", "OK");
        }
    }
}

public class RecentRecordItem
{
    public int Id { get; set; }
    public string Category { get; set; } = "";
    public string Type { get; set; } = "";
    public string AmountText { get; set; } = "";
    public string AmountColor { get; set; } = "#243B53";
    public string DateText { get; set; } = "";
}