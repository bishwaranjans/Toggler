using System;

namespace Toggler.WebApi
{
    public class HttpResourceNotFoundException : Exception
    {
        public HttpResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
