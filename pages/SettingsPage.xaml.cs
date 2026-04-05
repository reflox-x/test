namespace pawledger.pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        string savedLanguage = Preferences.Default.Get("language", "English");
        string savedCurrency = Preferences.Default.Get("currency", "¥");
        bool savedDarkMode = Preferences.Default.Get("darkmode", false);

        LanguagePicker.SelectedItem = savedLanguage;

        if (savedCurrency == "¥")
            CurrencyPicker.SelectedItem = "¥ Yen";
        else if (savedCurrency == "$")
            CurrencyPicker.SelectedItem = "$ USD";
        else
            CurrencyPicker.SelectedItem = "€ Euro";

        DarkModeSwitch.IsToggled = savedDarkMode;
        ApplyTheme(savedDarkMode);
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        if (LanguagePicker.SelectedItem == null)
            return;

        string selectedLanguage = LanguagePicker.SelectedItem.ToString() ?? "English";
        Preferences.Default.Set("language", selectedLanguage);

        await DisplayAlertAsync("Saved", $"Language set to {selectedLanguage}.", "OK");
    }

    private async void OnCurrencyChanged(object sender, EventArgs e)
    {
        if (CurrencyPicker.SelectedItem == null)
            return;

        string selected = CurrencyPicker.SelectedItem.ToString() ?? "¥ Yen";
        string symbol = "¥";

        if (selected.StartsWith("$"))
            symbol = "$";
        else if (selected.StartsWith("€"))
            symbol = "€";

        Preferences.Default.Set("currency", symbol);

        await DisplayAlertAsync("Saved", $"Currency set to {symbol}.", "OK");
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        bool isDark = e.Value;
        Preferences.Default.Set("darkmode", isDark);
        ApplyTheme(isDark);
    }

    private void ApplyTheme(bool isDark)
    {
        Application.Current!.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }
}