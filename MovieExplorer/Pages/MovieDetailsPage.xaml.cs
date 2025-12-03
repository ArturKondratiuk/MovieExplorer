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
        }
    }
}