using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NotifiqueMe
{
    public class LoginPage : ContentPage
    {
        // Class operation variables
        private bool isLoggedIn;
        private LanguageModule language = LanguageModule.Instance;
        private NavPage instanceToReturn;
        private ConnectivityModule serverConnection = ConnectivityModule.Instance;

        // Public Variables
        public string email;

        // Login Page Structure
        ViewCell titleCell;
        Entry username;
        ViewCell usernameCell;
        Entry password;
        ViewCell passwordCell;
        Button login;
        Button signup;

        // Override Back button on device
        override protected bool OnBackButtonPressed()
        {
            return false;
        }

        // Constructor
        public LoginPage(NavPage returnTo)
        {
            // Sets the page to return to when login completes.
            instanceToReturn = returnTo;

            // Initialize all views to be used.
            titleCell = new ViewCell
            {
                Height = 100.0,
                View = new StackLayout
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(0.0, 10.0),
                    Children = {
                        new Label {
                            Text = language.notifiqueMeText,
                            FontSize = 25,
                            HorizontalOptions = LayoutOptions.Start
                        },
                        new Label {
                            Text = "Alpha",
                            VerticalOptions = LayoutOptions.Start
                        }
                    }
                }
            };

            username = new Entry();
            username.Placeholder = "@poli.ufrj.br";
            username.HorizontalOptions = LayoutOptions.FillAndExpand;

            usernameCell = new ViewCell
            {
                View = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 10,
                    Padding = new Thickness(10.0, 0.0),
                    Children = {
                        new Label {
                            Text = language.usernameText,
                            FontSize = 18,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        },
                        username
                    }
                }
            };

            password = new Entry();
            password.IsPassword = true;
            password.HorizontalOptions = LayoutOptions.FillAndExpand;

            passwordCell = new ViewCell
            {
                View = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 10.0,
                    Padding = new Thickness(10.0, 0.0),
                    Children = {
                        new Label {
                            Text = language.passwordText,
                            FontSize = 18,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center
                        },
                        password
                    }
                }
            };


            login = new Button()
            {
                Text = language.loginText,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Command = new Command(async o =>
                {
                    await LoginButtonPressed();
                    if (isLoggedIn == true)
                    {
                        await Navigation.PopModalAsync();
                        instanceToReturn.credentialsValid = true;
                        instanceToReturn.user = username.Text;
                        instanceToReturn.Resume();
                    }
                })
            };

            signup = new Button()
            {
                Text = language.signupText,
                HorizontalOptions = LayoutOptions.End,
                Command = new Command(async o =>
                {
                    SignupButtonPressed();
                    if (isLoggedIn == true) await Navigation.PopModalAsync();
                })
            };
        }

        // Async task to draw the login page
        public Task<bool> SignInAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Content = new StackLayout
            {
                Children = {
                    new TableView
                    {
                        Intent = TableIntent.Form,
                        HasUnevenRows = true,
                        Root = new TableRoot
                        {
                            new TableSection
                            {
                                titleCell,
                                usernameCell,
                                passwordCell
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Spacing = 1.0,
                        Padding = 10.0,
                        Children =
                        {
                            login,
                            signup
                        }
                    }
                }
            };
            return tcs.Task;
        }

        // Method called when the login button is pressed
        async private Task<bool> LoginButtonPressed()
        {
            var tcs = new TaskCompletionSource<bool>();

            // Check if either field is empty. If so, display an error and return.
            if ( String.IsNullOrEmpty(username.Text) || String.IsNullOrEmpty(password.Text))
            {
                await DisplayAlert("Error", "Username or password field empty.", "OK");
                tcs.SetResult(false);
                return tcs.Task.Result;
            }

            // Get username from input box
            string currUsername;
            // If the user typed in an email, remove everything after @
            if (username.Text.Contains("@"))
            {
                currUsername = username.Text.Remove(username.Text.IndexOf("@"));
            }
            // Else set the username to the entered text
            else currUsername = username.Text;

            // Encrypt the password using an MD5 hash
            byte[] currPassword = System.Text.Encoding.ASCII.GetBytes(password.Text);
            var hashPassword = System.Security.Cryptography.MD5.Create().ComputeHash(currPassword);
            // Print encrypted password to a string
            StringBuilder encryptedPassword = new StringBuilder();
            for (int i = 0; i < hashPassword.Length; i++)
            {
                encryptedPassword.Append(hashPassword[i].ToString("X2"));
            }
            // Do user validation tasks
            // To-Do Implement validation tasks. For now just print it out.
            await DisplayAlert("Logging In", "Username: " + currUsername + "\n" + "Password (Hash): "  + encryptedPassword.ToString(), "OK");
            await DisplayAlert("Connecting...", "Attempting to connect to the server.", "OK");
            if (serverConnection.startConnection())
            {
                await DisplayAlert("Connected to Server!", "Server connection established successfully.", "OK");
                await DisplayAlert("Testing...", "Attempting to communicate with the server.", "OK");
                string returnData = serverConnection.sendMessage();
                await DisplayAlert("Success!", returnData, "OK");
                isLoggedIn = true;
            }
            else await DisplayAlert("Failure.", "Could not connect to the server.", "OK");

            tcs.SetResult(true);
            return tcs.Task.Result;
        }

        // Method to call when the sign up button is pressed
        private void SignupButtonPressed()
        {
            DisplayAlert("Not yet implemented.", "Nobody here but us chickens.", "BA-CAW!");
        }
    }
}
