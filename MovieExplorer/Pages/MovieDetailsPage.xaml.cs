using MovieExplorer.Models;

namespace MovieExplorer.Pages {
    public partial class MovieDetailsPage {
        private readonly Movie movie;

        public MovieDetailsPage(Movie movie) {
            InitializeComponent();
            this.movie = movie;

            //filling ui
            TitleLabel.Text = movie.Title;
            YearLabel.Text = $"Year: {movie.Year}";
            GenreLabel.Text = string.Join(", ", movie.Genre);
            EmojiLabel.Text = movie.Emoji;
            RatingLabel.Text = $"IMDB: {movie.Rating:0.0}";

            //write "viewed" in history when page is opened in UserProfilePage.xaml.cs
            RecordViewed();

            UpdateFavButton();
        }

        //method for "viewed" in history
        private async void RecordViewed() {
            await HistoryStore.AddAsync(new HistoryEntry
            {
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                Emoji = movie.Emoji,
                Action = "viewed",
                Timestamp = DateTime.Now
            });
        }

        //updates favourite button text
        private void UpdateFavButton() {
            //ñheck if the movie is already in favourites
            bool isFavourite = false;

            foreach (var fav in FavouriteStore.Favourites) {
                if (fav.Title == movie.Title && fav.Year == movie.Year) {
                    isFavourite = true;
                    break;
                }
            }

            //set button text
            if (isFavourite)
                FavButton.Text = "Remove from favourites";
            else
                FavButton.Text = "Add to favourites";
        }

        //toggle favourite and write its history
        private async void OnToggleFavourite(object sender, EventArgs e) {
            bool added = await FavouriteStore.ToggleAsync(movie);

            var entry = new HistoryEntry();
            entry.Title = movie.Title;
            entry.Year = movie.Year;
            entry.Genre = movie.Genre;
            entry.Emoji = movie.Emoji;

            if (added == true) {
                entry.Action = "favourited";
            }
            else {
                entry.Action = "unfavourited"; 
            }

            entry.Timestamp = DateTime.Now;

            await HistoryStore.AddAsync(entry);

            //update ui
            UpdateFavButton();
        }
    }
}