using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
{
    public static class Helper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        //public static async string GetSecret(string secretName)
        //{ 
        //    string keyVaultUri = "https://applicationsecretsdemo.vault.azure.net/";
 
        //    var client = new SecretClient( keyVaultUri, new DefaultAzureCredential());

        //    var secret = await client.GetSecretAsync(secretName);

        //    return secret;
        //}
    }
}
