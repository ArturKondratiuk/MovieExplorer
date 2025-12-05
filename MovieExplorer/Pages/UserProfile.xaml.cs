using MovieExplorer.Models;

namespace MovieExplorer.Pages;

public partial class UserProfile {
	public UserProfile() {
		InitializeComponent();
	}

    protected override async void OnAppearing() {

        //show current user name, if there is no name show "unauthorized user"
        if (string.IsNullOrWhiteSpace(UserStore.CurrentUserName))
            WelcomeLabel.Text = "Welcome, unauthorized user";
        else
            WelcomeLabel.Text = $"Welcome, {UserStore.CurrentUserName}";
    }
}