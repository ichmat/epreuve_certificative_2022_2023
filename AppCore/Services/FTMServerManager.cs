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
        /// <summary>
        /// dictionnaire des informations nécessaire pour le traitement des messages : chiffrement, déchiffrement, signature <br></br>
        /// string : id de connexion temporaire
        /// SecurityManager : l'objet de traitement
        /// </summary>
        private readonly Dictionary<string, SecurityManager> _securityClients;
        private readonly Dictionary<string, UserToken> _tokenClients;

        public FTMServerManager()
        {
            _securityClients = new Dictionary<string, SecurityManager>();
            _tokenClients = new Dictionary<string, UserToken>();
        }

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Attend le semaphore afin de modifier les données de la BDD
        /// </summary>
        /// <returns>tache d'attente</returns>
        public static async Task WaitLock()
        {
            await _semaphore.WaitAsync();
        }

        /// <summary>
        /// Rend le semaphore de modification
        /// </summary>
        public static void Release()
        {
            _semaphore.Release();
        }

        /// <summary>
        /// Ajoute ou remplace l'utilisateur avec l'id de connexion temporaire
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <exception cref="ArgumentNullException">l'id n'est pas valide (vide)</exception>
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

        /// <summary>
        /// À appeler pour enregistrer une connexion temporaire afin que le serveur puisse transmettre la clé synchrone à partir du chiffrement async
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <param name="async_public_key">clé asynchrone public du client</param>
        /// <returns>Le message contenant la clé synchrone</returns>
        /// <exception cref="ArgumentNullException">l'id n'est pas valide (vide) <br></br> <b>ou</b> <br></br> la clé asynchrone n'est pas valide (vide)</exception>
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

        /// <summary>
        /// Enregistre la clé de signature publique du client puis générère une pair de clé de signature du serveur et renvoie la clé public 
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <param name="client_sign_key">clé de signature public du client</param>
        /// <returns>Le message contenant la clé de signature public du serveur</returns>
        /// <exception cref="ArgumentNullException">l'id n'est pas valide (vide) <br></br> <b>ou</b> <br></br> la clé signature n'est pas valide (vide)</exception>
        public FTMessageServer SetPublicSignKeyAndReturnServerPkSignKey(string id, string? client_sign_key)
        {

            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException("UserGuid is not valid"); }
            if (string.IsNullOrWhiteSpace(client_sign_key)) { throw new ArgumentNullException("Public key is not valid"); }

            _securityClients[id].SetPublicKeySignature(client_sign_key);
            return FTMessageServer.GenerateNotSecure(_securityClients[id].GenerateSignatureKeyAndReturnPubKey());
        }

        /// <summary>
        /// Vérifie les information de l'utilisateur à partir de l'id de connexion temporaire
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <returns>S'il y a une erreur, un string non null est envoyer</returns>
        public string? UserExistAndCheckState(string id)
        {
            if (!_securityClients.ContainsKey(id)) return APIError.USER_ID_NOT_EXIST;
           

            
            return null;
        }

        /// <summary>
        /// Obtenir le Guid de l'utilisateur par l'id de connexion temporaire
        /// </summary>
        /// <param name="userId">l'id de connexion temporaire</param>
        /// <returns>le Guid de l'utilisateur</returns>
        public Guid? GetUserGuidByToken(string userId)
        {
            if(_tokenClients.ContainsKey(userId))
                return _tokenClients[userId].UtilisateurId;
            return null;
        }

        /// <summary>
        /// Vérifie si l'id de connexion temporaire existe
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <returns>True : ok <br></br> False : n'existe pas</returns>
        public bool UserExist(string id)
        {
            return _securityClients.ContainsKey(id);
        }

        /// <summary>
        /// Vérifie le token de l'utilisateur
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <param name="arg">le message envoyer</param>
        /// <returns>True : ok <br></br>False : le token ne correspond pas</returns>
        public bool CheckToken(string id, EndPointArgs arg)
        {
            return _tokenClients.ContainsKey(id) && _tokenClients[id].Expired && _tokenClients[id].QuotaExceeded && _tokenClients[id].Token == arg.Token;
        }

        public void ProcessToken(string id)
        {
            _tokenClients[id].ResetQuotaIfExpired();
            _tokenClients[id].IncreaseQuota();
            _tokenClients[id].ResetTimeOut();
        }

        /// <summary>
        /// Genère un token de connexion à partir de l'utilisateur trouver
        /// </summary>
        /// <param name="id">l'id de connexion temporaire</param>
        /// <param name="user">l'utilisateur qui correspond au credential</param>
        /// <returns>Le message avec le token de connexion</returns>
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
