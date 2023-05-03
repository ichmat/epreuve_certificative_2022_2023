using AppCore.Context;
using AppCore.Models;
using AppCore.Services;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Produces("application/json")]
    public abstract class FreshTechController : Controller
    {
        protected readonly FTDbContext dbContext;

        private readonly string[] notToLog = new string[] {
            "ProcessAndCheckToken",
            "ProcessResponse",
        };

        protected FreshTechController(FTDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Procède au vérification du message : <br></br>
        /// - Attend l'autorisation de modification <br></br>
        /// - Vérifie la validité de l'utilisateur (id valide, quota, expiration) <br></br>
        /// - Déchiffre le contenu du message<br></br>
        /// - Vérifie la validité du token
        /// </summary>
        /// <typeparam name="T">le type de donnée traité pour le déchiffrage ayant comme type parent <see cref="EndPointArgs"/> </typeparam>
        /// <param name="message">le message du client</param>
        /// <param name="process">la fonction a executé si la vérification passe</param>
        /// <returns>La réponse de l'API</returns>
        protected async Task<IActionResult> ProcessAndCheckToken<T>(FTMessageClient message, Func<T, IActionResult> process, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") where T : EndPointArgs
        {
            return await ProcessResponse<T>(message, (args) =>
            {
                if(Program.serverManager.CheckToken(message.UserGuid, args))
                {
                    LogAction(memberName, TypeLog.Info);
                    Program.serverManager.ProcessToken(message.UserGuid);
                    return process.Invoke(args);
                }
                else
                {
                    LogAction(memberName, TypeLog.Error);
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }
            });
        }

        /// <summary>
        /// Procède au vérification du message : <br></br>
        /// - Attend l'autorisation de modification <br></br>
        /// - Vérifie la validité de l'utilisateur (id valide, quota, expiration)<br></br>
        /// - Déchiffre le contenu du message
        /// </summary>
        /// <typeparam name="T">le type de donnée traité pour le déchiffrage</typeparam>
        /// <param name="message">le message du client</param>
        /// <param name="process">la fonction a executé si la vérification passe</param>
        /// <returns>La réponse de l'API</returns>
        protected async Task<IActionResult> ProcessResponse<T>(FTMessageClient message, Func<T, IActionResult> process, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") where T : class
        {
            await FTMServerManager.WaitLock();
            LogAction(memberName);

            IActionResult res;

            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    T? obj = message.SecureDecrypt<T>(Program.serverManager[message.UserGuid]);
                    if(obj != null)
                    {
                        res = process.Invoke(obj);
                    }
                    else
                        res = BadRequest(APIError.BAD_FORMAT_DATA);
                }
                else
                {
                    res = BadRequest(err);
                }
            }
            catch (Exception ex)
            {
                res = BadRequest(APIError.CANCELED_REQUEST);
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;
        }

        /// <summary>
        /// Procède au vérification du message : <br></br>
        /// - Attend l'autorisation de modification <br></br>
        /// - Vérifie la validité de l'utilisateur (id valide, quota, expiration)
        /// </summary>
        /// <param name="message">le message du client</param>
        /// <param name="process">la fonction a executé si la vérification passe</param>
        /// <returns>La réponse de l'API</returns>
        protected async Task<IActionResult> ProcessResponse(FTMessageClient message, Func<IActionResult> process, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            await FTMServerManager.WaitLock();

            LogAction(memberName);

            IActionResult res;

            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    res = process.Invoke();
                }
                else
                {
                    res = BadRequest(err);
                }
            }
            catch (Exception ex)
            {
                res = BadRequest(APIError.CANCELED_REQUEST);
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;
        }

        /// <summary>
        /// Génère un message serveur
        /// </summary>
        /// <param name="UserGuid">l'id de connexion temporaire</param>
        /// <param name="data">les données dans le message à chiffrer et signer</param>
        /// <returns>le message du serveur</returns>
        protected FTMessageServer Message(string UserGuid, object data)
        {
            return FTMessageServer.GenerateSecure(
                Program.serverManager[UserGuid],
                data);
        }

        /// <summary>
        /// Génère un message serveur
        /// </summary>
        /// <param name="message">le message du client</param>
        /// <param name="data">les données dans le message à chiffrer et signer</param>
        /// <returns>le message du serveur</returns>
        protected FTMessageServer Message(FTMessageClient message, object data)
        {
            return FTMessageServer.GenerateSecure(
                Program.serverManager[message.UserGuid],
                data);
        }

        private void LogAction(string memberName, TypeLog type = TypeLog.Info)
        {
            if(notToLog.Contains(memberName))
            {
                return;
            }
            Program.Log(memberName, type);
        }

        /// <summary>
        /// Récupère l'utilisateur par l'id de connexion temporaire
        /// </summary>
        /// <param name="userGuid">l'id de connexion temporaire</param>
        /// <returns>L'utilisateur trouvé</returns>
        protected Utilisateur? GetUserByUserGuid(string userGuid)
        {
            Guid? userid = Program.serverManager.GetUserGuidByUserId(userGuid);
            if (userid != null)
            {
                return dbContext.Utilisateurs.FirstOrDefault(x => x.UtilisateurId == userid);
            }

            return null;
        }
    }
}
