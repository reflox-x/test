using pawledger.Services;

namespace pawledger.pages;

public partial class AddRecordPage : ContentPage
{
    private bool _isExpense = true;

    public AddRecordPage()
    {
        InitializeComponent();
        UpdateModeUI();
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

        LedgerService.AddRecord(amount, type, category);

        await DisplayAlertAsync("Saved", "Record saved successfully.", "OK");
        await Shell.Current.GoToAsync("..");
    }
}