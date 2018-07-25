using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Toggler.IdentityServer
{
    public class Config
    {
        //Defining the InMemory Clients
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "toggler_auth_client",
                    ClientName = "Toggler.Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowOfflineAccess = true,
                    RequireConsent = false,

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris =
                        { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "toggler_auth_api"
                    }
                }
            };
        }

        //Defining the InMemory API's
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("toggler_auth_api", "Toggler.IdentityServer.Api")
            };
        }

        public static List<TestUser> GetUsers()
        {
            List<TestUser> testUsers = new List<TestUser>();

            testUsers.Add(new TestUser()
            {
                SubjectId = "1",
                Username = "Bish",
                Password = "password"
            });

            return testUsers;
        }

        //Support for OpenId connectivity scopes
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return
                 new List<IdentityResource>
                 {
                    new IdentityResources.OpenId(), // Support for the OpenId
                    new IdentityResources.Profile() // To get the profile information
                 };
        }
    }
}
