using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Moinsa.Arcante.Company.Client.Test
{
    public class Settings
    {
        private static string _user = "1234";
        private static string _pws = "1234";
        private static string _apiUrl = "https://localhost:17117/";
        private static string _applicationName = "ARCANTE";
        private static string _defaultCompany = "DICAS";

        public static string User { get => _user; }

        public static string Password { get => _pws; }

        public static string Url { get => _apiUrl; }

        public static string ApplicationName { get => _applicationName; } 
        
        public static string DefaultCompany { get => _defaultCompany; }
    }
}
