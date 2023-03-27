using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class FTMServerManager
    {
        private readonly Dictionary<string, SecurityManager> _securityClients;

        public FTMServerManager()
        {
            _securityClients = new Dictionary<string, SecurityManager>();
        }

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static async Task WaitLock()
        {
            await _semaphore.WaitAsync();
        }

        public static void Release()
        {
            _semaphore.Release();
        }

        public void AddOrClearUser(string id)
        {
            if(string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException("UserGuid is not valid"); }

            if(_securityClients.ContainsKey(id))
            {
                _securityClients[id].Clear();
            }
            else
            {
                _securityClients.Add(id, new SecurityManager());
            }
        } 

        public FTMessageServer SetAsyncKeyAndReturnSyncKey(string id, string async_public_key)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException("UserGuid is not valid"); }
            if (string.IsNullOrWhiteSpace(async_public_key)) { throw new ArgumentNullException("Public key is not valid"); }

            _securityClients[id].SetUnsafeAsyncKey(async_public_key);

            string syncKey = _securityClients[id].GenerateAndGetSyncKey();

            return FTMessageServer.SecureSyncKey(
                    _securityClients[id],
                    syncKey);
        }



        public bool UserExist(string id)
        {
            return _securityClients.ContainsKey(id);
        }

        public SecurityManager this[string id]
        {
            get
            {
                return _securityClients[id];
            }
        }
    }
}
