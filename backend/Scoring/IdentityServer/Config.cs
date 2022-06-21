using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IntelART.IdentityServer
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("customerApi", "Custiomer-specific API"),
                new ApiResource("loanApplicationApi", "API for creating and submitting loan appications"),
                new ApiResource("bankInternalApi", "Internal API to be used by the bank employees and systems"),
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection config)
        {
            string customerApplicationUrl = config.GetSection("CustomerApplication")["Url"];
            string bankApplicationUrl = config.GetSection("BankApplication")["Url"];
            string shopApplicationUrl = config.GetSection("ShopApplication")["Url"];
            IConfigurationSection extenralClients = config.GetSection("ExternalClients");
            string oidcEndpoint = "/signin-oidc";
            string customerOidcEndpoint = string.Format("{0}{1}", customerApplicationUrl, oidcEndpoint);
            string bankOidcEndpoint = string.Format("{0}{1}", bankApplicationUrl, oidcEndpoint);
            string shopOidcEndpoint = string.Format("{0}{1}", shopApplicationUrl, oidcEndpoint);
            List<Client> clients = new List<Client>();
            clients.Add(new Client
            {
                ClientId = "customerApplication",
                ClientName = "Online application to be used by the custiomer",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowAccessTokensViaBrowser = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { customerOidcEndpoint },
                PostLogoutRedirectUris = { customerOidcEndpoint },
                AllowedCorsOrigins = { customerApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "loanApplicationApi",
                    "customerApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            });

            clients.Add(new Client
            {
                ClientId = "customerApplication2",
                ClientName = "Online application to be used by the custiomer",
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowAccessTokensViaBrowser = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { customerOidcEndpoint },
                PostLogoutRedirectUris = { customerOidcEndpoint },
                AllowedCorsOrigins = { customerApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "loanApplicationApi",
                    "customerApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            });

            clients.Add(new Client
            {
                ClientId = "shopApplication",
                ClientName = "Online application to be used at shops",
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowAccessTokensViaBrowser = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { shopOidcEndpoint },
                PostLogoutRedirectUris = { shopOidcEndpoint },
                AllowedCorsOrigins = { shopApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "loanApplicationApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            });

            clients.Add(new Client
            {
                ClientId = "shopApplication2",
                ClientName = "Online application to be used at shops",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowAccessTokensViaBrowser = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { shopOidcEndpoint },
                PostLogoutRedirectUris = { shopOidcEndpoint },
                AllowedCorsOrigins = { shopApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "loanApplicationApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            });

            clients.Add(new Client
            {
                ClientId = "bankApplication",
                ClientName = "Online application to be used at bank",
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                ////AllowAccessTokensViaBrowser = true,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { bankOidcEndpoint },
                PostLogoutRedirectUris = { bankOidcEndpoint },
                AllowedCorsOrigins = { bankApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "bankInternalApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            });

            if (extenralClients != null)
            {
                IEnumerable<IConfigurationSection> children = extenralClients.GetChildren();
                if (children != null)
                {
                    foreach (IConfigurationSection clientConfiguration in children)
                    {
                        string clientId = clientConfiguration["ClientId"];
                        string clientName = clientConfiguration["ClientName"] ?? "";
                        IEnumerable<string> secrets = null;
                        IConfigurationSection secretsConfiguration = clientConfiguration.GetSection("Secrets");
                        if (secretsConfiguration != null)
                        {
                            secrets = secretsConfiguration.GetChildren().Select(item => item.Value);
                        }
                        if (clientId != null
                            && secrets != null
                            && !clients.Any(c => c.ClientId == clientId))
                        {
                            clients.Add(GetClient(
                                clientId,
                                clientName,
                                secrets,
                                customerOidcEndpoint,
                                customerApplicationUrl));
                        }
                    }
                }
            }

            if (!clients.Any(c => c.ClientId == "haypost"))
            {
                clients.Add(GetClient(
                    "haypost",
                    "HayPost API",
                    new[] { "646CBBDD-9D1E-46C5-B3C5-988FE8720F0F09952545-8E5A-4FCA-981D-574E3367B3CF" },
                    customerOidcEndpoint,
                    customerApplicationUrl));
            }
            else if (!clients.Any(c => c.ClientId == "sfl"))
            {
                clients.Add(GetClient(
                    "sfl",
                    "SFL API",
                    new[] { "A5B8EDF6-11D0-4317-B54C-152CA88FD160DAD2D14B-908E-4FC3-BBBD-E6B95E84168B" },
                    customerOidcEndpoint,
                    customerApplicationUrl));
            }

            return clients;
        }

        private static Client GetClient(
            string clientId,
            string clientName,
            IEnumerable<string> secrets,
            string shopOidcEndpoint,
            string shopApplicationUrl)
        {
            return new Client
            {
                ClientId = clientId,
                ClientName = clientName,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowAccessTokensViaBrowser = true,

                ClientSecrets = new List<Secret>(secrets.Select(s => new Secret(s.Sha256()))),

                RedirectUris = { shopOidcEndpoint },
                PostLogoutRedirectUris = { shopOidcEndpoint },
                AllowedCorsOrigins = { shopApplicationUrl },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    "loanApplicationApi",
                },

                RequireConsent = false,
                AllowOfflineAccess = true,
            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                };
        }

        public static void SetupIdentityServer(IdentityServerOptions options)
        {
            options.UserInteraction.LoginUrl = "/Authentication/Login";
            options.UserInteraction.LogoutUrl = "/Authentication/Logout";
        }
    }
}
