using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;

namespace AppCore.Services
{
    public class FTMServerManager
    {
        private readonly Dictionary<string, SecurityManager> _securityClients;
        private readonly Dictionary<string, UserToken> _tokenClients;

        public FTMServerManager()
        {
            _securityClients = new Dictionary<string, SecurityManager>();
            _tokenClients = new Dictionary<string, UserToken>();
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

        public FTMessageServer SetPublicSignKeyAndReturnServerPkSignKey(string id, string? client_sign_key)
        {

            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException("UserGuid is not valid"); }
            if (string.IsNullOrWhiteSpace(client_sign_key)) { throw new ArgumentNullException("Public key is not valid"); }

            _securityClients[id].SetPublicKeySignature(client_sign_key);
            return FTMessageServer.GenerateNotSecure(_securityClients[id].GenerateSignatureKeyAndReturnPubKey());
        }

        public string? UserExistAndCheckState(string id)
        {
            if (!_securityClients.ContainsKey(id)) return APIError.USER_ID_NOT_EXIST;
            if (_tokenClients[id].Expired) return APIError.USER_EXPIRED;
            if (_tokenClients[id].QuotaExceeded) return APIError.QUOTA_EXCEEDED;

            _tokenClients[id].ResetQuotaIfExpired();
            _tokenClients[id].IncreaseQuota();
            _tokenClients[id].ResetTimeOut();
            return null;
        }

        public bool UserExist(string id)
        {
            return _securityClients.ContainsKey(id);
        }

        public bool CheckToken(string id, EndPointArgs arg)
        {
            return _tokenClients.ContainsKey(id) && _tokenClients[id].Token == arg.Token;
        }

        public FTMessageServer GenerateToken(string id, Utilisateur user)
        {
            if (!_tokenClients.ContainsKey(id))
            {
                _tokenClients.Add(id, new UserToken(Guid.NewGuid().ToString(), user.UtilisateurId));
            }
            else
            {
                _tokenClients[id] = new UserToken(Guid.NewGuid().ToString(), user.UtilisateurId);
            }
            return FTMessageServer.GenerateSecure(_securityClients[id],_tokenClients[id].Token);
        }

        public SecurityManager this[string id]
        {
            get
            {
                return _securityClients[id];
            }
        }
    }

    public class UserToken
    {
        public string Token;
        public Guid UtilisateurId;

        private DateTime _expiration_date;
        private int _quota;
        private DateTime _quota_reset;

        private const int QUOTA_MAX = 50;
        private const int MINUTES_EXPIRATIONS = 60;
        private const int MINUTES_QUOTA_RESET = 5;

        public UserToken(string token, Guid utilisateurId)
        {
            Token = token;
            UtilisateurId = utilisateurId;

            _expiration_date = DateTime.Now.AddMinutes(MINUTES_EXPIRATIONS);
            _quota_reset = DateTime.Now.AddMinutes(MINUTES_QUOTA_RESET);
            _quota = 0;
        }

        public bool Expired => _expiration_date < DateTime.Now;

        public bool QuotaExceeded
        {
            get
            {
                if (_quota_reset > DateTime.Now)
                {
                    return (_quota >= QUOTA_MAX);
                }
                else
                {
                    return false;
                }
            }
        }

        public void IncreaseQuota() => _quota++;

        public void ResetQuotaIfExpired()
        {
            if (_quota_reset <= DateTime.Now)
            {
                _quota_reset = DateTime.Now.AddMinutes(MINUTES_QUOTA_RESET);
                _quota = 0;
            }
        }

        public void ResetTimeOut()
        {
            _expiration_date = DateTime.Now.AddMinutes(MINUTES_EXPIRATIONS);
        }
    }
}
