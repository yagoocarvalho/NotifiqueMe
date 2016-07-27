using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifiqueMe.Server
{
    enum accountType { Basic }

    class UserCredential
    {
        string username;
        string passhash;
        string firstName;
        string lastName;
        string email;
        accountType accType;
        
    }
}
