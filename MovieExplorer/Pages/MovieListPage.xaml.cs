using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages;
public partial class MovieListPage {
    //full list of movies
    private List<Movie> allMovies = new();

    //displayed list bound to CollectionView
    private readonly ObservableCollection<DisplayMovie> shownMovies = new();

    public MovieListPage() {
        InitializeComponent();
        LoadMovies();
    }

    //movie details
    class DisplayMovie {
        public Movie Original { get; set; }
        public string Title => Original.Title;
        public int Year => Original.Year;
        public string YearText => $"Year: {Year}";
        public string GenreString { get; set; }
        public double Rating => Original.Rating;
        public string RatingDisplay => $"IMDB: {Rating:0.0}";
        public string Emoji => Original.Emoji;

    }

    //load movies
    private async void LoadMovies() {
        var service = new MovieService();

        var loaded = await service.LoadMoviesAsync();

        if (loaded == null)
            allMovies = new List<Movie>();
        else
            allMovies = loaded;

        //build displayed list
        foreach (var movie in allMovies)
            shownMovies.Add(ToDisplayMovie(movie));

        MovieListView.ItemsSource = shownMovies;

    }

    //display movies
    private DisplayMovie ToDisplayMovie(Movie m) {
        var dm = new DisplayMovie();
        dm.Original = m;

        if (m.Genre != null)
            dm.GenreString = string.Join(", ", m.Genre);
        else
            dm.GenreString = "";

        return dm;
    }

    //searching
    private void OnSearchChanged(object sender, TextChangedEventArgs e) {
        string text = e.NewTextValue;

        if (text == null)
            text = "";

        text = text.Trim().ToLower();
    }

    private async void OnMovieTap(object sender, EventArgs e) {
        //get the label that was tapped
        Label tappedLabel = sender as Label;
        if (tappedLabel == null)
            return;

        //get the DisplayMovie attached to this label
        DisplayMovie item = tappedLabel.BindingContext as DisplayMovie;
        if (item == null || item.Original == null)
            return;

        //open movie details page
        await Navigation.PushAsync(new MovieDetailsPage(item.Original));
    }
}