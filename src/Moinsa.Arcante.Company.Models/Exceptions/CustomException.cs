using System;
using System.Collections.Generic;
using System.Net;

namespace Moinsa.Arcante.Company.Models.Exceptions
{
    public class CustomException : Exception
    {
        private List<string> _errors;
        private HttpStatusCode _statusCode;

        public List<string> ErrorMessages { get { return _errors; } }

        public HttpStatusCode StatusCode { get; }

        public CustomException(string message, List<string> errors,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            this._errors = errors;
            this._statusCode = statusCode;
        }
    }
}
