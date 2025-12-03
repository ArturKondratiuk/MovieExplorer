using System.Text.Json;
namespace MovieExplorer.Models {
    public class MovieService {
        private const string LocalFileName = "movies_cache.json";
        private string localPath = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);
        public async Task<List<Movie>> LoadMoviesAsync() {
            //if cash exist read it
            if (File.Exists(localPath)) {
                string json = await File.ReadAllTextAsync(localPath);
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                return JsonSerializer.Deserialize<List<Movie>>(json, options);
            }

            //first start - read json from resources
            using var stream = await FileSystem.OpenAppPackageFileAsync("Resources/Data/moviesemoji.json");
            using var reader = new StreamReader(stream); string jsonData = await reader.ReadToEndAsync();

            //save locally
            await File.WriteAllTextAsync(localPath, jsonData);
            var opt = new JsonSerializerOptions();
            opt.PropertyNameCaseInsensitive = true;
            return JsonSerializer.Deserialize<List<Movie>>(jsonData, opt);
        }
    }
}