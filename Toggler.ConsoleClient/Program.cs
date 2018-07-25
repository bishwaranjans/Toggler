using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Toggler.ConsoleClient
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var identityServer = await DiscoveryClient.GetAsync("http://localhost:44370"); //discover the IdentityServer
            if (identityServer.IsError)
            {
                Console.Write(identityServer.Error);
                Console.ReadLine();
                return;
            }

            try
            {
                //Get the token
                var tokenClient = new TokenClient(identityServer.TokenEndpoint, "toggler_auth_client", "secret");
                //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
                var tokenResponse =
                    await tokenClient.RequestResourceOwnerPasswordAsync("Bish", "password", "toggler_auth_api");

                // Set the bearer token before call of the API
                Client.SetBearerToken(tokenResponse.AccessToken);

                // Call API
                var response = await Client.GetAsync("http://localhost:44378/api/toggles");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred : " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
