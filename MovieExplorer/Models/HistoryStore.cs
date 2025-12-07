using System.Text.Json;

namespace MovieExplorer.Models {

    //class stores viewing history for the current user
    public static class HistoryStore {

        //in-memory list of history entries
        public static List<HistoryEntry> Entries { get; private set; } = new();

        //returns the file path for the current user's history file
        private static string GetCurrentFilePath() {
            //no user logged in
            if (string.IsNullOrWhiteSpace(UserStore.CurrentUserName))
                return null;

            //return user's history file path
            return UserStore.HistoryPathFor(UserStore.CurrentUserName);
        }

        //loads history from disk
        public static async Task LoadAsync() {
            Entries.Clear();

            string filePath = GetCurrentFilePath();

            //if no file path — nothing to load
            if (filePath == null || !File.Exists(filePath))
                return;

            string json = await File.ReadAllTextAsync(filePath);

            var list = JsonSerializer.Deserialize<List<HistoryEntry>>(json);

            //add entries if the file contains a valid list
            if (list != null) {
                Entries.AddRange(list);
            }
        }

        //saves history to disk
        public static async Task SaveAsync() {
            string filePath = GetCurrentFilePath();

            //no path — can't save
            if (filePath == null)
                return;

            string json = JsonSerializer.Serialize(Entries);

            await File.WriteAllTextAsync(filePath, json);
        }

        //clears entries in memory and in the file
        public static async Task ClearAsync() {
            Entries.Clear();
            await SaveAsync();
        }

        //adds one entry and saves the updated list
        public static async Task AddAsync(HistoryEntry entry) {
            if (entry == null)
                return;

            Entries.Add(entry);
            await SaveAsync();
        }

        //clears only in-memory history (used on logout)
        public static void ClearMemory() {
            Entries.Clear();
        }
    }
}