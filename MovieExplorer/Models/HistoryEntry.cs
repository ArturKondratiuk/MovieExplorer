namespace MovieExplorer.Models {
    public class HistoryEntry {
        public string Title { get; set; }
        public int Year { get; set; }
        public List<string> Genre { get; set; }
        public string Emoji { get; set; }
        public string Action { get; set; }  //viewed/favourited/unfavourited
        public DateTime Timestamp { get; set; }
    }
}
