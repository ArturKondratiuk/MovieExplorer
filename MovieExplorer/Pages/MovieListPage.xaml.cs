using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages;
public partial class MovieListPage {
    //full list of movies
    private List<Movie> allMovies = new();

    //displayed list bound to CollectionView
    private readonly ObservableCollection<DisplayMovie> shownMovies = new();

    //sorting: 0 = none, 1 = asc, -1 = desc
    int yearSort = 0;
    int ratingSort = 0;
    string lastSortPressed = "";

    //filters
    private readonly List<string> selectedGenres = new();
    private readonly List<string> selectedDirectors = new();

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

        CreateGenreCheckboxesFromData();
        CreateDirectorCheckboxesFromData();
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

        ApplyFiltersAndSort(text);
    }

    //sorting buttons
    private void OnSortByYear(object sender, EventArgs e) {
        if (yearSort == 1)
            yearSort = -1;
        else
            yearSort = 1;

        if (yearSort == 1)
            YearSortButton.Text = "Year ↑";
        else
            YearSortButton.Text = "Year ↓";

        lastSortPressed = "Year";

        UpdateSortButtonVisuals();
        ApplyFiltersAndSort(SearchBarControl.Text);
    }

    private void OnSortByRating(object sender, EventArgs e) {
        if (ratingSort == 1)
            ratingSort = -1;
        else
            ratingSort = 1;

        if (ratingSort == 1)
            RatingSortButton.Text = "Rating ↑";
        else
            RatingSortButton.Text = "Rating ↓";

        lastSortPressed = "Rating";

        UpdateSortButtonVisuals();
        ApplyFiltersAndSort(SearchBarControl.Text);
    }

    //button colors (looks good)
    private void UpdateSortButtonVisuals() {
        if (yearSort != 0)
            YearSortButton.BackgroundColor = Colors.Orange;
        else
            YearSortButton.BackgroundColor = Colors.Gray;

        if (ratingSort != 0)
            RatingSortButton.BackgroundColor = Colors.Orange;
        else
            RatingSortButton.BackgroundColor = Colors.Gray;
    }

    //genre panel
    private void OnToggleGenrePanel(object sender, EventArgs e) {
        GenrePanel.IsVisible = !GenrePanel.IsVisible;
    }

    private void CreateGenreCheckboxesFromData() {
        //clear panel
        GenreCheckboxContainer.Children.Clear();

        //genres without repeats
        List<string> genres = new List<string>();

        //collecting all genres
        foreach (var movie in allMovies) {
            if (movie.Genre == null)
                continue;

            foreach (var genre in movie.Genre) {
                if (!genres.Contains(genre))
                    genres.Add(genre);
            }
        }

        //sort list
        genres.Sort();

        //elements for each genre
        foreach (var g in genres) {
            var row = CreateCheckboxRow(g, OnGenreChecked);
            GenreCheckboxContainer.Children.Add(row);
        }
    }

    //genre checkboxes
    private void OnGenreChecked(object sender, CheckedChangedEventArgs e) {
        CheckBox cb = sender as CheckBox;
        if (cb == null)
            return;

        //genre with attached checkbox
        string genre = cb.BindingContext as string;
        if (genre == null)
            return;

        //if user tip add genre to filter
        if (e.Value == true) {
            if (!selectedGenres.Contains(genre))
                selectedGenres.Add(genre);
        }

        else {
            //else remove from filter
            selectedGenres.Remove(genre);
        }
        //update film list
        ApplyFiltersAndSort(SearchBarControl.Text);
    }

    //director panel
    private void OnToggleDirectorPanel(object sender, EventArgs e) {
        DirectorPanel.IsVisible = !DirectorPanel.IsVisible;
    }

    private void CreateDirectorCheckboxesFromData() {
        //clear panel before creating new elements
        DirectorCheckboxContainer.Children.Clear();

        //collect all directors
        List<string> directors = new List<string>();

        //sorting all films
        foreach (var movie in allMovies) {
            //skip if director empty
            if (string.IsNullOrWhiteSpace(movie.Director))
                continue;

            //add if no director
            if (!directors.Contains(movie.Director))
                directors.Add(movie.Director);
        }

        //sort directors by alphabet
        directors.Sort();

        //line with checknox for every director
        foreach (var director in directors) {
            var row = CreateCheckboxRow(director, OnDirectorChecked);
            DirectorCheckboxContainer.Children.Add(row);
        }
    }

    private void OnDirectorChecked(object sender, CheckedChangedEventArgs e) {
        //get checkbox
        CheckBox box = sender as CheckBox;

        //get name of director which stored in bindning context
        string director = box.BindingContext as string;

        //if tip add director to list
        if (e.Value) {
            if (!selectedDirectors.Contains(director))
                selectedDirectors.Add(director);
        }

        //if not delete from list
        else {
            selectedDirectors.Remove(director);
        }
        //update list
        ApplyFiltersAndSort(SearchBarControl.Text);
    }

    //checkboxes
    private View CreateCheckboxRow(string text, EventHandler<CheckedChangedEventArgs> onChecked) {
        //create checkbox
        CheckBox checkBox = new CheckBox();
        checkBox.BindingContext = text;      //connect the meaning (genre/director)
        checkBox.CheckedChanged += onChecked; //subscribe to the event

        //label
        Label label = new Label();
        label.Text = text;
        label.VerticalTextAlignment = TextAlignment.Center;

        //container
        HorizontalStackLayout row = new HorizontalStackLayout();
        row.Spacing = 6;
        row.Padding = new Thickness(6, 0);

        //chekbox with text
        row.Children.Add(checkBox);
        row.Children.Add(label);

        return row;
    }

    //apply filters and sort
    private void ApplyFiltersAndSort(string search) {
        //start with empy list
        List<Movie> result = new List<Movie>();

        //check every movie if it passes all filters
        foreach (var m in allMovies) {
            bool ok = true;
            //if search is not empty title must contain the search text
            if (!string.IsNullOrWhiteSpace(search)) {
                if (!m.Title.ToLower().Contains(search))
                    ok = false;
            }

            //genre filter
            if (selectedGenres.Count > 0) {
                //movie must have at least one genre matching selectedGenres
                bool foundGenre = false;

                if (m.Genre != null) {
                    foreach (var g in m.Genre) {
                        if (selectedGenres.Contains(g)) {
                            foundGenre = true;
                            break;
                        }
                    }
                }

                if (!foundGenre)
                    ok = false;
            }

            //director filter
            if (selectedDirectors.Count > 0) {
                if (!selectedDirectors.Contains(m.Director))
                    ok = false;
            }

            //only add movies that passed all checks
            if (ok)
                result.Add(m);
        }

        //sorting
        if (lastSortPressed == "Year" && yearSort != 0) {
            // ascending
            if (yearSort == 1)
                result = result.OrderBy(m => m.Year).ToList();
            // descending
            else
                result = result.OrderByDescending(m => m.Year).ToList();
        }

        else if (lastSortPressed == "Rating" && ratingSort != 0) {
            if (ratingSort == 1)
                result = result.OrderBy(m => m.Rating).ToList();
            else
                result = result.OrderByDescending(m => m.Rating).ToList();
        }
        //refresh ui
        UpdateShownMovies(result);
    }

    //update the CollectionView
    private void UpdateShownMovies(IEnumerable<Movie> movies) {
        //clear old items
        shownMovies.Clear();

        //add each movie one by one
        foreach (Movie movie in movies) {
            DisplayMovie item = ToDisplayMovie(movie);
            shownMovies.Add(item);
        }
    }

    //to movie details if clicked on title/emoji
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

//more time and effort was put into this page than in the entirety of last year.
//special thanks to my mom and dad for raising me to be a great person.
//at the end of the project I will tell them that they can be proud of me.