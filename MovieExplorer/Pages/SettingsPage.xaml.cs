namespace MovieExplorer.Pages;

public partial class SettingsPage {
    //theme store
    public static class SettingsStore {
        public static bool DarkThemeEnabled { get; set; }
    }

    public SettingsPage() {
        InitializeComponent();

        //sync ui with saved settings
        ThemeSwitch.IsToggled = SettingsStore.DarkThemeEnabled;
    }

    //light/dark theme
    private void OnThemeToggled(object sender, ToggledEventArgs e) {
        bool isDark = e.Value;

        //apply theme
        if (isDark) {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }

        else {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}