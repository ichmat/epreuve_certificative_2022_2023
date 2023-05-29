using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;
using AppCore.Services.GeneralMessage.Args;

namespace AppCore.Services
{
    public class FTMClientManager
    {
        private readonly string _id;
        private readonly SecurityManager _securityManager;
        private string? _token;

        private readonly HttpClient _client;

        private const string API_URL = "https://7afb-195-200-178-237.ngrok-free.app";

        public Utilisateur? CurrentUser { get; private set; }

        public FTMClientManager() {
            _id = Guid.NewGuid().ToString();
            _securityManager = new SecurityManager();
            _client = new HttpClient();
#if DEBUG
            _client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
#endif
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

        public async Task<bool> IsConnected()
        {
            FTMessageClient msg = FTMessageClient.GenerateNotSecure(
                _id,
                string.Empty
                );

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(APIRoute.SIGN_KEY, msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
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

                        CurrentUser = await SendAndGetResponse<Utilisateur>(new EPGetUserByToken());

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
        
        public async Task<bool> Register(EPCreateUser request)
        {
            FTMessageClient msg = FTMessageClient.GenerateSecure(_id,
               _securityManager,
               request);

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(request.Route(), msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                throw;
            }

            return false;
        }

        public async Task<bool> SendRequest(EndPointArgs request)
        {
            if (_token == null)
                throw new ArgumentNullException("user not connected");

            request.Token = _token;

            FTMessageClient msg = FTMessageClient.GenerateSecure(_id,
               _securityManager,
               request);

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(request.Route(), msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
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

        public async Task<T?> SendAndGetResponse<T>(EndPointArgs request) where T: class
        {
            if (_token == null)
                throw new ArgumentNullException("user not connected");

            request.Token = _token;

            FTMessageClient msg = FTMessageClient.GenerateSecure(_id,
               _securityManager,
               request);

            HttpResponseMessage response = await _client.PostAsJsonAsync(request.Route(), msg);
            if (response.IsSuccessStatusCode)
            {
                FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                if (res != null)
                {
                    return res.SecureDecrypt<T>(_securityManager);
                }
            }

            return null;
        }

        public async Task<T?> SendAndGetResponseStruct<T>(EndPointArgs request) where T : struct
        {
            if (_token == null)
                throw new ArgumentNullException("user not connected");

            request.Token = _token;

            FTMessageClient msg = FTMessageClient.GenerateSecure(_id,
               _securityManager,
               request);

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(request.Route(), msg);
                if (response.IsSuccessStatusCode)
                {
                    FTMessageServer? res = await response.Content.ReadFromJsonAsync<FTMessageServer>();
                    if (res != null)
                    {
                        return res.SecureDecryptStruct<T>(_securityManager);
                    }
                }
            }
            catch
            {
                throw;
            }

            return null;
        }
    }
}
