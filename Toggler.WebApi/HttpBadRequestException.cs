using System;

namespace Toggler.WebApi
{
    public class HttpBadRequestException : Exception
    {
        public HttpBadRequestException(string message) : base(message)
        {

        }
    }
}
