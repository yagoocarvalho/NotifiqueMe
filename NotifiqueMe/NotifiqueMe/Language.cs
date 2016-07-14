using Java.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotifiqueMe
{
    enum textLanguage { English, Portuguese }

    class LanguageModule
    {
        // Class operation variables, do not touch.
        private static LanguageModule instance = null;
        private static readonly object padlock = new object();

        // Default language to use when system language cannot be identified.
        private static readonly textLanguage defaultLanguage = textLanguage.English;
        private static textLanguage language;

        // Translation variables
        public string notifiqueMeText;
        public string usernameText;
        public string passwordText;
        public string loginText;
        public string signupText;

        // Initializes the LanguageModule class
        public LanguageModule()
        {
            // Parse default system language and compare it to enumerated languages.
            textLanguage lang;

            // If the system language matches an enumerated language, set it as default.
            if (Enum.TryParse(Locale.Default.GetDisplayLanguage(Locale.English), out lang))
            {
                language = lang;
            }

            // Else set default language to the default language
            else language = defaultLanguage;

            // Change the app language to the newly defined language
            ChangeLanguage(language);
        }

        // Gets a singleton instance of the LanguageModule class
        public static LanguageModule Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LanguageModule();
                    }
                    return instance;
                }
            }
        }

        // Changes the language currently in use
        public void ChangeLanguage(textLanguage lang)
        {
            language = lang;
            switch (lang)
            {
                // Definitions for english language variables
                case textLanguage.English:
                    notifiqueMeText = "NotifiqueMe";
                    usernameText = "Username";
                    passwordText = "Password";
                    loginText = "Log In";
                    signupText = "Sign Up";

                    break;
                // Definitions for portuguese language variables
                case textLanguage.Portuguese:
                    notifiqueMeText = "NotifiqueMe";
                    usernameText = "Usuário ";
                    passwordText = "Senha   ";
                    loginText = "Entrar";
                    signupText = "Cadastrar-me";

                    break;
            }
        }
    }
}
