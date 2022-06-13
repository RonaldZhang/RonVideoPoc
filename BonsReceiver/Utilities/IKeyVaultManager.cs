using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RonVideo.Utilities
{
    public interface IKeyVaultManager
    {
        public Task<string> GetSecret(string key);
    }
}
