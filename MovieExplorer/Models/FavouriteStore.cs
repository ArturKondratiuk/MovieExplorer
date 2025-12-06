using System.Text.Json;
using System.Collections.ObjectModel;

namespace MovieExplorer.Models {
    //stores favs for the current user in memory and saves/loads from disk
    public static class FavouriteStore {
        //ui updates automatically when items change
        public static ObservableCollection<Movie> Favourites { get; } = new();

        //loads favs from file
        public static async Task LoadAsync() {
            Favourites.Clear();

            var user = UserStore.CurrentUserName;
            if (string.IsNullOrWhiteSpace(user))
                return;

            var path = UserStore.FavouritesPathFor(user);
            if (!File.Exists(path))
                return;

            var json = await File.ReadAllTextAsync(path);
            var list = JsonSerializer.Deserialize<List<Movie>>(json);

            if (list != null)
                foreach (var m in list)
                    Favourites.Add(m);
        }

        //save current favs to file
        public static async Task SaveAsync() {
            var user = UserStore.CurrentUserName;
            if (string.IsNullOrWhiteSpace(user))
                return;

            var path = UserStore.FavouritesPathFor(user);
            var json = JsonSerializer.Serialize(Favourites.ToList());
            await File.WriteAllTextAsync(path, json);
        }

        //adds or removes a favourite (true = added / false = removed)
        public static async Task<bool> ToggleAsync(Movie m) {
            if (m == null)
                return false;

            //find by unique key (title + year)
            var existing = Favourites.FirstOrDefault(x => x.Title == m.Title && x.Year == m.Year);

            if (existing != null) {
                //remove favourite
                Favourites.Remove(existing);
                await SaveAsync();
                return false;
            }

            //copy to prevents modifying original movie object
            Favourites.Add(new Movie {
                Title = m.Title,
                Year = m.Year,
                Genre = new List<string>(m.Genre),
                Director = m.Director,
                Rating = m.Rating,
                Emoji = m.Emoji
            });

            await SaveAsync();
            return true;
        }

        //removes a movie without toggling logic
        public static async Task RemoveAsync(Movie movie) {
            //If nothing was passed, stop
            if (movie == null)
                return;

            //find the movie in the favourites list
            Movie movieToRemove = null;

            foreach (var item in Favourites) {
                if (item.Title == movie.Title && item.Year == movie.Year) {
                    movieToRemove = item;
                    break;
                }
            }

            //if the movie exists in the list — remove it
            if (movieToRemove != null) {
                Favourites.Remove(movieToRemove);

                //save the updated list
                await SaveAsync();
            }
        }

        //clears in-memory favs on logout (not files)
        public static void ClearMemory() {
            Favourites.Clear();
        }
    }
}