using pawledger.Services;
using pawledger.models;
using pawledger.Services;

namespace pawledger.pages;

public partial class PlanPage : ContentPage
{
    private readonly DatabaseService? _databaseService;

    private void ApplyLanguage()
    {
        PlanTitleLabel.Text = LanguageService.GetText("Plan");
        MyPlansTitleLabel.Text = LanguageService.GetText("MyPlans");
        AddToPlanButton.Text = $"+ {LanguageService.GetText("AddToPlan")}";
    }

    public PlanPage()
    {
        InitializeComponent();

        _databaseService = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<DatabaseService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLanguage();
        await RefreshPlansAsync();
    }

    private async Task RefreshPlansAsync()
    {
        if (_databaseService == null)
            return;

        List<PlanDb> plans = await _databaseService.GetPlansAsync();

        decimal totalSaved = plans.Sum(p => p.SavedAmount);
        decimal totalTarget = plans.Sum(p => p.TargetAmount);

        double overallProgress = 0;
        if (totalTarget > 0)
            overallProgress = (double)(totalSaved / totalTarget);

        SavedLabel.Text = $"Saved ¥{totalSaved} / ¥{totalTarget}";
        ProgressLabel.Text = $"{overallProgress:P0}";
        PlanProgressBar.Progress = overallProgress;

        PlanCollectionView.ItemsSource = null;
        PlanCollectionView.ItemsSource = plans;
    }

    private async void OnAddPlanClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
            return;

        string? title = await DisplayPromptAsync("New Plan", "Enter plan title:");
        if (string.IsNullOrWhiteSpace(title))
            return;

        string? amountText = await DisplayPromptAsync("New Plan", "Enter target amount:");
        if (!decimal.TryParse(amountText, out decimal targetAmount) || targetAmount <= 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        string? deadline = await DisplayPromptAsync("New Plan", "Enter deadline (e.g. 2026/12/01):");
        if (string.IsNullOrWhiteSpace(deadline))
            return;

        await _databaseService.AddPlanAsync(new PlanDb
        {
            Title = title.Trim(),
            TargetAmount = targetAmount,
            SavedAmount = 0,
            Deadline = deadline.Trim()
        });

        await RefreshPlansAsync();
        await DisplayAlertAsync("Saved", "Plan added successfully.", "OK");
    }

    private async void OnAddSavingsClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
            return;

        if (sender is not Button button || button.BindingContext is not PlanDb selectedPlan)
            return;

        string? amountText = await DisplayPromptAsync(
            "Add Savings",
            $"Enter amount for {selectedPlan.Title}:");

        if (!decimal.TryParse(amountText, out decimal amount) || amount <= 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        selectedPlan.SavedAmount += amount;

        if (selectedPlan.SavedAmount > selectedPlan.TargetAmount)
            selectedPlan.SavedAmount = selectedPlan.TargetAmount;

        await _databaseService.UpdatePlanAsync(selectedPlan);
        await RefreshPlansAsync();

        await DisplayAlertAsync("Saved", "Savings updated successfully.", "OK");
    }

    private async void OnEditPlanClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
            return;

        if (sender is not Button button || button.BindingContext is not PlanDb selectedPlan)
            return;

        string? newTitle = await DisplayPromptAsync(
            "Edit Plan",
            "Enter new title:",
            initialValue: selectedPlan.Title);

        if (string.IsNullOrWhiteSpace(newTitle))
            return;

        string? amountText = await DisplayPromptAsync(
            "Edit Plan",
            "Enter new target amount:",
            initialValue: selectedPlan.TargetAmount.ToString());

        if (!decimal.TryParse(amountText, out decimal newTargetAmount) || newTargetAmount <= 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid target amount.", "OK");
            return;
        }

        string? newDeadline = await DisplayPromptAsync(
            "Edit Plan",
            "Enter new deadline:",
            initialValue: selectedPlan.Deadline);

        if (string.IsNullOrWhiteSpace(newDeadline))
            return;

        selectedPlan.Title = newTitle.Trim();
        selectedPlan.TargetAmount = newTargetAmount;
        selectedPlan.Deadline = newDeadline.Trim();

        if (selectedPlan.SavedAmount > selectedPlan.TargetAmount)
            selectedPlan.SavedAmount = selectedPlan.TargetAmount;

        await _databaseService.UpdatePlanAsync(selectedPlan);
        await RefreshPlansAsync();

        await DisplayAlertAsync("Updated", "Plan updated successfully.", "OK");
    }

    private async void OnDeletePlanClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
            return;

        if (sender is not Button button || button.BindingContext is not PlanDb selectedPlan)
            return;

        bool confirm = await DisplayAlertAsync(
            "Delete Plan",
            $"Are you sure you want to delete {selectedPlan.Title}?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _databaseService.DeletePlanAsync(selectedPlan);
        await RefreshPlansAsync();

        await DisplayAlertAsync("Deleted", "Plan deleted successfully.", "OK");
    }
}