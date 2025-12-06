using MovieExplorer.Models;

namespace MovieExplorer.Pages {
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

            //load favourites for current user
            await FavouriteStore.LoadAsync();

            //bind favourites 
            RefreshFavs();
        }

        //refreshing
        void RefreshFavs() {
            if (FavouriteStore.Favourites == null || FavouriteStore.Favourites.Count == 0) {
                EmptyLabel.IsVisible = true;
                FavListView.ItemsSource = null;
            }

            else {
                EmptyLabel.IsVisible = false;
                FavListView.ItemsSource = FavouriteStore.Favourites;
            }
        }

        //remove favourite
        private async void OnRemoveFavourite(object sender, EventArgs e) {
            Movie movie = (Movie)((Button)sender).BindingContext;
            if (movie == null)
                return;

            //remove from store
            await FavouriteStore.RemoveAsync(movie);

            //refresh ui
            RefreshFavs();
        }

        //logout
        private async void OnLogout(object sender, EventArgs e) {
            FavouriteStore.ClearMemory();
            UserStore.Logout();

            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}