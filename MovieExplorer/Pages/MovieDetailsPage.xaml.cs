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

            UpdateFavButton();
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

        //toggle favourite
        private async void OnToggleFavourite(object sender, EventArgs e) {
            bool added = await FavouriteStore.ToggleAsync(movie);

            //update ui
            UpdateFavButton();
        }
    }
}