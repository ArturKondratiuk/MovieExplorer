using System.Text.Json;

namespace MovieExplorer.Models {
    //stores information about the current user and gives file paths for saving their data
    public static class UserStore {
        //path to store list of all usernames
        private static readonly string usersFile = Path.Combine(FileSystem.AppDataDirectory, "users.json");

        //username of the person currently logged in
        public static string CurrentUserName { get; private set; } = "";

        //simple list of all known users
        private static List<string> users = new List<string>();

        //load the list of users from file
        public static void LoadUsers() {
            try {
                //if the file exists, read it
                if (File.Exists(usersFile)) {
                    string json = File.ReadAllText(usersFile);

                    //converting JSON into a List<string>
                    var list = JsonSerializer.Deserialize<List<string>>(json);

                    //if JSON is valid, use it, if not, create an empty list
                    if (list != null)
                        users = list;
                    else
                        users = new List<string>();
                }

                else {
                    //no file -> empty list
                    users = new List<string>();
                }
            }

            catch {
                //if any error, use an empty list
                users = new List<string>();
            }
        }

        //save user list to file
        private static void SaveUsers() {
            string json = JsonSerializer.Serialize(users);
            File.WriteAllText(usersFile, json);
        }

        //log in a user / create a new one if don't exist
        public static void Login(string username) {
            //do nothing if the name is empty
            if (string.IsNullOrWhiteSpace(username))
                return;

            //clean spaces (insanely good method)
            username = username.Trim();

            //load existing users
            LoadUsers();

            //if this username does not exist yet, add it
            if (!users.Contains(username)) {
                users.Add(username);
                SaveUsers();
            }

            //set the current user name
            CurrentUserName = username;
        }

        //log out (just clear the name)
        public static void Logout() {
            CurrentUserName = "";
        }

        //build a safe file path for favourites
        public static string FavouritesPathFor(string user) {
            string safe = MakeSafe(user);
            return Path.Combine(FileSystem.AppDataDirectory, safe + "_favourites.json");
        }

        //build a safe file path for history (i will use it later)
        public static string HistoryPathFor(string user) {
            string safe = MakeSafe(user);
            return Path.Combine(FileSystem.AppDataDirectory, safe + "_history.json");
        }

        //replace characters that are not allowed in filenames (that's a good idea isn't it)
        private static string MakeSafe(string s) {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '_');
            }
            return s;
        }
    }
}