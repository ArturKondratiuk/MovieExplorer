namespace MovieExplorer.Models;

public class Movie {
    public string Title { get; set; }
    public int Year { get; set; }
    public List<string> Genre { get; set; } //genre list - could be more than one
    public string Director { get; set; }
    public double Rating { get; set; }
    public string Emoji { get; set; }
    public string GenreString { get; set; } //text version of genre
}