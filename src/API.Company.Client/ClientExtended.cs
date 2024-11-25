using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;

namespace API.Company.Client
{
    public partial class Client
    {
        private IConfiguration _configuration;
        internal static byte[] _key = { 33, 55, 12, 222, 66, 33, 56, 55, 88, 22, 45, 67, 98, 33, 32, 123, 234, 53, 222, 223, 224, 225, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73 };
        internal static byte[] _iv = { 33, 55, 12, 222, 55, 80, 56, 55, 88, 22, 45, 67, 98, 33, 32, 123 };
        private const string SETTING_SERVICE_URL =  "ServiceCompanies:Url";
        private const string SETTING_SERVICE_USER = "ServiceCompanies:User";
        private const string SETTING_SERVICE_PASSWORD = "ServiceCompanies:Password";
        private const string SETTING_SERVICE_APPLICATION_NAME = "ServiceCompanies:ApplicationName";
        private const string SETTING_SERVICE_DEFAULT_COMPANY = "ServiceCompanies:DefaultCompany";

        public Client(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            _baseUrl = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_URL);
            _user = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_USER, true);
            _password = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_PASSWORD, true);
            _applicationName = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_APPLICATION_NAME);
            _defaultCompany = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_DEFAULT_COMPANY);
        }

        public Client(string url, string user, string password, string applicationName, string defaultCompany) :
            base(url, user, password, applicationName, defaultCompany)
        {
            _baseUrl = url;
            _applicationName = applicationName;
            _defaultCompany = defaultCompany;
            _user = user;
            _password = password;
        }

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
             CommonPrepareRequest(request);
        }
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder)
        {
        }

        private void CommonPrepareRequest(System.Net.Http.HttpRequestMessage request)
        {
            if (BaseUrl== null)
            {
                throw new ArgumentNullException($"La propiedad {nameof(BaseUrl)} no está establecida y no es posible utilizar llamadas al servicio");
            }

            if (request.RequestUri == null)
                return;

            if (!request.RequestUri.PathAndQuery.Contains("/token/authenticate") && string.IsNullOrEmpty(AccessToken))
            {
                AccessToken = TokenAuthenticateAsync("", new Models.CredentialsModel()
                {
                    Username = _user,
                    Password = _password
                }).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(AccessToken))
                {
                    request.SetBearerToken(AccessToken);
                }
            }
        }

        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            if (response != null)
            {
                if (response.RequestMessage.RequestUri.PathAndQuery.Contains("/token/authenticate"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        AccessToken = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        AccessToken = string.Empty;
                    }
                }
            }
        }

        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }

            if (ReadResponseAsString || typeof(T).FullName == typeof(System.String).FullName)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {                   
                    return new ObjectResponseResult<T>((T)Convert.ChangeType(responseText, typeof(T)), responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new SwaggerException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSerializerSettings);
                        var typedBody = serializer.Deserialize<T>(jsonTextReader);
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new SwaggerException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }

        public static T GetValueFromConfiguration<T>(IConfiguration configuration, string settingName, bool forceEncrypt = false, bool allowNull = true)
        {
            if (configuration == null)
            { 
                return default(T);
            }

            object value = configuration.GetSection(settingName)?.Value;

            if (value == null)
                value = default(T);
            else 
            {
                if (forceEncrypt && typeof(T) == typeof(string))
                {
                    string aux = (string)value;
                    if (IsEncrypted((string)value))
                    {
                        value = Decrypt(aux, _key, _iv);
                    }
                    else
                    {
                        aux = Encrypt((string)value, _key, _iv);
                        configuration.GetSection(settingName).Value = aux;
                    }
                }
            }

            if (value == null && allowNull == false)
            {
                throw new System.Configuration.ConfigurationErrorsException($"El valor de la configuración [{settingName}] es obligatorio y no está establecido");
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(this._baseUrl))
                {
                    _baseUrl = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_URL);
                }

                return _baseUrl;
            }
        }

        private string _applicationName = null;
        public string ApplicationName
        {
            get
            {
                if (_applicationName == null)
                {
                    _applicationName = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_APPLICATION_NAME);
                }

                return _applicationName;
            }
        }

        private string _defaultCompany = null;
        public string DefaultCompany
        {
            get
            {
                if (_defaultCompany == null)
                {
                    _defaultCompany = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_DEFAULT_COMPANY);
                }

                return _defaultCompany;
            }
        }

        private string _user = null;
        public string User
        {
            get
            {
                if (_user == null)
                {
                    _user = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_USER, true);
                }

                return _user;
            }
        }

        private string _password = null;
        public string Password
        {
            get
            {
                if (_password == null)
                {
                    _password = GetValueFromConfiguration<string>(_configuration, SETTING_SERVICE_PASSWORD, true);
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

        internal static  string Decrypt(string cipherText, byte[] key, byte[] iv)
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

        internal static bool IsEncrypted(string data)
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
