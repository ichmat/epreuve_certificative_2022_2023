﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;

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

        public async Task<bool> ConnexionStart()
        {
            if(await EstablishConnection())
            {
                return await GetSignKey();
            }
            return false;
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

        public async Task<bool> GetSignKey()
        {
            FTMessageClient msg = FTMessageClient.GenerateNotSecure(
                _id,
                _securityManager.GenerateSignatureKeyAndReturnPubKey()
                );

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(APIRoute.SIGN_KEY, msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
                        _securityManager.SetPublicKeySignature(res.Message);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public async Task<bool> Login(string? pseudo, string? mail, string password)
        {
            if (pseudo == null && mail == null)
                throw new ArgumentNullException("need pseudo or mail (can be both)");

            FTMessageClient msg = FTMessageClient.GenerateSecure(_id,
                _securityManager,
                new Credentials(pseudo, password, mail));

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(APIRoute.ATTEMPT_CONNECTION, msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
                        _token = res.SecureDecrypt<string>(_securityManager);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    
    }
}
