using MovieExplorer.Models;

namespace MovieExplorer.Pages {
    public partial class MainPage {
        public MainPage() {
            InitializeComponent();

            //if user name is not empty, go to MoviesListPage
            if (UserStore.CurrentUserName != null && UserStore.CurrentUserName.Trim() != "") {
                //to movieListPage
                Shell.Current.GoToAsync("//MovieListPage");
            }
        }

        //login method
        private async void OnLogin(object sender, EventArgs e) {
            //get the name from the entry box
            string name = NameEntry.Text;

            //if name is empty, show an error and stop
            if (name == null || name == "") {
                await DisplayAlert("Error", "Please enter your name", "OK");
                return;
            }

            //log in the user (this sets CurrentUserName)
            UserStore.Login(name);

            //go to the MovieListPage
            await Shell.Current.GoToAsync("//MovieListPage");
        }
    }
}