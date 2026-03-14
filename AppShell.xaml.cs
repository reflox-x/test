using pawledger.pages;

namespace pawledger;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddRecordPage), typeof(AddRecordPage));
    }
}