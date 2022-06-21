using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests
{
    public class KeyVaultManagerTests
    {

        //public async void Test()
        //{
        //    //AsyncPageable<SecretProperties> allSecrets = _secretClient.GetPropertiesOfSecretsAsync();
        //    //await foreach (SecretProperties secretProperties in allSecrets)
        //    //{
        //    //    Console.WriteLine(secretProperties.Name);
        //    //}

        //    // Get the all secrets
        //    var secrets = await _secretClient.GetPropertiesOfSecretsAsync()
        //        .ToListAsync()
        //        .ConfigureAwait(false);

        //    var utcNow = DateTimeOffset.UtcNow;
        //    var results = new Dictionary<string, object>();
        //    foreach (var secret in secrets)
        //    {
        //        // Get the all versions of the given secret
        //        // Filter only enabled versions
        //        // Sort by the created date in a reverse order
        //        var versions = await _secretClient.GetPropertiesOfSecretVersionsAsync(secret.Name)
        //                                   .WhereAwait(p => new ValueTask<bool>(p.Enabled.GetValueOrDefault() == true))
        //                                   .OrderByDescendingAwait(p => new ValueTask<DateTimeOffset>(p.CreatedOn.GetValueOrDefault()))
        //                                   .ToListAsync()
        //                                   .ConfigureAwait(false);

        //        // Do nothing if there is no version enabled
        //        if (!versions.Any())
        //        {
        //            continue;
        //        }

        //        // Do nothing if there is only one version enabled
        //        if (versions.Count < 2)
        //        {
        //            continue;
        //        }

        //        // Do nothing if the latest version was generated less than a day ago
        //        if (versions.First().CreatedOn.GetValueOrDefault() <= utcNow.AddDays(-1))
        //        {
        //            continue;
        //        }

        //        // Disable all versions except the first (latest) one
        //        var candidates = versions.Skip(1).ToList();
        //        var result = new List<SecretProperties>() { versions.First() };
        //        foreach (var candidate in candidates)
        //        {
        //            candidate.Enabled = false;
        //            var response = await _secretClient.UpdateSecretPropertiesAsync(candidate).ConfigureAwait(false);

        //            result.Add(response.Value);
        //        }

        //        results.Add(secret.Name, result);
        //    }
        //}

        [TestMethod]
        public async void TestKV()
        {
            await Task.Delay(500);
            return;

        }
    }
}
