namespace pawledger.pages;

public partial class PlanPage : ContentPage
{
    public PlanPage()
    {
        InitializeComponent();
    }

    private async void OnAddPlanClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Plan", "Add plan feature will be implemented next.", "OK");
    }
}