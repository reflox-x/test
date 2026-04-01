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
        IncomeLabel.Text = $"¥{LedgerService.IncomeTotal}";
        ExpenseLabel.Text = $"¥{LedgerService.ExpenseTotal}";
        BalanceLabel.Text = $"¥{LedgerService.Balance}";

        TotalLabel.Text = $"¥{LedgerService.IncomeTotal}";

        var food = LedgerService.Records
            .Where(r => r.Category == "Food")
            .Sum(r => r.Amount);

        var pet = LedgerService.Records
            .Where(r => r.Category == "Pet")
            .Sum(r => r.Amount);

        FoodLabel.Text = $"¥{food}";
        PetLabel.Text = $"¥{pet}";
    }

    private async void OnAddRecordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddRecordPage));
    }
}