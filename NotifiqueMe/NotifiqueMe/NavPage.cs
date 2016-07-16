using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NotifiqueMe
{
    public class NavPage : Page
    {
        // Storage Variables
        public string user;
        public bool credentialsValid;

        // Constructor of the NavPage class
        public NavPage()
        {
            Title = "NotifiqueMe";
            credentialsValid = false;
            MainTask();
        }

        // Async method to create a login page
        async void Login()
        {
            // Instantiate a LoginPage object, passing this as the page to return to when done.
            LoginPage loginPage = new LoginPage(this);
            // Push it modally to the top of the stack.
            await Navigation.PushModalAsync(loginPage);
            // Wait for it to finish drawing before continuing.
            await loginPage.SignInAsync();
        }

        // Public function to resume the main task when returning from from another page.
        public void Resume()
        {
            MainTask();
        }

        // The Main loop to execute in the app.
        async private void MainTask()
        {
            // Do things
            int loopcount = 0;
            while (credentialsValid == true)
            {
                
                await DisplayAlert("Welcome!", "You are now logged in.", "OK");
                if (loopcount < 10)
                {
                    await DisplayAlert("Yeah.", "Now this will loop forever until we actually add something here.", "Got it.");
                    loopcount++;
                }
                else
                {
                    await DisplayAlert("Seriously!?", "You went through this " + loopcount.ToString() + " times? If you have time to do this go add something here!", "Alright, sheesh.");
                    loopcount++;
                }
                

            }
            // If credentials aren't valid call the login method.
            Login();
        }
    }
}
