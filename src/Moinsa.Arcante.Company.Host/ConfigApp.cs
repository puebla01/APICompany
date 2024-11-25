

using Moinsa.Arcante.Company.Host.Security;
using System.Security.Cryptography;

namespace Moinsa.Arcante.Company.Host
{
    public static class ConfigApp
    {
        internal static byte[] _key = { 33, 55, 12, 222, 66, 33, 56, 55, 88, 22, 45, 67, 98, 33, 32, 123, 234, 53, 222, 223, 224, 225, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73 };
        internal static byte[] _iv = { 33, 55, 12, 222, 55, 80, 56, 55, 88, 22, 45, 67, 98, 33, 32, 123 };
        internal static string filePathSettings = null;
        private static IConfiguration _configuration = null;

        static ConfigApp()
        {
            filePathSettings = $@"{AppDomain.CurrentDomain.BaseDirectory}\appsettings.json";
            if (!System.IO.File.Exists(filePathSettings))
            {
                filePathSettings = $@"{AppDomain.CurrentDomain.BaseDirectory}\bin\appsettings.json";
                if (!System.IO.File.Exists(filePathSettings))
                {
                    throw new System.IO.FileNotFoundException($"File config appsettings.json not found");
                }
            }

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .Add<WritableJsonConfigurationSource>((s) =>
                {
                    s.FileProvider = null;
                    s.Path = filePathSettings;
                    s.Optional = false;
                    s.ReloadOnChange = true;
                    s.ResolveFileProvider();
                })
                .AddJsonFile(filePathSettings);
            _configuration = builder.Build();
        }

        public static IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        private static string _connectionStringArcante = null;

        public static string ConnectionStringArcante
        {
            get
            {
                if (_connectionStringArcante == null)
                {
                    var aux = Configuration.GetSection("ConnectionStrings:OrganizacionesDbConnection")?.Value ?? string.Empty;

                    if (IsEncryted(aux))
                    {
                        _connectionStringArcante = Decrypt(aux, _key, _iv);
                    }
                    else
                    {
                        _connectionStringArcante = aux;
                        aux = Encrypt(_connectionStringArcante, _key, _iv);
                        Configuration.GetSection("ConnectionStrings:OrganizacionesDbConnection").Value = aux;
                    }
                }

                if (string.IsNullOrEmpty(_connectionStringArcante))
                {
                    throw new System.Configuration.ConfigurationErrorsException("No se ha podido leer la cadena de conexión [ConnectionStrings:OrganizacionesDbConnection] del archivo de configuración");
                }

                return _connectionStringArcante;
            }
        }

        private static string _user = null;

        public static string User
        {
            get
            {
                if (_user == null)
                {
                    var aux = Configuration.GetSection("Application:Usuario")?.Value ?? string.Empty;

                    if (IsEncryted(aux))
                    {
                        _user = Decrypt(aux, _key, _iv);
                    }
                    else
                    {
                        _user = aux;
                        aux = Encrypt(_user, _key, _iv);
                        Configuration.GetSection("Application:Usuario").Value = aux;
                    }
                }

                if (string.IsNullOrEmpty(_user))
                {
                    throw new System.Configuration.ConfigurationErrorsException("No se ha podido leer la cadena de conexión [Application:Usuario] del archivo de configuración");
                }

                return _user;
            }
        }

        private static string _password = null;

        public static string Password
        {
            get
            {
                if (_password == null)
                {
                    var aux = Configuration.GetSection("Application:Contraseña")?.Value ?? string.Empty;

                    if (IsEncryted(aux))
                    {
                        _password = Decrypt(aux, _key, _iv);
                    }
                    else
                    {
                        _password = aux;
                        aux = Encrypt(_password, _key, _iv);
                        Configuration.GetSection("Application:Contraseña").Value = aux;
                    }
                }

                if (string.IsNullOrEmpty(_password))
                {
                    throw new System.Configuration.ConfigurationErrorsException("No se ha podido leer la cadena de conexión [Application:Contraseña] del archivo de configuración");
                }

                return _password;
            }
        }

        internal static string Encrypt(string message, byte[] key, byte[] iv)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(message); // Write all data to the stream.
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        internal static string Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        internal static bool IsEncryted(this string data)
        {
            if (data == null || data.Length < 24)
            {
                return false;
            }

            try
            {
                var decryptedData = Decrypt(data, _key, _iv);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}