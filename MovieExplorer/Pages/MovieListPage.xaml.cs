namespace MovieExplorer.Pages;
public partial class MovieListPage {

    public MovieListPage() {
        InitializeComponent();
    }

    //searching
    private void OnSearchChanged(object sender, TextChangedEventArgs e) {
        string text = e.NewTextValue;

        if (text == null)
            text = "";

        text = text.Trim().ToLower();
    }
}