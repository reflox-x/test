using pawledger.Services;
using System.Linq;

namespace pawledger.pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshData();
    }

    private void RefreshData()
    {
        string currency = Preferences.Default.Get("currency", "¥");

        IncomeLabel.Text = $"{currency}{LedgerService.IncomeTotal}";
        ExpenseLabel.Text = $"{currency}{LedgerService.ExpenseTotal}";
        BalanceLabel.Text = $"{currency}{LedgerService.Balance}";

        TotalLabel.Text = $"{currency}{LedgerService.IncomeTotal}";

        var food = LedgerService.Records
            .Where(r => r.Category == "Food")
            .Sum(r => r.Amount);

        var pet = LedgerService.Records
            .Where(r => r.Category == "Pet")
            .Sum(r => r.Amount);

        FoodLabel.Text = $"{currency}{food}";
        PetLabel.Text = $"{currency}{pet}";
    }

    private async void OnAddRecordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddRecordPage));
    }
}