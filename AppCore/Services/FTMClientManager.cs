using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class FTMClientManager
    {
        private readonly string _id;
        private readonly SecurityManager _securityManager;
        private string? _token;

        private readonly HttpClient _client;

        private const string API_URL = "https://localhost:7252";

        public FTMClientManager() {
            _id = Guid.NewGuid().ToString();
            _securityManager = new SecurityManager();
            _client = new HttpClient();
            _client.BaseAddress = new Uri(API_URL);
        }

        public async Task<bool> EstablishConnection()
        {
            FTMessageClient msg = FTMessageClient.GenerateNotSecure(
                _id,
                _securityManager.GenerateUnsafeAsyncKeysAndReturnPublicKey()
                );
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(APIRoute.ESTABLISH_CONNECTION, msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
                        _securityManager.SetSyncKey(res.ReadSyncKey(_securityManager));
                        return true;
                    }
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
           

            return false;
        }
    }
}
