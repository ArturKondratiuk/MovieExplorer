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

            //load data for current user
            await FavouriteStore.LoadAsync();
            await HistoryStore.LoadAsync();

            //bind favourites 
            RefreshFavs();
            RefreshHistory();
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

        void RefreshHistory() {
            if (HistoryStore.Entries == null || HistoryStore.Entries.Count == 0) {
                HistoryListView.ItemsSource = null;
                return;
            }
            var sortedHistory = SortHistoryByNewest(HistoryStore.Entries);
            HistoryListView.ItemsSource = sortedHistory;
        }

        List<HistoryEntry> SortHistoryByNewest(List<HistoryEntry> list) {
            return list
                .OrderByDescending(x => x.Timestamp)
                .ToList();
        }

        //remove favourite
        private async void OnRemoveFavourite(object sender, EventArgs e) {
            Movie movie = (Movie)((Button)sender).BindingContext;
            if (movie == null)
                return;

            //remove from store
            await FavouriteStore.RemoveAsync(movie);

            //add history entry for unfavourited
            await HistoryStore.AddAsync(new HistoryEntry {
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                Emoji = movie.Emoji,
                Action = "unfavourited",
                Timestamp = DateTime.Now
            });

            //refresh ui
            RefreshFavs();
            RefreshHistory();
        }

        //logout
        private async void OnLogout(object sender, EventArgs e) {
            FavouriteStore.ClearMemory();
            HistoryStore.ClearMemory();
            UserStore.Logout();

            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}