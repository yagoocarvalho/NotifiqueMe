using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace NotifiqueMe
{ 

    public class App : Application
	{
        // Current application version
        string version = "dev0.0.1";

        // Language/Translation module singleton
        LanguageModule language = LanguageModule.Instance;

		public App ()
		{
            // Force a language change (for testing purposes)
            language.ChangeLanguage(textLanguage.Portuguese);
            // Set the main page of the application
            MainPage = new NavigationPage(new NavPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
