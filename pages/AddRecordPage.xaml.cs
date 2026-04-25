using pawledger.Services;
using pawledger.models;
using pawledger.Services;

namespace pawledger.pages;

public partial class AddRecordPage : ContentPage
{
    private bool _isExpense = true;
    private readonly DatabaseService? _databaseService;

    private void ApplyLanguage()
    {
        PageTitleLabel.Text = LanguageService.GetText("AddRecord");
        ExpenseButton.Text = LanguageService.GetText("Expense");
        IncomeButton.Text = LanguageService.GetText("Income");
        CategoryEntry.Placeholder = LanguageService.GetText("CategoryPlaceholder");
        SelectDateLabel.Text = LanguageService.GetText("SelectDate");
        SaveRecordButton.Text = LanguageService.GetText("SaveRecord");
    }

    public AddRecordPage()
    {
        InitializeComponent();
        UpdateModeUI();

        _databaseService = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<DatabaseService>();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnExpenseClicked(object sender, EventArgs e)
    {
        _isExpense = true;
        UpdateModeUI();
    }

    private void OnIncomeClicked(object sender, EventArgs e)
    {
        _isExpense = false;
        UpdateModeUI();
    }

    private void UpdateModeUI()
    {
        if (_isExpense)
        {
            ExpenseButton.BackgroundColor = Color.FromArgb("#F7CACA");
            ExpenseButton.TextColor = Color.FromArgb("#C96969");

            IncomeButton.BackgroundColor = Color.FromArgb("#EAF1EF");
            IncomeButton.TextColor = Color.FromArgb("#8AA4AF");
        }
        else
        {
            IncomeButton.BackgroundColor = Color.FromArgb("#DDEFE8");
            IncomeButton.TextColor = Color.FromArgb("#4D8CAD");

            ExpenseButton.BackgroundColor = Color.FromArgb("#F1EAEA");
            ExpenseButton.TextColor = Color.FromArgb("#B7A1A1");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!decimal.TryParse(AmountEntry.Text, out decimal amount))
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        string category = string.IsNullOrWhiteSpace(CategoryEntry.Text)
            ? "Other"
            : CategoryEntry.Text.Trim();

        string type = _isExpense ? "Expense" : "Income";

        // Maintain memory logic
        LedgerService.AddRecord(amount, type, category);

        // Write to SQLite
        if (_databaseService != null)
        {
            await _databaseService.AddRecordAsync(new RecordDb
            {
                Amount = amount,
                Type = type,
                Category = category,
                CreatedAt = RecordDatePicker.Date ?? DateTime.Now
            });
        }

        HapticService.Vibrate(80);

        await DisplayAlertAsync("Saved", "Record saved successfully.", "OK");
        await Shell.Current.GoToAsync("..");
    }
}