using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RonVideo.Utilities
{
    public class LocalKVManager : IKeyVaultManager
    {
        public async Task<string> GetSecret(string key)
        {
            await Task.Delay(100);
            return key+"secret";
        }
    }
}
