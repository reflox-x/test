using pawledger.Services;

namespace pawledger.pages;

public partial class PlanPage : ContentPage
{
    public PlanPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshPlans();
    }

    private void RefreshPlans()
    {
        SavedLabel.Text = $"Saved ¥{PlanService.TotalSaved} / ¥{PlanService.TotalTarget}";
        ProgressLabel.Text = $"{PlanService.OverallProgress:P0}";
        PlanProgressBar.Progress = PlanService.OverallProgress;

        PlanCollectionView.ItemsSource = null;
        PlanCollectionView.ItemsSource = PlanService.Plans;
    }

    private async void OnAddPlanClicked(object sender, EventArgs e)
    {
        string? title = await DisplayPromptAsync("New Plan", "Enter plan title:");
        if (string.IsNullOrWhiteSpace(title))
            return;

        string? amountText = await DisplayPromptAsync("New Plan", "Enter target amount:");
        if (!decimal.TryParse(amountText, out decimal targetAmount))
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        string? deadline = await DisplayPromptAsync("New Plan", "Enter deadline (e.g. 2026/12/01):");
        if (string.IsNullOrWhiteSpace(deadline))
            return;

        PlanService.AddPlan(title.Trim(), targetAmount, deadline.Trim());
        RefreshPlans();

        await DisplayAlertAsync("Saved", "Plan added successfully.", "OK");
    }

    private async void OnAddSavingsClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not PlanItem selectedPlan)
            return;

        string? amountText = await DisplayPromptAsync(
            "Add Savings",
            $"Enter amount for {selectedPlan.Title}:");

        if (!decimal.TryParse(amountText, out decimal amount) || amount <= 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        PlanService.AddSavings(selectedPlan, amount);
        RefreshPlans();

        await DisplayAlertAsync("Saved", "Savings updated successfully.", "OK");
    }

    private async void OnDeletePlanClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not PlanItem selectedPlan)
            return;

        bool confirm = await DisplayAlertAsync(
            "Delete Plan",
            $"Are you sure you want to delete {selectedPlan.Title}?",
            "Yes",
            "No");

        if (!confirm)
            return;

        PlanService.DeletePlan(selectedPlan);
        RefreshPlans();

        await DisplayAlertAsync("Deleted", "Plan deleted successfully.", "OK");
    }
}