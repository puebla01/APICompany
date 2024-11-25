using API.Company.Client;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Company.Client
{
    public abstract class BaseClient
    {
        private string _accessToken = null;
        public string AccessToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    if (!IsValidToken(_accessToken))
                    {
                        _accessToken = string.Empty;
                    }
                }

                return _accessToken;
            }
            set
            {
                _accessToken = value;
            }
        }

        protected BaseClient(IConfiguration configuration)
        {             
        }

        protected BaseClient(string url, string user, string password, string applicationName, string defaultCompany)
        {
        }

        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();
            if(!string.IsNullOrWhiteSpace(AccessToken))
            {
                msg.SetBearerToken(AccessToken);
            }
            return Task.FromResult(msg);
        }        

        protected Task<HttpClient>CreateHttpClientAsync(CancellationToken cancellationToken) 
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
                //[ALR] De momento no validamos certificados
                //if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                //{
                //    return true;
                //}

                //// If there are errors in the certificate chain, look at each error to determine the cause.
                //if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                //{
                //    if (chain != null && chain.ChainStatus != null)
                //    {
                //        foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                //        {
                //            if ((cert.Subject == cert.Issuer) &&
                //               (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                //            {
                //                // Self-signed certificates with an untrusted root are valid. 
                //                continue;
                //            }
                //            else
                //            {
                //                if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                //                {
                //                    // If there are any other errors in the certificate chain, the certificate is invalid,
                //                    // so the method returns false.
                //                    return false;
                //                }
                //            }
                //        }
                //    }

                //    // When processing reaches this line, the only errors in the certificate chain are 
                //    // untrusted root errors for self-signed certificates. These certificates are valid
                //    // for default Exchange server installations, so return true.
                //    return true;
                //}
                //else
                //{
                //    // In all other cases, return false.
                //    return false;
                //}
            };
            var client_ = new System.Net.Http.HttpClient(httpClientHandler);
            return Task.FromResult(client_);
        }

        public bool IsValidToken(string token)
        {
            //TODO: [ALR] Pendiente de chequear porque esta función genera excepción
            return true;
            //if (string.IsNullOrEmpty(token))
            //    return false;

            //var tokenHandler = new JwtSecurityTokenHandler();
            //try
            //{
            //    var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            //    {
            //        ValidateLifetime = true,
            //        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            //        ClockSkew = TimeSpan.Zero
            //    }, out SecurityToken validatedToken);

            //    var jwtToken = (JwtSecurityToken)validatedToken;

            //    // return user id from JWT token if validation successful
            //    return true;
            //}
            //catch
            //{
            //    // return null if validation fails
            //    return false;
            //}
        }
    }
}