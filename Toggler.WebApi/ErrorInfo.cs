using Newtonsoft.Json;

namespace Toggler.WebApi
{
    public class ErrorInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
